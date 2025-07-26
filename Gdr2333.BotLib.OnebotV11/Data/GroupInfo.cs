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

namespace Gdr2333.BotLib.OnebotV11.Data;

/// <summary>
/// 群聊信息
/// </summary>
public class GroupInfo
{
    /// <summary>
    /// 群号
    /// </summary>
    [JsonInclude, JsonRequired, JsonPropertyName("group_id"), JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public long GroupId { get; internal set; }

    /// <summary>
    /// 群名
    /// </summary>
    [JsonInclude, JsonRequired, JsonPropertyName("group_name")]
    public string GroupName { get; internal set; }

    /// <summary>
    /// 当前成员数
    /// </summary>
    [JsonInclude, JsonRequired, JsonPropertyName("member_count"), JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public int MemberCount { get; internal set; }

    /// <summary>
    /// 最大成员数
    /// </summary>
    [JsonInclude, JsonRequired, JsonPropertyName("max_member_count"), JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public int MemberCapacity { get; internal set; }
}