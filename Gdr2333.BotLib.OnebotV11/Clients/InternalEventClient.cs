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

using Gdr2333.BotLib.OnebotV11.Events;
using Gdr2333.BotLib.OnebotV11.Utils;
using System.Net.WebSockets;
using System.Text.Json;
using static Gdr2333.BotLib.OnebotV11.Clients.OnebotV11ClientBase;

namespace Gdr2333.BotLib.OnebotV11.Clients;

internal class InternalEventClient(WebSocket eventWebSocket, OnebotV11ClientBase srcClient, CancellationToken cancellationToken, Action<InternalEventClient> onFail)
{
    private readonly WebSocket _eventWebSocket = eventWebSocket;

    private readonly OnebotV11ClientBase _srcClient = srcClient;

    private readonly CancellationToken _cancellationToken = cancellationToken;

    private readonly JsonSerializerOptions _opt = StaticData.GetOptions();

    /// <summary>
    /// 当接受到Onebot事件时触发的事件
    /// </summary>
    public event OnebotEventOccurrence? OnEventOccurrence;

    /// <summary>
    /// 当事件接收器出现异常时触发的事件
    /// </summary>
    public event OnExceptionInLoop? OnExceptionOccurrence;

    public void StartEventLoop()
    {
        EventLoop().ContinueWith((_) => OnLoopExit(EventLoop));
    }

    private void OnLoopExit(Func<Task> loop)
    {
        if (_cancellationToken.IsCancellationRequested)
            return;
        else if (_eventWebSocket.State != WebSocketState.Open)
            onFail(this);
        else
            loop().ContinueWith((_) => OnLoopExit(loop));
    }

    private async Task EventLoop()
    {
        var buffer = new byte[8192];
        var input = new List<byte>(8192);
        while (!_cancellationToken.IsCancellationRequested)
        {
            try
            {
                input.Clear();
                var res = await _eventWebSocket.ReceiveAsync(buffer, _cancellationToken);
                while (!res.EndOfMessage)
                {
                    input.AddRange(buffer[..res.Count]);
                    res = await _eventWebSocket.ReceiveAsync(buffer, _cancellationToken);
                }
                input.AddRange(buffer[..res.Count]);
                var result = JsonSerializer.Deserialize<OnebotV11EventArgsBase>(input.ToArray(), _opt);
                if (result is not null)
                    OnEventOccurrence?.Invoke(_srcClient, result);
                else
                    throw new InvalidDataException($"无法解读的事件！缓存区域Base64：{Convert.ToBase64String(input.ToArray())}");
            }
            catch (Exception e)
            {
                OnExceptionOccurrence?.Invoke(_srcClient, e);
                if (e is WebSocketException)
                    return;
            }
        }
    }
}
