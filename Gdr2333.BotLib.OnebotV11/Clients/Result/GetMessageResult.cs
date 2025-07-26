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
using Gdr2333.BotLib.OnebotV11.Data;
using Gdr2333.BotLib.OnebotV11.Messages;
using Gdr2333.BotLib.OnebotV11.Utils;

namespace Gdr2333.BotLib.OnebotV11.Clients.Result;

/// <summary>
/// “获取消息”结果
/// </summary>
public class GetMessageResult
{
    /// <summary>
    /// 发送时间
    /// </summary>
    [JsonInclude, JsonRequired, JsonPropertyName("time"), JsonConverter(typeof(UnixTimeToDateTimeConverter))]
    public DateTime SendTime { get; internal set; }

    /// <summary>
    /// 消息类型
    /// </summary>
    [JsonInclude, JsonRequired, JsonPropertyName("message_type")]
    public MessageType MessageType { get; internal set; }

    /// <summary>
    /// 消息Id
    /// </summary>
    [JsonInclude, JsonRequired, JsonPropertyName("message_id"), JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public long MessageId { get; internal set; }

    /// <summary>
    /// 消息的“真实”Id
    /// </summary>
    [JsonInclude, JsonPropertyName("real_id"), JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public long? RealId { get; internal set; }

    /// <summary>
    /// 消息发送者信息
    /// </summary>
    [JsonInclude, JsonRequired, JsonPropertyName("sender"), JsonConverter(typeof(AutoDecisionUserInfoConverter))]
    public UserInfo Sender { get; internal set; }

    /// <summary>
    /// 消息内容
    /// </summary>
    [JsonInclude, JsonRequired, JsonPropertyName("message")]
    public Message Messsage { get; internal set; }
}