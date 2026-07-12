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

using Gdr2333.BotLib.OnebotV11.Utils;
using System.Net.WebSockets;
using System.Text.Json;
using System.Threading;

namespace Gdr2333.BotLib.OnebotV11.Clients;

/// <summary>
/// 单条 API WebSocket 连接的内部封装（反向 WebSocket 中的 /api 端点）。
/// </summary>
internal sealed class InternalApiClient(WebSocket apiWebSocket, CancellationToken cancellationToken, Action<InternalApiClient> onFail)
{
    private readonly WebSocket _apiWebSocket = apiWebSocket;
    private readonly CancellationToken _cancellationToken = cancellationToken;
    private readonly JsonSerializerOptions _opt = StaticData.GetOptions();
    private readonly ApiRequestDispatcher _dispatcher = new(cancellationToken);
    private bool _running;
    // SendLoop 与 ReceiveLoop 都会把 WebSocketException 转给 onFail——
    // 共享 latch 才能保证 FailApiClient 只跑一次，否则会 KeyNotFoundException。
    private int _failSignaled;

    /// <summary>
    /// 当事件接收器出现异常时触发的事件
    /// </summary>
    public event EventHandler<Exception>? OnExceptionOccurrence;

    public void Start()
    {
        if (_running)
            return;
        _running = true;
        _ = SendLoop();
        _ = ReceiveLoop();
    }

    private async Task SendLoop()
    {
        try
        {
            await foreach (var request in _dispatcher.Outbound.ReadAllAsync(_cancellationToken))
            {
                var requestBin = JsonSerializer.SerializeToUtf8Bytes(request, request.GetType(), _opt);
                await _apiWebSocket.SendAsync(requestBin, WebSocketMessageType.Text, true, _cancellationToken);
            }
        }
        catch (OperationCanceledException) { }
        catch (WebSocketException)
        {
            // 连接级故障：与 ReceiveLoop 共享 latch。
            if (Interlocked.Exchange(ref _failSignaled, 1) == 0)
                onFail(this);
        }
        catch (Exception e)
        {
            // 其它异常属"单条请求"软错误（序列化失败、构造期 bug 等），
            // 只上报、不终结循环；连接级故障由 WebSocketException 单独处理。
            OnExceptionOccurrence?.Invoke(null, e);
        }
    }

    private async Task ReceiveLoop()
    {
        var buffer = new byte[8192];
        try
        {
            while (!_cancellationToken.IsCancellationRequested)
            {
                var bytes = await ReceiveMessageAsync(buffer, _apiWebSocket, _cancellationToken);
                var result = JsonSerializer.Deserialize<OnebotV11ApiResult>(bytes, _opt);
                if (result is null)
                {
                    OnExceptionOccurrence?.Invoke(null,
                        new InvalidDataException($"无法解析的API调用结果！返回原文：{Convert.ToBase64String(bytes)}"));
                    continue;
                }
                if (!_dispatcher.TryDeliver(result))
                {
                    // 没有匹配的 guid 通常意味着"服务端在错误回复之前的请求"，
                    // 属单条软错误，连接本身仍然健康——继续。
                    OnExceptionOccurrence?.Invoke(null,
                        new InvalidDataException($"收到未匹配的 API 响应 Guid={result.Guid}"));
                }
            }
        }
        catch (OperationCanceledException) { }
        catch (WebSocketException)
        {
            // 连接级故障：与 SendLoop 共享 latch。
            if (Interlocked.Exchange(ref _failSignaled, 1) == 0)
                onFail(this);
        }
        catch (Exception e)
        {
            // 其它异常属"单条响应"软错误，只上报、不终结循环，
            // 与 EventLoop / UniverseClient 策略保持一致。
            OnExceptionOccurrence?.Invoke(null, e);
        }
    }

    internal static async Task<byte[]> ReceiveMessageAsync(byte[] buffer, WebSocket socket, CancellationToken token)
    {
        using var ms = new MemoryStream();
        while (true)
        {
            var res = await socket.ReceiveAsync(buffer, token);
            ms.Write(buffer, 0, res.Count);
            if (res.EndOfMessage)
                return ms.ToArray();
        }
    }

    public Task CallApiAsync(string apiName, CancellationToken? cancellationToken = null) =>
        _dispatcher.CallAsync(apiName, cancellationToken);

    public Task CallApiAsync<TRequest>(string apiName, TRequest requestData, CancellationToken? cancellationToken = null) =>
        _dispatcher.CallAsync(apiName, requestData, cancellationToken);

    public Task<TResult> InvokeApiAsync<TResult>(string apiName, CancellationToken? cancellationToken = null) =>
        _dispatcher.InvokeAsync<TResult>(apiName, cancellationToken);

    public Task<TResult> InvokeApiAsync<TRequest, TResult>(string apiName, TRequest requestData, CancellationToken? cancellationToken = null) =>
        _dispatcher.InvokeAsync<TRequest, TResult>(apiName, requestData, cancellationToken);
}
