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
using Gdr2333.BotLib.OnebotV11.Utils;

namespace Gdr2333.BotLib.OnebotV11.Events.Base;

/// <summary>
/// 事件参数基类
/// </summary>
[JsonDerivedType(typeof(GroupMessageReceivedEventArgs))]
[JsonDerivedType(typeof(PrivateMessageReceivedEventArgs))]
[JsonDerivedType(typeof(MessageReceivedEventArgsBase))]
public abstract class OnebotV11EventArgsBase
{
    /// <summary>
    /// 事件发生时间
    /// </summary>
    [JsonInclude, JsonRequired, JsonPropertyName("time"), JsonConverter(typeof(UnixTimeToDateTimeConverter))]
    public DateTime Time { get; internal set; }

    /// <summary>
    /// 机器人用户ID
    /// </summary>
    [JsonInclude, JsonRequired, JsonPropertyName("self_id"), JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public long BotId { get; internal set; }

    /// <summary>
    /// 事件类型
    /// </summary>
    [JsonInclude, JsonRequired, JsonPropertyName("post_type")]
    public string PostType { get; internal set; }
}