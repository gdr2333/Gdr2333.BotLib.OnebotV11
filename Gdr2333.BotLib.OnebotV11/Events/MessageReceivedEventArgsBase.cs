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

using Gdr2333.BotLib.OnebotV11.Data;
using Gdr2333.BotLib.OnebotV11.Messages;
using System.Text.Json.Serialization;

namespace Gdr2333.BotLib.OnebotV11.Events;

/// <summary>
/// 消息接收事件基类
/// </summary>
/// <param name="messageType">消息类型</param>
/// <param name="messageId">消息ID</param>
/// <param name="userId">用户ID</param>
/// <param name="message">消息内容</param>
/// <param name="rawMessage">消息纯文本内容（由实现定义）</param>
/// <param name="font">消息所使用的字体</param>
[JsonDerivedType(typeof(GroupMessageReceivedEventArgs))]
[JsonDerivedType(typeof(PrivateMessageReceivedEventArgs))]
[method: JsonConstructor]
public abstract class MessageReceivedEventArgsBase(MessageType messageType, long messageId, long userId, Message message,
                                                   string rawMessage, int font) : OnebotV11EventArgsBase, IUserEventArgs
{
    /// <summary>
    /// 消息类型
    /// </summary>
    [JsonInclude, JsonRequired, JsonPropertyName("message_type")]
    public MessageType MessageType { get; internal set; } = messageType;

    /// <summary>
    /// 消息ID
    /// </summary>
    [JsonInclude, JsonRequired, JsonPropertyName("message_id"), JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public long MessageId { get; internal set; } = messageId;

    /// <summary>
    /// 用户ID
    /// </summary>
    [JsonInclude, JsonRequired, JsonPropertyName("user_id"), JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public long UserId { get; internal set; } = userId;

    /// <summary>
    /// 消息内容
    /// </summary>
    [JsonInclude, JsonRequired, JsonPropertyName("message")]
    public Message Message { get; internal set; } = message;

    /// <summary>
    /// 消息纯文本内容（由实现定义）
    /// </summary>
    [JsonInclude, JsonRequired, JsonPropertyName("raw_message")]
    public string RawMessage { get; internal set; } = rawMessage;

    /// <summary>
    /// 消息所使用的字体（真的有人用？）
    /// </summary>
    [JsonInclude, JsonPropertyName("font"), JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public int Font { get; private set; } = font;

    /// <summary>
    /// 消息发送者信息
    /// </summary>
    [JsonIgnore]
    public abstract UserInfo? Sender { get; internal set; }
}