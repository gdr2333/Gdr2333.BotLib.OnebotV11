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
            onFail(this);
        }
        catch (Exception e)
        {
            OnExceptionOccurrence?.Invoke(null, e);
            onFail(this);
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
                var result = JsonSerializer.Deserialize<OnebotV11ApiResult>(bytes, _opt)
                    ?? throw new InvalidDataException($"无法解析的API调用结果！返回原文：{Convert.ToBase64String(bytes)}");
                if (!_dispatcher.TryDeliver(result))
                    throw new InvalidDataException($"收到未匹配的 API 响应 Guid={result.Guid}");
            }
        }
        catch (OperationCanceledException) { }
        catch (WebSocketException)
        {
            onFail(this);
        }
        catch (Exception e)
        {
            OnExceptionOccurrence?.Invoke(null, e);
            onFail(this);
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
