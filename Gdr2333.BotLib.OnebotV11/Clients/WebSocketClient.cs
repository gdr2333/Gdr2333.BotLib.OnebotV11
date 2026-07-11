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

using System.Net.WebSockets;

namespace Gdr2333.BotLib.OnebotV11.Clients;

/// <summary>
/// 正向Websocket客户端
/// </summary>
/// <param name="target">目标服务器URL</param>
/// <param name="maxRetry">最大重试次数</param>
/// <param name="accessToken">访问令牌</param>
public sealed class WebSocketClient(Uri target, int maxRetry = 5, string? accessToken = null) : OnebotV11ClientBase, IDisposable
{
    private InternalUniverseClient? _client;
    private readonly Uri _target = target;
    private int _retry = 0;
    private readonly int _maxRetry = maxRetry;
    private readonly string? _accessToken = accessToken;
    private CancellationTokenSource _tokenSource = new();
    private bool _disposed;

    /// <inheritdoc/>
    public override event OnebotEventOccurrence? OnEventOccurrence;

    /// <inheritdoc/>
    public override event OnExceptionInLoop? OnExceptionOccurrence;

    /// <summary>
    /// 启动WebSocket客户端
    /// </summary>
    /// <returns></returns>
    /// <exception cref="WebSocketException"></exception>
    /// <exception cref="ObjectDisposedException">客户端已被释放</exception>
    public async Task LinkAsync()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        if (_tokenSource.IsCancellationRequested)
        {
            _tokenSource.Dispose();
            _tokenSource = new();
            _retry = 0;
        }

        while (!_disposed && !_tokenSource.IsCancellationRequested)
        {
            ClientWebSocket? websocket = null;
            try
            {
                websocket = new ClientWebSocket();
                if (!string.IsNullOrEmpty(_accessToken))
                    websocket.Options.SetRequestHeader("Authorization", $"Bearer {_accessToken}");
                await websocket.ConnectAsync(_target, _tokenSource.Token);

                if (websocket.State != WebSocketState.Open)
                    throw new WebSocketException("WebSocket 握手后未进入 Open 状态。");

                var loopCts = CancellationTokenSource.CreateLinkedTokenSource(_tokenSource.Token);
                var client = new InternalUniverseClient(websocket, loopCts.Token, FailUniverseClient);
                client.OnEventOccurrence += (_, e) => OnEventOccurrence?.Invoke(this, e);
                client.OnExceptionOccurrence += (_, e) => OnExceptionOccurrence?.Invoke(this, e);
                websocket = null; // ownership transferred to client
                _client = client;
                client.Start();
                return; // 成功建立连接，等待后续 onFail 触发重连
            }
            catch (OperationCanceledException)
            {
                websocket?.Dispose();
                return;
            }
            catch (Exception e)
            {
                websocket?.Dispose();
                _client = null;
                OnExceptionOccurrence?.Invoke(this, e);
                _retry++;
                if (_retry >= _maxRetry)
                    throw new WebSocketException("无法连接！", e);
                // 简单线性退避，避免失败时打满 CPU
                try
                {
                    await Task.Delay(TimeSpan.FromMilliseconds(200 * _retry), _tokenSource.Token);
                }
                catch (OperationCanceledException)
                {
                    return;
                }
            }
        }
    }

    private void FailUniverseClient(InternalUniverseClient client)
    {
        // 此方法由 InternalUniverseClient 在连接级故障时同步回调；
        // 重连在后台触发（不阻塞当前堆栈），Dispose 配合取消令牌使其安全中止。
        if (_disposed || _tokenSource.IsCancellationRequested)
            return;
        _ = RetryAsync();

        async Task RetryAsync()
        {
            try
            {
                _retry = 0;
                await LinkAsync();
            }
            catch (Exception e)
            {
                OnExceptionOccurrence?.Invoke(this, e);
            }
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

    /// <summary>
    /// 释放客户端持有的全部非托管资源（连接、取消令牌源等）。释放后客户端不可再用。
    /// </summary>
    public void Dispose()
    {
        if (_disposed)
            return;
        _disposed = true;
        try
        {
            _tokenSource.Cancel();
        }
        catch (ObjectDisposedException)
        {
        }
        _tokenSource.Dispose();
        GC.SuppressFinalize(this);
    }
}
