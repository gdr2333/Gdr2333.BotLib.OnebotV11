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

using System.Text.Json.Serialization;
using Gdr2333.BotLib.OnebotV11.Events.Base;
using Gdr2333.BotLib.OnebotV11.Events.Data;

namespace Gdr2333.BotLib.OnebotV11.Events;

/// <summary>
/// 私聊消息接收事件
/// </summary>
public class PrivateMessageReceivedEventArgs : MessageReceivedEventArgsBase
{
    /// <inheritdoc/>
    [JsonInclude, JsonRequired, JsonPropertyName("sub_type")]
    public override string SubType { get; internal set; } = string.Empty;

    /// <inheritdoc/>
    [JsonInclude, JsonPropertyName("sender")]
    public override Sender? Sender { get; internal set; } = null;
}