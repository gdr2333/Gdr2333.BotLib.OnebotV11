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

using System.Net;

namespace Gdr2333.BotLib.OnebotV11.Clients;

/// <summary>
/// 反向WebSocket客户端
/// </summary>
public sealed class ReverseWebSocketClient : OnebotV11ClientBase, IDisposable
{
    /// <inheritdoc/>
    public override event OnebotEventOccurrence? OnEventOccurrence;
    /// <inheritdoc/>
    public override event OnExceptionInLoop? OnExceptionOccurrence;

    private readonly List<InternalApiClient> _apiClients = [];
    private readonly ReaderWriterLockSlim _apiClientsLock = new();

    private readonly List<InternalEventClient> _eventClients = [];
    private readonly ReaderWriterLockSlim _eventClientsLock = new();

    private readonly List<InternalUniverseClient> _universeClients = [];
    private readonly ReaderWriterLockSlim _universeClientsLock = new();

    private readonly Dictionary<object, CancellationTokenSource> _subTaskCTS = [];

    private readonly HttpListener _httpLisenter;
    private readonly CancellationTokenSource _cancellationTokenSource;
    private readonly CancellationToken _cancellationToken;
    private readonly string? _accessToken;
    private bool _disposed;

    /// <summary>
    /// 新建一个反向Websocket服务器
    /// </summary>
    /// <param name="target">目标地址</param>
    /// <param name="accessToken">（可选）访问密钥</param>
    public ReverseWebSocketClient(Uri target, string? accessToken = null)
    {
        _httpLisenter = new HttpListener();
        _httpLisenter.Prefixes.Add(target.AbsoluteUri);
        _httpLisenter.Prefixes.Add(target.AbsoluteUri + "api/");
        _httpLisenter.Prefixes.Add(target.AbsoluteUri + "event/");
        _cancellationTokenSource = new CancellationTokenSource();
        _cancellationToken = _cancellationTokenSource.Token;
        _accessToken = accessToken;
    }

    /// <summary>
    /// 启动服务器
    /// </summary>
    public void Start()
    {
        _httpLisenter.Start();
        HttpLoop();
    }

    private async void HttpLoop()
    {
        while (!_cancellationToken.IsCancellationRequested)
        {
            try
            {
                var content = await _httpLisenter.GetContextAsync();
                if (content is null)
                    continue;
                if (!string.IsNullOrEmpty(_accessToken) && content.Request.Headers["Authorization"] != $"Bearer {_accessToken}")
                {
                    content.Response.StatusCode = 401;
                    content.Response.Close();
                }
                var webSocket = await content.AcceptWebSocketAsync(null);
                CancellationTokenSource cts = new();
                if (content.Request.Url is null)
                {
                    var client = new InternalUniverseClient(webSocket.WebSocket, cts.Token, FailUniverseClient);
                    lock (_subTaskCTS)
                        _subTaskCTS.Add(client, cts);
                    // 那我们就当是universe好了，反正错不了，大概
                    ProcessUniverseClient(client);
                }
                else if (content.Request.Url.Segments[^1].StartsWith("api"))
                {
                    var client = new InternalApiClient(webSocket.WebSocket, cts.Token, FailApiClient);
                    lock (_subTaskCTS)
                        _subTaskCTS.Add(client, cts);
                    ProcessApiClient(client);
                }
                else if (content.Request.Url.Segments[^1].StartsWith("event"))
                {
                    var client = new InternalEventClient(webSocket.WebSocket, this, cts.Token, FailEventClient);
                    lock (_subTaskCTS)
                        _subTaskCTS.Add(client, cts);
                    ProcessEventClient(client);
                }
                else
                {
                    var client = new InternalUniverseClient(webSocket.WebSocket, cts.Token, FailUniverseClient);
                    lock (_subTaskCTS)
                        _subTaskCTS.Add(client, cts);
                    ProcessUniverseClient(client);
                }
            }
            catch (Exception e)
            {
                OnExceptionOccurrence?.Invoke(this, e);
            }
        }
    }

    private void ProcessApiClient(InternalApiClient apiClient)
    {
        apiClient.OnExceptionOccurrence += (sender, e) => OnExceptionOccurrence?.Invoke(this, e);
        try
        {
            _apiClientsLock.EnterWriteLock();
            _apiClients.Add(apiClient);
        }
        finally
        {
            _apiClientsLock.ExitWriteLock();
        }
        apiClient.Start();
    }

    private void FailApiClient(InternalApiClient apiClient)
    {
        try
        {
            _apiClientsLock.EnterWriteLock();
            _apiClients.Remove(apiClient);
            lock (_subTaskCTS)
            {
                _subTaskCTS[apiClient].Cancel();
                _subTaskCTS.Remove(apiClient);
            }
        }
        finally
        {
            _apiClientsLock.ExitWriteLock();
        }
    }

    private void ProcessEventClient(InternalEventClient eventClient)
    {
        eventClient.OnExceptionOccurrence += (sender, e) =>
        {
            OnExceptionOccurrence?.Invoke(this, e);
        };
        eventClient.OnEventOccurrence += (sender, e) =>
        {
            OnEventOccurrence?.Invoke(this, e);
        };
        try
        {
            _eventClientsLock.EnterWriteLock();
            _eventClients.Add(eventClient);
        }
        finally
        {
            _eventClientsLock.ExitWriteLock();
        }
        eventClient.StartEventLoop();
    }


    private void FailEventClient(InternalEventClient eventClient)
    {
        try
        {
            _eventClientsLock.EnterWriteLock();
            _eventClients.Remove(eventClient);
            lock (_subTaskCTS)
            {
                _subTaskCTS[eventClient].Cancel();
                _subTaskCTS.Remove(eventClient);
            }
        }
        finally
        {
            _eventClientsLock.ExitWriteLock();
        }
    }

    private void ProcessUniverseClient(InternalUniverseClient universeClient)
    {
        universeClient.OnExceptionOccurrence += (sender, e) =>
        {
            OnExceptionOccurrence?.Invoke(this, e);
        };
        universeClient.OnEventOccurrence += (sender, e) =>
        {
            OnEventOccurrence?.Invoke(sender, e);
        };
        try
        {
            _universeClientsLock.EnterWriteLock();
            _universeClients.Add(universeClient);
        }
        finally
        {
            _universeClientsLock.ExitWriteLock();
        }
        universeClient.Start();
    }

    private void FailUniverseClient(InternalUniverseClient universeClient)
    {
        try
        {
            _universeClientsLock.EnterWriteLock();
            _universeClients.Remove(universeClient);
            lock (_subTaskCTS)
            {
                _subTaskCTS[universeClient].Cancel();
                _subTaskCTS.Remove(universeClient);
            }
        }
        finally
        {
            _universeClientsLock.ExitWriteLock();
        }
    }

    /// <summary>
    /// 停止整个反向WebSocket服务器
    /// </summary>
    public void Stop()
    {
        _cancellationTokenSource.Cancel();
        lock (_subTaskCTS)
            foreach (var i in _subTaskCTS)
                i.Value.Cancel();
    }

    /// <summary>
    /// 释放反向WebSocket服务器持有的全部非托管资源（HTTP监听、取消令牌源、每个连接的取消令牌源等）。释放后不可再 <see cref="Start"/>。
    /// </summary>
    public void Dispose()
    {
        if (_disposed)
            return;
        _disposed = true;
        try
        {
            if (_httpLisenter.IsListening)
                _httpLisenter.Stop();
        }
        catch (ObjectDisposedException)
        {
        }
        try
        {
            _cancellationTokenSource.Cancel();
        }
        catch (ObjectDisposedException)
        {
        }
        lock (_subTaskCTS)
        {
            foreach (var cts in _subTaskCTS.Values)
            {
                try
                {
                    cts.Cancel();
                }
                catch (ObjectDisposedException)
                {
                }
                cts.Dispose();
            }
            _subTaskCTS.Clear();
        }
        _httpLisenter.Close();
        _cancellationTokenSource.Dispose();
        GC.SuppressFinalize(this);
    }

    /// <inheritdoc/>
    public override async Task CallApiAsync(string apiName, CancellationToken? cancellationToken = null)
    {
        List<Task> tasks = [];
        List<Task> apiTasks = [];
        try
        {
            _apiClientsLock.EnterReadLock();
            foreach (var c in _apiClients)
                apiTasks.Add(c.CallApiAsync(apiName, cancellationToken));
        }
        finally
        {
            _apiClientsLock.ExitReadLock();
        }
        List<Task> universeTasks = [];
        try
        {
            _universeClientsLock.EnterReadLock();
            foreach (var c in _universeClients)
                universeTasks.Add(c.CallApiAsync(apiName, cancellationToken));
        }
        finally
        {
            _universeClientsLock.ExitReadLock();
        }
        if (apiTasks.Count == 0 && universeTasks.Count == 0)
            throw new InvalidOperationException("没有已连接的客户端！");
        tasks.AddRange(apiTasks);
        tasks.AddRange(universeTasks);
        var exceptions = new List<Exception>();
        while (tasks.Count > 0)
        {
            var t = await Task.WhenAny(tasks);
            if (t.IsCompletedSuccessfully)
                return;
            if (t.Exception is not null)
                exceptions.Add(t.Exception);
            tasks.Remove(t);
        }
        // 全部失败时抛出聚合异常（保留 innerException）
        if (exceptions.Count > 0)
            throw new AggregateException("所有 OneBot 客户端均未能完成调用。", exceptions);
    }

    /// <inheritdoc/>
    public override async Task CallApiAsync<TRequest>(string apiName, TRequest requestData, CancellationToken? cancellationToken = null)
    {
        List<Task> tasks = [];
        List<Task> apiTasks = [];
        try
        {
            _apiClientsLock.EnterReadLock();
            foreach (var c in _apiClients)
                apiTasks.Add(c.CallApiAsync(apiName, requestData, cancellationToken));
        }
        finally
        {
            _apiClientsLock.ExitReadLock();
        }
        List<Task> universeTasks = [];
        try
        {
            _universeClientsLock.EnterReadLock();
            foreach (var c in _universeClients)
                universeTasks.Add(c.CallApiAsync(apiName, requestData, cancellationToken));
        }
        finally
        {
            _universeClientsLock.ExitReadLock();
        }
        if (apiTasks.Count == 0 && universeTasks.Count == 0)
            throw new InvalidOperationException("没有已连接的客户端！");
        tasks.AddRange(apiTasks);
        tasks.AddRange(universeTasks);
        var exceptions = new List<Exception>();
        while (tasks.Count > 0)
        {
            var t = await Task.WhenAny(tasks);
            if (t.IsCompletedSuccessfully)
                return;
            if (t.Exception is not null)
                exceptions.Add(t.Exception);
            tasks.Remove(t);
        }
        if (exceptions.Count > 0)
            throw new AggregateException("所有 OneBot 客户端均未能完成调用。", exceptions);
    }

    /// <inheritdoc/>
    public override async Task<TResult> InvokeApiAsync<TResult>(string apiName, CancellationToken? cancellationToken = null)
    {
        List<Task<TResult>> tasks = [];
        List<Task<TResult>> apiTasks = [];
        try
        {
            _apiClientsLock.EnterReadLock();
            foreach (var c in _apiClients)
                apiTasks.Add(c.InvokeApiAsync<TResult>(apiName, cancellationToken));
        }
        finally
        {
            _apiClientsLock.ExitReadLock();
        }
        List<Task<TResult>> universeTasks = [];
        try
        {
            _universeClientsLock.EnterReadLock();
            foreach (var c in _universeClients)
                universeTasks.Add(c.InvokeApiAsync<TResult>(apiName, cancellationToken));
        }
        finally
        {
            _universeClientsLock.ExitReadLock();
        }
        if (apiTasks.Count == 0 && universeTasks.Count == 0)
            throw new InvalidOperationException("没有已连接的客户端！");
        tasks.AddRange(apiTasks);
        tasks.AddRange(universeTasks);
        var exceptions = new List<Exception>();
        while (tasks.Count > 0)
        {
            var t = await Task.WhenAny(tasks);
            if (t.IsCompletedSuccessfully)
                return await t;
            if (t.Exception is not null)
                exceptions.Add(t.Exception);
            tasks.Remove(t);
        }
        if (exceptions.Count > 0)
            throw new AggregateException("所有 OneBot 客户端均未能完成调用。", exceptions);
        throw new InvalidOperationException("没有已连接的客户端！");
    }

    /// <inheritdoc/>
    public override async Task<TResult> InvokeApiAsync<TRequest, TResult>(string apiName, TRequest requestData, CancellationToken? cancellationToken = null)
    {
        List<Task<TResult>> tasks = [];
        List<Task<TResult>> apiTasks = [];
        try
        {
            _apiClientsLock.EnterReadLock();
            foreach (var c in _apiClients)
                apiTasks.Add(c.InvokeApiAsync<TRequest, TResult>(apiName, requestData, cancellationToken));
        }
        finally
        {
            _apiClientsLock.ExitReadLock();
        }
        List<Task<TResult>> universeTasks = [];
        try
        {
            _universeClientsLock.EnterReadLock();
            foreach (var c in _universeClients)
                universeTasks.Add(c.InvokeApiAsync<TRequest, TResult>(apiName, requestData, cancellationToken));
        }
        finally
        {
            _universeClientsLock.ExitReadLock();
        }
        if (apiTasks.Count == 0 && universeTasks.Count == 0)
            throw new InvalidOperationException("没有已连接的客户端！");
        tasks.AddRange(apiTasks);
        tasks.AddRange(universeTasks);
        var exceptions = new List<Exception>();
        while (tasks.Count > 0)
        {
            var t = await Task.WhenAny(tasks);
            if (t.IsCompletedSuccessfully)
                return await t;
            if (t.Exception is not null)
                exceptions.Add(t.Exception);
            tasks.Remove(t);
        }
        if (exceptions.Count > 0)
            throw new AggregateException("所有 OneBot 客户端均未能完成调用。", exceptions);
        throw new InvalidOperationException("没有已连接的客户端！");
    }
}
