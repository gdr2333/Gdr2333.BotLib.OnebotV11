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
/// “获取群荣耀信息”结果
/// </summary>
public class GetGroupHonorInfoResult
{
    /// <summary>
    /// 群Id
    /// </summary>
    [JsonInclude, JsonRequired, JsonPropertyName("group_id"), JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public long GroupId { get; internal set; }

    /// <summary>
    /// 当前龙王
    /// </summary>
    [JsonInclude, JsonPropertyName("current_talkative")]
    public TalkAtiveInfo? CurrentTalkative { get; internal set; }

    /// <summary>
    /// 历史龙王列表
    /// </summary>
    [JsonInclude, JsonPropertyName("talkative_list")]
    public TalkAtiveInfo[]? TalkativeHistory { get; internal set; }

    /// <summary>
    /// 历史“群聊之火”列表
    /// </summary>
    [JsonInclude, JsonPropertyName("performer_list")]
    public HonorInfo[]? PerformerHistory { get; internal set; }

    /// <summary>
    /// 历史“群聊炽焰”列表
    /// </summary>
    [JsonInclude, JsonPropertyName("legend_list")]
    public HonorInfo[]? LegendHistory { get; internal set; }

    /// <summary>
    /// 历史“冒尖小春笋”列表
    /// </summary>
    [JsonInclude, JsonPropertyName("strong_newbie_list")]
    public HonorInfo[]? StrongNewbieHistory { get; internal set; }

    /// <summary>
    /// 历史“快乐之源”列表
    /// </summary>
    [JsonInclude, JsonPropertyName("emotion_list")]
    public HonorInfo[]? EmotionHistory { get; internal set; }
}

/// <summary>
/// 群荣耀信息基类
/// </summary>
[JsonDerivedType(typeof(TalkAtiveInfo))]
[JsonDerivedType(typeof(HonorInfo))]
[method: JsonConstructor]
public abstract class HonorInfoBase(long userId, string nickname, Uri? avatarUrl)
{
    /// <summary>
    /// 获得该荣耀称号的用户Id
    /// </summary>
    [JsonInclude, JsonRequired, JsonPropertyName("user_id"), JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public long UserId { get; internal set; } = userId;

    /// <summary>
    /// 获得该荣耀称号的用户昵称
    /// </summary>
    [JsonInclude, JsonRequired, JsonPropertyName("nickname")]
    public string Nickname { get; internal set; } = nickname;

    /// <summary>
    /// 获得该荣耀称号的用户头像URL
    /// </summary>
    [JsonInclude, JsonPropertyName("avatar")]
    public Uri? AvatarUrl { get; internal set; } = avatarUrl;
}

/// <summary>
/// 龙王信息
/// </summary>
public class TalkAtiveInfo : HonorInfoBase
{
    [JsonConstructor]
    internal TalkAtiveInfo(TimeSpan duration, long userId, string nickname, Uri? avatarUrl) : base(userId, nickname, avatarUrl)    {
        Duration = duration;
    }

    /// <summary>
    /// 当上龙王的总时长
    /// </summary>
    [JsonInclude, JsonRequired, JsonPropertyName("day_count"), JsonConverter(typeof(DayToTimeSpanConverter))]
    public TimeSpan Duration { get; internal set; }
}

/// <summary>
/// 其他荣耀称号信息
/// </summary>
public class HonorInfo : HonorInfoBase
{
    [JsonConstructor]
    internal HonorInfo(string description, long userId, string nickname, Uri? avatarUrl) : base(userId, nickname, avatarUrl)    {
        Description = description;
    }

    /// <summary>
    /// 称号描述
    /// </summary>
    [JsonInclude, JsonRequired, JsonPropertyName("description")]
    public string Description { get; internal set; }
}