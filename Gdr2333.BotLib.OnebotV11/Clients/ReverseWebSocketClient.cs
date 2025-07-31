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

using System.Net;
using System.Net.WebSockets;

namespace Gdr2333.BotLib.OnebotV11.Clients;

/// <summary>
/// 反向WebSocket客户端
/// </summary>
public class ReverseWebSocketClient : OnebotV11ClientBase
{
    /// <inheritdoc/>
    public override event OnebotEventOccurrence? OnEventOccurrence;
    /// <inheritdoc/>
    public override event OnExceptionInLoop OnExceptionOccurrence;

    private readonly List<InternalApiClient> _apiClients = [];
    private readonly ReaderWriterLockSlim _apiClientsLock = new();

    private readonly List<InternalEventClient> _eventClients = [];
    private readonly ReaderWriterLockSlim _eventClientsLock = new();

    private readonly List<InternalUniverseClient> _universeClients = [];
    private readonly ReaderWriterLockSlim _universeClientsLock = new();

    private readonly HttpListener _httpLisenter;
    private readonly CancellationTokenSource _cancellationTokenSource;
    private readonly CancellationToken _cancellationToken;
    private readonly string? _accessToken;

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
                if (content == null)
                    continue;
                if (!string.IsNullOrEmpty(_accessToken) && content.Request.Headers["Authorization"] != $"Bearer {_accessToken}")
                {
                    content.Response.StatusCode = 401;
                    content.Response.Close();
                }
                var webSocket = await content.AcceptWebSocketAsync(null);
                if (content.Request.Url.Segments[^1].StartsWith("api"))
                    ProcessApiClient(new(webSocket.WebSocket, _cancellationToken));
                else if (content.Request.Url.Segments[^1].StartsWith("event"))
                    ProcessEventClient(new(webSocket.WebSocket, this, _cancellationToken));
                else
                    ProcessUniverseClient(new(webSocket.WebSocket, _cancellationToken));
            }
            catch (Exception e)
            {
                OnExceptionOccurrence?.Invoke(this, e);
            }
        }
    }

    private void ProcessApiClient(InternalApiClient apiClient)
    {
        apiClient.OnExceptionOccurrence += (sender, e) =>
        {
            if (e is WebSocketException)
            {
                try
                {
                    _apiClientsLock.EnterWriteLock();
                    _apiClients.Remove(apiClient);
                }
                finally
                {
                    _apiClientsLock.ExitWriteLock();
                }
            }
            else
                OnExceptionOccurrence.Invoke(this, e);
        };
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

    private void ProcessEventClient(InternalEventClient eventClient)
    {
        eventClient.OnExceptionOccurrence += (sender, e) =>
        {
            if (e is WebSocketException)
            {
                try
                {
                    _eventClientsLock.EnterWriteLock();
                    _eventClients.Remove(eventClient);
                }
                finally
                {
                    _eventClientsLock.ExitWriteLock();
                }
            }
            else
                OnExceptionOccurrence.Invoke(this, e);
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

    private void ProcessUniverseClient(InternalUniverseClient universeClient)
    {
        universeClient.OnExceptionOccurrence += (sender, e) =>
        {
            if (e is WebSocketException)
            {
                try
                {
                    _universeClientsLock.EnterWriteLock();
                    _universeClients.Remove(universeClient);
                }
                finally
                {
                    _universeClientsLock.ExitWriteLock();
                }
            }
            else
                OnExceptionOccurrence.Invoke(this, e);
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

    /// <summary>
    /// 停止整个反向WebSocket服务器
    /// </summary>
    public void Stop()
    {
        _cancellationTokenSource.Cancel();
    }

    /// <inheritdoc/>
    public override Task CallApiAsync(string apiName, CancellationToken? cancellationToken = null)
    {
        List<Task> tasks = [];
        Task last = Task.CompletedTask;
        try
        {
            _apiClientsLock.EnterReadLock();
            foreach (var c in _apiClients)
                tasks.Add(c.CallApiAsync(apiName, cancellationToken));
        }
        finally
        {
            _apiClientsLock.ExitReadLock();
        }
        try
        {
            _universeClientsLock.EnterReadLock();
            foreach (var c in _universeClients)
                tasks.Add(c.CallApiAsync(apiName, cancellationToken));
        }
        finally
        {
            _universeClientsLock.ExitReadLock();
        }
        while (tasks.Count > 0)
        {
            var t = Task.WhenAny(tasks);
            t.Wait();
            if (t.IsCompletedSuccessfully)
                return t.Result;
            last = t.Result;
        }
        return last;
    }

    /// <inheritdoc/>
    public override Task CallApiAsync<TRequest>(string apiName, TRequest requestData, CancellationToken? cancellationToken = null)
    {
        List<Task> tasks = [];
        Task last = Task.CompletedTask;
        try
        {
            _apiClientsLock.EnterReadLock();
            foreach (var c in _apiClients)
                tasks.Add(c.CallApiAsync(apiName, requestData, cancellationToken));
        }
        finally
        {
            _apiClientsLock.ExitReadLock();
        }
        try
        {
            _universeClientsLock.EnterReadLock();
            foreach (var c in _universeClients)
                tasks.Add(c.CallApiAsync(apiName, requestData, cancellationToken));
        }
        finally
        {
            _universeClientsLock.ExitReadLock();
        }
        while (tasks.Count > 0)
        {
            var t = Task.WhenAny(tasks);
            t.Wait();
            if (t.IsCompletedSuccessfully)
                return t.Result;
            last = t.Result;
        }
        return last;
    }

    /// <inheritdoc/>
    public override Task<TResult> InvokeApiAsync<TResult>(string apiName, CancellationToken? cancellationToken = null)
    {
        List<Task<TResult>> tasks = [];
        Task<TResult>? last = null;
        try
        {
            _apiClientsLock.EnterReadLock();
            foreach (var c in _apiClients)
                tasks.Add(c.InvokeApiAsync<TResult>(apiName, cancellationToken));
        }
        finally
        {
            _apiClientsLock.ExitReadLock();
        }
        try
        {
            _universeClientsLock.EnterReadLock();
            foreach (var c in _universeClients)
                tasks.Add(c.InvokeApiAsync<TResult>(apiName, cancellationToken));
        }
        finally
        {
            _universeClientsLock.ExitReadLock();
        }
        while (tasks.Count > 0)
        {
            var t = Task.WhenAny(tasks);
            t.Wait();
            if (t.IsCompletedSuccessfully)
                return t.Result;
            last = t.Result;
        }
        return last ?? throw new InvalidOperationException("没有已连接的客户端！");
    }

    /// <inheritdoc/>
    public override Task<TResult> InvokeApiAsync<TRequest, TResult>(string apiName, TRequest requestData, CancellationToken? cancellationToken = null)
    {
        List<Task<TResult>> tasks = [];
        Task<TResult>? last = null;
        try
        {
            _apiClientsLock.EnterReadLock();
            foreach (var c in _apiClients)
                tasks.Add(c.InvokeApiAsync<TRequest, TResult>(apiName, requestData, cancellationToken));
        }
        finally
        {
            _apiClientsLock.ExitReadLock();
        }
        try
        {
            _universeClientsLock.EnterReadLock();
            foreach (var c in _universeClients)
                tasks.Add(c.InvokeApiAsync<TRequest, TResult>(apiName, requestData, cancellationToken));
        }
        finally
        {
            _universeClientsLock.ExitReadLock();
        }
        while (tasks.Count > 0)
        {
            var t = Task.WhenAny(tasks);
            t.Wait();
            if (t.IsCompletedSuccessfully)
                return t.Result;
            last = t.Result;
        }
        return last ?? throw new InvalidOperationException("没有已连接的客户端！");
    }
}
