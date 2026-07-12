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
    // M5：volatile 修可见性——后台重连线程写、API 调用线程读。
    private volatile InternalUniverseClient? _client;
    private readonly Uri _target = target;
    private int _retry = 0;
    private readonly int _maxRetry = maxRetry;
    private readonly string? _accessToken = accessToken;

    // C1：把"退出"令牌和"循环"令牌拆开——
    // _exitCts 只跟整体生命周期（Dispose / 主动退出）相关；
    // _loopCts 与每个 InternalUniverseClient 一一对应；重连前显式 Cancel+Dispose，
    // 让旧客户端持有的所有 in-flight API 调用通过取消路径抛 OperationCanceledException。
    // 不再有"循环 token 永远不会触发"导致的 API 调用永久挂起。
    private CancellationTokenSource _exitCts = new();
    private CancellationTokenSource _loopCts = new();
    private readonly object _loopReplaceLock = new();
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
        if (_exitCts.IsCancellationRequested)
        {
            // 上次整体被退出过——重新创建退出 token，再清零重试计数。
            _exitCts.Dispose();
            _exitCts = new();
            _retry = 0;
        }

        // C1：进入新一轮连接前，主动取消并释放旧循环 token，确保旧的 InternalUniverseClient
        // 上的所有 in-flight 请求能正常走取消路径，而不是永久挂起。
        await StopLoopAsync();

        while (!_disposed && !_exitCts.IsCancellationRequested)
        {
            ClientWebSocket? websocket = null;
            try
            {
                websocket = new ClientWebSocket();
                if (!string.IsNullOrEmpty(_accessToken))
                    websocket.Options.SetRequestHeader("Authorization", $"Bearer {_accessToken}");
                await websocket.ConnectAsync(_target, _exitCts.Token);

                if (websocket.State != WebSocketState.Open)
                    throw new WebSocketException("WebSocket 握手后未进入 Open 状态。");

                CancellationTokenSource loopCts;
                lock (_loopReplaceLock)
                {
                    _loopCts.Dispose();
                    _loopCts = CancellationTokenSource.CreateLinkedTokenSource(_exitCts.Token);
                    loopCts = _loopCts;
                }
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
                    await Task.Delay(TimeSpan.FromMilliseconds(200 * _retry), _exitCts.Token);
                }
                catch (OperationCanceledException)
                {
                    return;
                }
            }
        }
    }

    // C1 配套：取消并 Dispose 当前持有的循环 token，让旧 InternalUniverseClient 上的循环退出。
    // 这会让 ApiRequestDispatcher 上挂着的所有请求通过取消路径抛 OperationCanceledException。
    private Task StopLoopAsync()
    {
        CancellationTokenSource? oldCts;
        InternalUniverseClient? oldClient;
        lock (_loopReplaceLock)
        {
            oldCts = _loopCts;
            oldClient = _client;
            // 立即把字段清掉，避免后续 StopAsync 内 onFail 又反查到这一个；
            // 真正的清理推迟到 await 完成，避免一次性释放双重访问。
            _loopCts = CancellationTokenSource.CreateLinkedTokenSource(_exitCts.Token);
            _client = null;
        }
        return CleanupOldAsync(oldCts, oldClient);
    }

    private static async Task CleanupOldAsync(CancellationTokenSource? cts, InternalUniverseClient? client)
    {
        if (client is not null)
        {
            try { await client.StopAsync(); }
            catch { /* 关闭 socket 失败也无所谓 */ }
        }
        if (cts is not null)
        {
            try { cts.Cancel(); } catch (ObjectDisposedException) { }
            cts.Dispose();
        }
    }

    private void FailUniverseClient(InternalUniverseClient client)
    {
        // 此方法由 InternalUniverseClient 在连接级故障时同步回调；
        // 重连在后台触发（不阻塞当前堆栈），Dispose 配合取消令牌使其安全中止。
        if (_disposed || _exitCts.IsCancellationRequested)
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
        // 先整体退出令牌，让正在跑的循环主动结束。
        try
        {
            _exitCts.Cancel();
        }
        catch (ObjectDisposedException)
        {
        }
        // 再取消+释放循环 token：让 in-flight 请求走取消路径、并清理旧 InternalUniverseClient。
        CancellationTokenSource? oldCts;
        InternalUniverseClient? oldClient;
        lock (_loopReplaceLock)
        {
            oldCts = _loopCts;
            oldClient = _client;
            _loopCts = new CancellationTokenSource();
            _client = null;
        }
        if (oldClient is not null)
        {
            // 不 await——Dispose 同步路径。StopAsync 自己处理 best-effort。
            _ = oldClient.StopAsync();
        }
        if (oldCts is not null)
        {
            try { oldCts.Cancel(); } catch (ObjectDisposedException) { }
            oldCts.Dispose();
        }
        _exitCts.Dispose();
        GC.SuppressFinalize(this);
    }
}
