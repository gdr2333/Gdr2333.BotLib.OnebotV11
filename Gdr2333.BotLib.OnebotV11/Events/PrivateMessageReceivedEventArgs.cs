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

using System.Text.Json;
using System.Text.Json.Serialization;
using Gdr2333.BotLib.OnebotV11.Events.Base;
using Gdr2333.BotLib.OnebotV11.Data;
using Gdr2333.BotLib.OnebotV11.Events.Interfaces;
using Gdr2333.BotLib.OnebotV11.Utils;

namespace Gdr2333.BotLib.OnebotV11.Events;

/// <summary>
/// 私聊消息接收事件
/// </summary>
public class PrivateMessageReceivedEventArgs : MessageReceivedEventArgsBase, IUserEventArgs
{
    /// <summary>
    /// 私聊消息子类型
    /// </summary>
    [JsonInclude, JsonRequired, JsonPropertyName("sub_type")]
    public PrivateMessageReceivedSubType SubType { get; internal set; }

    /// <inheritdoc/>
    [JsonInclude, JsonPropertyName("sender")]
    public override UserInfo? Sender { get; internal set; } = null;
}

/// <summary>
/// 私聊消息子类型
/// </summary>
[JsonConverter(typeof(PrivateMessageReceivedSubTypeConverter))]
public enum PrivateMessageReceivedSubType
{
    /// <summary>
    /// 好友发的消息
    /// </summary>
    Friend,
    /// <summary>
    /// 群临时会话消息
    /// </summary>
    Group,
    /// <summary>
    /// 其他
    /// </summary>
    Other
}

internal class PrivateMessageReceivedSubTypeConverter : JsonConverter<PrivateMessageReceivedSubType>
{
    public override PrivateMessageReceivedSubType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
        reader.GetString()?.ToLower() switch
        {
            "friend" => PrivateMessageReceivedSubType.Friend,
            "group" => PrivateMessageReceivedSubType.Group,
            _ => PrivateMessageReceivedSubType.Other
        };

    public override void Write(Utf8JsonWriter writer, PrivateMessageReceivedSubType value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value switch
        {
            PrivateMessageReceivedSubType.Friend => "friend",
            PrivateMessageReceivedSubType.Group => "group",
            PrivateMessageReceivedSubType.Other => "other",
            _ => throw new InvalidDataException(StaticData.BadEnumValueMessage)
        });
    }
}