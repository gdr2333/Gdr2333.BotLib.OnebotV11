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

using Gdr2333.BotLib.OnebotV11.Utils;
using System.Text.Json.Serialization;

namespace Gdr2333.BotLib.OnebotV11.Data;

/// <summary>
/// 扩展的群成员信息
/// </summary>
public class GroupMemberInfoEx : GroupMemberInfo
{
    /// <summary>
    /// 加入时间
    /// </summary>
    [JsonInclude, JsonPropertyName("join_time"), JsonConverter(typeof(UnixTimeToDateTimeConverter))]
    public DateTime? JoinTime { get; internal set; }

    /// <summary>
    /// 最后发言时间
    /// </summary>
    [JsonInclude, JsonPropertyName("last_sent_time"), JsonConverter(typeof(UnixTimeToDateTimeConverter))]
    public DateTime? LastSpeakingTime { get; internal set; }

    /// <summary>
    /// 被TX封过号
    /// </summary>
    [JsonInclude, JsonPropertyName("unfriendly"), JsonConverter(typeof(OB11JsonBoolConverter))]
    public bool? IsUnfriendly { get; internal set; }

    /// <summary>
    /// 头衔过期时间
    /// </summary>
    /// <remarks>
    /// 经典问题之NTQQ里没用
    /// </remarks>
    [JsonInclude, JsonPropertyName("title_expire_time"), JsonConverter(typeof(UnixTimeToDateTimeConverter))]
    public DateTime? TitleExpires { get; internal set; }

    /// <summary>
    /// 是否可以更改这位的群聊卡片
    /// </summary>
    [JsonInclude, JsonPropertyName("card_changeable"), JsonConverter(typeof(OB11JsonBoolConverter))]
    public bool? CanChangeCard { get; internal set; }
}