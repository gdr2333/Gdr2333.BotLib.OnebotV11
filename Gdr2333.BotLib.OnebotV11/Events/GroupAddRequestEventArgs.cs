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
using Gdr2333.BotLib.OnebotV11.Events.Base;

namespace Gdr2333.BotLib.OnebotV11.Events;

/// <summary>
/// （加群请求 或 邀请加群）事件参数
/// </summary>
public class GroupAddRequestEventArgs : RequestEventArgsBase, IGroupEventArgs
{
    /// <summary>
    /// 子类型（加群请求 或 邀请加群）
    /// </summary>
    [JsonInclude, JsonRequired, JsonPropertyName("sub_type")]
    public GroupAddRequestType Subtype { get; internal set; }

    /// <summary>
    /// 群聊Id
    /// </summary>
    [JsonInclude, JsonRequired, JsonPropertyName("group_id"), JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public long GroupId { get; internal set; }

    /// <summary>
    /// 验证信息
    /// </summary>
    [JsonInclude, JsonPropertyName("comment")]
    public string? Comment { get; internal set; }
}