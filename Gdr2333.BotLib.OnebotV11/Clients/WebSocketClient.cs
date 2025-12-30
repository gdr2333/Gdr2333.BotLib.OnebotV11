/*
   Copyright 2025 All contributors of Gdr2333.BotLib

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

using System.Net.WebSockets;

namespace Gdr2333.BotLib.OnebotV11.Clients;

/// <summary>
/// 正向Websocket客户端
/// </summary>
/// <param name="target">目标服务器URL</param>
/// <param name="maxRetry">最大重试次数</param>
/// <param name="accessToken">访问令牌</param>
public class WebSocketClient(Uri target, int maxRetry = 5, string? accessToken = null) : OnebotV11ClientBase
{
    private InternalUniverseClient? _client;
    private readonly Uri _target = target;
    private int _retry = 0;
    private readonly int _maxRetry = maxRetry;
    private readonly string? _accessToken = accessToken;
    private CancellationTokenSource _tokenSource = new();

    /// <inheritdoc/>
    public override event OnebotEventOccurrence? OnEventOccurrence;

    /// <inheritdoc/>
    public override event OnExceptionInLoop? OnExceptionOccurrence;

    /// <summary>
    /// 启动WebSocket客户端
    /// </summary>
    /// <returns></returns>
    /// <exception cref="WebSocketException"></exception>
    public async Task LinkAsync()
    {
        if (_tokenSource.IsCancellationRequested)
            _tokenSource = new();
        var websocket = new ClientWebSocket();
        if (!string.IsNullOrEmpty(_accessToken))
            websocket.Options.SetRequestHeader("Authorization", $"Bearer {_accessToken}");
        await websocket.ConnectAsync(_target, default);
        if (websocket.State == WebSocketState.Open)
        {
            _client = new(websocket, _tokenSource.Token, async (client) =>
            {
                _retry++;
                if (_retry < _maxRetry)
                    await LinkAsync();
                else
                    throw new WebSocketException("无法连接！");
            });
            _client.OnEventOccurrence += (_, e) => OnEventOccurrence?.Invoke(this, e);
            _client.OnExceptionOccurrence += async (_, e) =>
            {
                    OnExceptionOccurrence?.Invoke(this, e);
            };
            _client.Start();
        }
        else
        {
            websocket.Dispose();
            _retry++;
            if (_retry < _maxRetry)
                await LinkAsync();
            else
                throw new WebSocketException("无法连接！");
        }
    }

    /// <inheritdoc/>
    public override Task CallApiAsync(string apiName, CancellationToken? cancellationToken = null) =>
        _client is null ? throw new InvalidOperationException($"必须先调用{nameof(LinkAsync)}才能进行API调用。") : _client.CallApiAsync(apiName, cancellationToken);

    /// <inheritdoc/>
    public override Task CallApiAsync<TRequest>(string apiName, TRequest requestData, CancellationToken? cancellationToken = null) =>
        _client is null ? throw new InvalidOperationException($"必须先调用{nameof(LinkAsync)}才能进行API调用。") : _client.CallApiAsync(apiName, requestData, cancellationToken);

    /// <inheritdoc/>
    public override Task<TResult> InvokeApiAsync<TResult>(string apiName, CancellationToken? cancellationToken = null) =>
        _client is null ? throw new InvalidOperationException($"必须先调用{nameof(LinkAsync)}才能进行API调用。") : _client.InvokeApiAsync<TResult>(apiName, cancellationToken);

    /// <inheritdoc/>
    public override Task<TResult> InvokeApiAsync<TRequest, TResult>(string apiName, TRequest requestData, CancellationToken? cancellationToken = null) =>
        _client is null ? throw new InvalidOperationException($"必须先调用{nameof(LinkAsync)}才能进行API调用。") : _client.InvokeApiAsync<TRequest, TResult>(apiName, requestData, cancellationToken);
}
