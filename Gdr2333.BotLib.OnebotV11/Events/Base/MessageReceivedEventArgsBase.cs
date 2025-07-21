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
using Gdr2333.BotLib.OnebotV11.Events.Data;
using Gdr2333.BotLib.OnebotV11.Events.Interfaces;
using Gdr2333.BotLib.OnebotV11.Messages;
using Gdr2333.BotLib.OnebotV11.Utils;

namespace Gdr2333.BotLib.OnebotV11.Events.Base;

/// <summary>
/// 消息接收事件基类
/// </summary>
[JsonDerivedType(typeof(GroupMessageReceivedEventArgs))]
[JsonDerivedType(typeof(PrivateMessageReceivedEventArgs))]
public abstract class MessageReceivedEventArgsBase : OnebotV11EventArgsBase, IUserEventArgs
{
    /// <summary>
    /// 消息类型
    /// </summary>
    [JsonInclude, JsonRequired, JsonPropertyName("message_type")]
    public MessageType MessageType { get; internal set; }

    /// <summary>
    /// 消息ID
    /// </summary>
    [JsonInclude, JsonRequired, JsonPropertyName("message_id"), JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public long MessageId { get; internal set; }

    /// <summary>
    /// 用户ID
    /// </summary>
    [JsonInclude, JsonRequired, JsonPropertyName("user_id"), JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public long UserId { get; internal set; }

    /// <summary>
    /// 消息内容
    /// </summary>
    [JsonInclude, JsonRequired, JsonPropertyName("message")]
    public Message Message { get; internal set; }

    /// <summary>
    /// 消息纯文本内容（由实现定义）
    /// </summary>
    [JsonInclude, JsonRequired, JsonPropertyName("raw_message")]
    public string RawMessage { get; internal set; }

    /// <summary>
    /// 消息所使用的字体（真的有人用？）
    /// </summary>
    [JsonInclude, JsonPropertyName("font"), JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public int Font { get; private set; } = 0;

    /// <summary>
    /// 消息发送者信息
    /// </summary>
    [JsonIgnore]
    public abstract Sender? Sender { get; internal set; }
}

/// <summary>
/// 消息类型
/// </summary>
[JsonConverter(typeof(MessageTypeConverter))]
public enum MessageType
{
    /// <summary>
    /// 私聊消息
    /// </summary>
    Private,
    /// <summary>
    /// 群聊消息
    /// </summary>
    Group
}

internal class MessageTypeConverter : JsonConverter<MessageType>
{
    public override MessageType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
        reader.GetString()?.ToLower() switch
        {
            "private" => MessageType.Private,
            "group" => MessageType.Group,
            _ => throw new InvalidDataException(StaticData.BadEnumValueMessage)
        };

    public override void Write(Utf8JsonWriter writer, MessageType value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value switch
        {
            MessageType.Private => "private",
            MessageType.Group => "group",
            _ => throw new InvalidDataException(StaticData.BadEnumValueMessage)
        });
    }
}