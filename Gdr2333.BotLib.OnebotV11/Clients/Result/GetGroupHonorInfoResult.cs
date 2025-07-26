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

namespace Gdr2333.BotLib.OnebotV11.Clients.Result;

public class GetGroupHonorInfoResult
{
    [JsonInclude, JsonRequired, JsonPropertyName("group_id"), JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public long GroupId { get; internal set; }

    [JsonInclude, JsonPropertyName("current_talkative")]
    public TalkAtiveInfo? CurrentTalkative { get; internal set; }

    [JsonInclude, JsonPropertyName("talkative_list")]
    public TalkAtiveInfo[]? TalkativeHistory { get; internal set; }

    [JsonInclude, JsonPropertyName("performer_list")]
    public HonorInfo[]? PerformerHistory { get; internal set; }

    [JsonInclude, JsonPropertyName("legend_list")]
    public HonorInfo[]? LegendHistory { get; internal set; }

    [JsonInclude, JsonPropertyName("strong_newbie_list")]
    public HonorInfo[]? StrongNewbieHistory { get; internal set; }

    [JsonInclude, JsonPropertyName("emotion_list")]
    public HonorInfo[]? EmotionHistory { get; internal set; }
}

[JsonDerivedType(typeof(TalkAtiveInfo))]
[JsonDerivedType(typeof(HonorInfo))]
public abstract class HonorInfoBase
{
    [JsonInclude, JsonRequired, JsonPropertyName("user_id"), JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public long UserId { get; internal set; }

    [JsonInclude, JsonRequired, JsonPropertyName("nickname")]
    public string Nickname { get; internal set; }

    [JsonInclude, JsonPropertyName("avatar")]
    public Uri? AvatarUrl { get; internal set; }
}

public class TalkAtiveInfo : HonorInfoBase
{
    [JsonInclude, JsonRequired, JsonPropertyName("day_count"), JsonConverter(typeof(DayToTimeSpanConverter))]
    public TimeSpan Duration { get; internal set; }
}

public class HonorInfo : HonorInfoBase
{
    [JsonInclude, JsonRequired, JsonPropertyName("description")]
    public string Description { get; internal set; }
}