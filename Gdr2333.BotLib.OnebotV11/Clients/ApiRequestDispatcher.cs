/*
   Copyright 2025-2026 All contributors of Gdr2333.BotLib

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
*/

using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading.Channels;

namespace Gdr2333.BotLib.OnebotV11.Clients;

/// <summary>
/// OneBot v11 API 请求调度的共享状态。
/// <para>把 API 调用（<see cref="CallAsync(string, CancellationToken?)"/> / <see cref="InvokeAsync{TResult}(string, CancellationToken?)"/> 等）抽到一处，
/// 并通过 <see cref="Outbound"/> 把请求投递给底层 WebSocket 发送循环。</para>
/// </summary>
internal sealed class ApiRequestDispatcher
{
    private readonly ConcurrentDictionary<Guid, Action<OnebotV11ApiResult>> _pending = new();
    private readonly Channel<OnebotV11ApiRequest> _outbound = Channel.CreateUnbounded<OnebotV11ApiRequest>();
    private readonly CancellationToken _cancellationToken;

    public ApiRequestDispatcher(CancellationToken cancellationToken)
    {
        _cancellationToken = cancellationToken;
    }

    /// <summary>
    /// 待发请求通道的读端。发送循环从此处 <see cref="ChannelReader{T}.ReadAsync(CancellationToken)"/> 取出请求。
    /// </summary>
    public ChannelReader<OnebotV11ApiRequest> Outbound => _outbound.Reader;

    /// <summary>
    /// 收到 API 响应时调用，匹配并执行对应回调。
    /// </summary>
    public bool TryDeliver(OnebotV11ApiResult result) =>
        _pending.TryRemove(result.Guid, out var action) && Invoke(action, result);

    private static bool Invoke(Action<OnebotV11ApiResult> action, OnebotV11ApiResult result)
    {
        action(result);
        return true;
    }

    /// <summary>
    /// 发送不关心返回值的 API 调用。
    /// </summary>
    public Task CallAsync(string apiName, CancellationToken? cancellationToken = null) =>
        SendAndAwait<object?>(apiName, requestData: null, cancellationToken, hasResult: false);

    /// <summary>
    /// 发送带请求参数、不关心返回值的 API 调用。
    /// </summary>
    public Task CallAsync<TRequest>(string apiName, TRequest requestData, CancellationToken? cancellationToken = null) =>
        SendAndAwait<object?>(apiName, requestData, cancellationToken, hasResult: false);

    /// <summary>
    /// 发送需要返回值的 API 调用。
    /// </summary>
    public Task<TResult> InvokeAsync<TResult>(string apiName, CancellationToken? cancellationToken = null) =>
        SendAndAwait<TResult>(apiName, requestData: null, cancellationToken, hasResult: true);

    /// <summary>
    /// 发送带请求参数、需要返回值的 API 调用。
    /// </summary>
    public Task<TResult> InvokeAsync<TRequest, TResult>(string apiName, TRequest requestData, CancellationToken? cancellationToken = null) =>
        SendAndAwait<TResult>(apiName, requestData, cancellationToken, hasResult: true);

    private Task<TResult> SendAndAwait<TResult>(string apiName, object? requestData, CancellationToken? cancellationToken, bool hasResult)
    {
        var realToken = cancellationToken ?? _cancellationToken;
        var tcs = new TaskCompletionSource<TResult>();
        var guid = Guid.NewGuid();
        var completed = false;

        var registration = new StrongBox<CancellationTokenRegistration>();
        registration.Value = realToken.Register(() =>
        {
            if (!completed)
            {
                completed = true;
                tcs.TrySetCanceled();
                _pending.TryRemove(guid, out _);
            }
            registration.Value.Dispose();
        });

        _pending.TryAdd(guid, result =>
        {
            completed = true;
            registration.Value.Dispose();
            switch (result.Retcode)
            {
                case 0:
                    if (!hasResult)
                    {
                        tcs.TrySetResult(default!);
                        return;
                    }
                    if (result.Data is null)
                    {
                        tcs.TrySetException(new OnebotV11ClientException(
                            $"服务端认为任务完成成功，但没有返回结果。调用ID={result.Guid}，错误码={result.Retcode}，错误={result.ErrorMessage}，错误描述={result.ErrorMessageEx}"));
                        return;
                    }
                    try
                    {
                        var deserialized = result.Data.Value.Deserialize<TResult>();
                        if (deserialized is not null)
                        {
                            tcs.TrySetResult(deserialized);
                            return;
                        }
                        tcs.TrySetException(new OnebotV11ClientException(
                            $"服务端返回的结果反序列化为 null。调用ID={result.Guid}"));
                    }
                    catch (Exception deserializationError)
                    {
                        // 反序列化失败保留原异常链路，调用方能直接定位数据格式问题
                        tcs.TrySetException(new OnebotV11ClientException(
                            $"服务端返回的结果反序列化失败。调用ID={result.Guid}，错误={result.ErrorMessage}，错误描述={result.ErrorMessageEx}",
                            deserializationError));
                    }
                    return;
                case 1:
                    // retcode 1：原始实现在 CallApiAsync 上视为成功，在 InvokeApiAsync 上视为"无结果"异常
                    if (hasResult)
                        tcs.TrySetException(new OnebotV11ClientException(
                            $"服务端认为任务完成成功，但没有返回结果。调用ID={result.Guid}，错误码={result.Retcode}，错误={result.ErrorMessage}，错误描述={result.ErrorMessageEx}"));
                    else
                        tcs.TrySetResult(default!);
                    return;
                default:
                    tcs.TrySetException(new OnebotV11ClientException(
                        $"返回了错误结果！调用ID={result.Guid}，错误码={result.Retcode}，错误={result.ErrorMessage}，错误描述={result.ErrorMessageEx}"));
                    return;
            }
        });

        _outbound.Writer.TryWrite(requestData is null
            ? new OnebotV11ApiRequest { Action = apiName, Guid = guid }
            : new OnebotV11ApiRequest<object> { Action = apiName, Guid = guid, Params = requestData });

        return tcs.Task;
    }
}
