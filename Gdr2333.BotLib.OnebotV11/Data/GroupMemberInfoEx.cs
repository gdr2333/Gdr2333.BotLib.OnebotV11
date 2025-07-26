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

namespace Gdr2333.BotLib.OnebotV11.Data;

public class GroupMemberInfoEx : GroupMemberInfo
{
    [JsonInclude, JsonPropertyName("join_time"), JsonConverter(typeof(UnixTimeToDateTimeConverter))]
    public DateTime? JoinTime { get; internal set; }

    [JsonInclude, JsonPropertyName("last_sent_time"), JsonConverter(typeof(UnixTimeToDateTimeConverter))]
    public DateTime? LastSpeakingTime { get; internal set; }

    [JsonInclude, JsonPropertyName("unfriendly"), JsonConverter(typeof(OB11JsonBoolConverter))]
    public bool? IsUnfriendly { get; internal set; }

    [JsonInclude, JsonPropertyName("title_expire_time"), JsonConverter(typeof(UnixTimeToDateTimeConverter))]
    public DateTime? TitleExpires { get; internal set; }

    [JsonInclude, JsonPropertyName("card_changeable"), JsonConverter(typeof(OB11JsonBoolConverter))]
    public bool? CanChangeCard { get; internal set; }
}