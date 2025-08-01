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

using Gdr2333.BotLib.OnebotV11.Events.Base;
using Gdr2333.BotLib.OnebotV11.Utils;
using System.Net.WebSockets;
using System.Text.Json;
using static Gdr2333.BotLib.OnebotV11.Clients.OnebotV11ClientBase;

namespace Gdr2333.BotLib.OnebotV11.Clients;

internal class InternalEventClient
{
    private readonly WebSocket _eventWebSocket;

    private readonly OnebotV11ClientBase _srcClient;

    private readonly CancellationToken _cancellationToken;

    private readonly JsonSerializerOptions _opt = StaticData.GetOptions();

    public InternalEventClient(WebSocket eventWebSocket, OnebotV11ClientBase srcClient, CancellationToken cancellationToken)
    {
        _eventWebSocket = eventWebSocket;
        _srcClient = srcClient;
        _cancellationToken = cancellationToken;
    }

    /// <summary>
    /// 当接受到Onebot事件时触发的事件
    /// </summary>
    public event OnebotEventOccurrence? OnEventOccurrence;

    /// <summary>
    /// 当事件接收器出现异常时触发的事件
    /// </summary>
    public event OnExceptionInLoop? OnExceptionOccurrence;

    public async void StartEventLoop()
    {
        var buffer = new byte[40960];
        Memory<byte> bufferMem = new(buffer);
        while (!_cancellationToken.IsCancellationRequested)
        {
            try
            {
                var res = await _eventWebSocket.ReceiveAsync(bufferMem, _cancellationToken);
                var result = JsonSerializer.Deserialize<OnebotV11EventArgsBase>(buffer.AsSpan()[..res.Count], _opt);
                if (result is not null)
                    OnEventOccurrence?.Invoke(_srcClient, result);
                else
                    throw new InvalidDataException($"无法解读的事件！缓存区域Base64：{Convert.ToBase64String(buffer)}");
            }
            catch(Exception e)
            {
                OnExceptionOccurrence?.Invoke(_srcClient, e);
            }
        }
    }
}
