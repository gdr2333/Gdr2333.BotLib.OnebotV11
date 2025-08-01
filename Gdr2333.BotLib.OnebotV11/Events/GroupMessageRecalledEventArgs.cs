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

namespace Gdr2333.BotLib.OnebotV11.Events;

/// <summary>
/// 群消息撤回事件
/// </summary>
public class GroupMessageRecalledEventArgs : NoticeEventArgsBase, IUserEventArgs, IGroupEventArgs
{
    /// <summary>
    /// 所在群Id
    /// </summary>
    [JsonInclude, JsonRequired, JsonPropertyName("group_id"), JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public long GroupId { get; internal set; }

    /// <summary>
    /// 发送消息的用户Id
    /// </summary>
    [JsonInclude, JsonRequired, JsonPropertyName("user_id"), JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public long UserId { get; internal set; }

    /// <summary>
    /// 执行撤回的用户Id
    /// </summary>
    [JsonInclude, JsonRequired, JsonPropertyName("operator_id"), JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public long OperatorId { get; internal set; }

    /// <summary>
    /// 撤回的消息Id
    /// </summary>
    [JsonInclude, JsonRequired, JsonPropertyName("message_id"), JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public long MessageId { get; internal set; }
}