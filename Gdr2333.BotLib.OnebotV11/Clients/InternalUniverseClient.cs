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

using Gdr2333.BotLib.OnebotV11.Events;
using Gdr2333.BotLib.OnebotV11.Utils;
using System.Net.WebSockets;
using System.Text.Json;
using System.Threading;

namespace Gdr2333.BotLib.OnebotV11.Clients;

/// <summary>
/// 合一（API + 事件）WebSocket 连接的内部封装。继承 <see cref="OnebotV11ClientBase"/> 是为了复用 API 调度入口，
/// 但实际调用转发到内部的 <see cref="ApiRequestDispatcher"/>，事件由单独的 <see cref="OnEventOccurrence"/> 暴露。
/// </summary>
internal sealed class InternalUniverseClient(WebSocket universeWebSocket, CancellationToken cancellationToken, Action<InternalUniverseClient> onFail) : OnebotV11ClientBase
{
    private readonly WebSocket _universeWebSocket = universeWebSocket;
    private readonly CancellationToken _cancellationToken = cancellationToken;
    private readonly JsonSerializerOptions _opt = StaticData.GetOptions();
    private readonly ApiRequestDispatcher _dispatcher = new(cancellationToken);
    private bool _running;
    // 防止 SendLoop/ReceiveLoop 都因 WebSocketException 重复触发 onFail，
    // 否则两次 FailUniverseClient 都会进入重连流程——参见 Clients/WebSocketClient.cs 处修复说明。
    private int _failSignaled;

    /// <summary>
    /// 当接受到 Onebot 事件时触发的事件
    /// </summary>
    public override event OnebotEventOccurrence? OnEventOccurrence;

    /// <summary>
    /// 当事件接收器出现异常时触发的事件
    /// </summary>
    public override event OnExceptionInLoop? OnExceptionOccurrence;

    public void Start()
    {
        if (_running)
            return;
        _running = true;
        _ = SendLoop();
        _ = ReceiveLoop();
    }

    /// <summary>
    /// 由父 <see cref="WebSocketClient"/> 在重连前调用：关掉 socket，让 SendLoop/ReceiveLoop 通过取消路径退出，
    /// 让 <see cref="ApiRequestDispatcher"/> 上挂起的所有请求抛 <see cref="OperationCanceledException"/>。
    /// </summary>
    public async Task StopAsync()
    {
        try
        {
            if (_universeWebSocket.State == WebSocketState.Open)
                await _universeWebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "reconnect", CancellationToken.None);
        }
        catch
        {
            // 关闭失败也无所谓——目的只是取消挂起并释放资源。
        }
        // 退出时尽量主动让 onFail 兜底一次（如果 WebSocketException 没赶上）。
        if (Interlocked.Exchange(ref _failSignaled, 1) == 0)
            onFail(this);
    }

    private async Task SendLoop()
    {
        try
        {
            await foreach (var request in _dispatcher.Outbound.ReadAllAsync(_cancellationToken))
            {
                var requestBin = JsonSerializer.SerializeToUtf8Bytes(request, request.GetType(), _opt);
                await _universeWebSocket.SendAsync(requestBin, WebSocketMessageType.Text, true, _cancellationToken);
            }
        }
        catch (OperationCanceledException) { }
        catch (WebSocketException)
        {
            // 连接级故障：通知上层回收当前连接。
            // 两条循环都可能抛 WebSocketException；latch 让 onFail 只触发一次。
            if (Interlocked.Exchange(ref _failSignaled, 1) == 0)
                onFail(this);
        }
        catch (Exception e)
        {
            // 其它异常属于"单条消息"的软错误（如派发表未覆盖的未知 notice 子类型、
            // 反序列化失败等），只上报、不终结循环。连接故障由 WebSocketException 单独处理。
            OnExceptionOccurrence?.Invoke(this, e);
        }
    }

    private async Task ReceiveLoop()
    {
        var buffer = new byte[8192];
        try
        {
            while (!_cancellationToken.IsCancellationRequested)
            {
                var bytes = await InternalApiClient.ReceiveMessageAsync(buffer, _universeWebSocket, _cancellationToken);
                using var doc = JsonDocument.Parse(bytes);
                var root = doc.RootElement;
                if (root.TryGetProperty("echo", out _))
                {
                    var result = root.Deserialize<OnebotV11ApiResult>(_opt);
                    if (result is not null)
                    {
                        _dispatcher.TryDeliver(result);
                    }
                    else
                    {
                        OnExceptionOccurrence?.Invoke(this,
                            new InvalidDataException($"无法解析的API调用结果！返回原文：{Convert.ToBase64String(bytes)}"));
                    }
                }
                else
                {
                    var evt = root.Deserialize<OnebotV11EventArgsBase>(_opt);
                    if (evt is not null)
                        OnEventOccurrence?.Invoke(this, evt);
                    else
                        OnExceptionOccurrence?.Invoke(this,
                            new InvalidDataException($"无法解读的事件！缓存区域Base64：{Convert.ToBase64String(bytes)}"));
                }
            }
        }
        catch (OperationCanceledException) { }
        catch (WebSocketException)
        {
            // 连接级故障：与 SendLoop 共享 latch，确保 onFail 只触发一次。
            if (Interlocked.Exchange(ref _failSignaled, 1) == 0)
                onFail(this);
        }
        catch (Exception e)
        {
            // 其它异常属于"单条消息"的软错误，只上报、不终结循环，
            // 与 EventLoop 的容错策略保持一致。
            OnExceptionOccurrence?.Invoke(this, e);
        }
    }

    public override Task CallApiAsync(string apiName, CancellationToken? cancellationToken = null) =>
        _dispatcher.CallAsync(apiName, cancellationToken);

    public override Task CallApiAsync<TRequest>(string apiName, TRequest requestData, CancellationToken? cancellationToken = null) =>
        _dispatcher.CallAsync(apiName, requestData, cancellationToken);

    public override Task<TResult> InvokeApiAsync<TResult>(string apiName, CancellationToken? cancellationToken = null) =>
        _dispatcher.InvokeAsync<TResult>(apiName, cancellationToken);

    public override Task<TResult> InvokeApiAsync<TRequest, TResult>(string apiName, TRequest requestData, CancellationToken? cancellationToken = null) =>
        _dispatcher.InvokeAsync<TRequest, TResult>(apiName, requestData, cancellationToken);
}
