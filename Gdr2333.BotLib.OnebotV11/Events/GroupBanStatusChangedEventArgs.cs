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

using System.Text.Json;
using System.Text.Json.Serialization;
using Gdr2333.BotLib.OnebotV11.Events.Base;
using Gdr2333.BotLib.OnebotV11.Utils;

namespace Gdr2333.BotLib.OnebotV11.Events;

/// <summary>
/// 群成员禁言状态变更事件
/// </summary>
public class GroupBanStatusChangedEventArgs : NoticeEventArgsBase, IUserEventArgs, IGroupEventArgs
{
    /// <summary>
    /// 群成员禁言状态变更事件子类型（禁言/解除禁言）
    /// </summary>
    [JsonInclude, JsonRequired, JsonPropertyName("sub_type")]
    public GroupBanStatusChangedSubtype Subtype { get; internal set; }

    /// <summary>
    /// 群Id
    /// </summary>
    [JsonInclude, JsonRequired, JsonPropertyName("group_id"), JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public long GroupId { get; internal set; }

    /// <summary>
    /// 用户Id
    /// </summary>
    [JsonInclude, JsonRequired, JsonPropertyName("user_id"), JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public long UserId { get; internal set; }

    /// <summary>
    /// 执行禁言的用户Id
    /// </summary>
    [JsonInclude, JsonRequired, JsonPropertyName("operator_id"), JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public long OperatorId { get; internal set; }

    /// <summary>
    /// 禁言持续时长
    /// </summary>
    [JsonInclude, JsonRequired, JsonPropertyName("duration"), JsonConverter(typeof(SecondToTimeSpanConverter))]
    public TimeSpan Duration { get; internal set; }
}

/// <summary>
/// 群成员禁言状态变更事件子类型
/// </summary>
[JsonConverter(typeof(GroupBanStatusChangedSubtypeConverter))]
public enum GroupBanStatusChangedSubtype
{
    /// <summary>
    /// 禁言
    /// </summary>
    Banned,
    /// <summary>
    /// 解除禁言
    /// </summary>
    Unbanned
}

internal class GroupBanStatusChangedSubtypeConverter : JsonConverter<GroupBanStatusChangedSubtype>
{
    public override GroupBanStatusChangedSubtype Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
        reader.GetString()?.ToLower() switch
        {
            "ban" => GroupBanStatusChangedSubtype.Banned,
            "lift_ban" => GroupBanStatusChangedSubtype.Unbanned,
            _ => throw new InvalidDataException(StaticData.BadEnumValueMessage)
        };

    public override void Write(Utf8JsonWriter writer, GroupBanStatusChangedSubtype value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value switch
        {
            GroupBanStatusChangedSubtype.Banned => "ban",
            GroupBanStatusChangedSubtype.Unbanned => "lift_ban",
            _ => throw new InvalidDataException(StaticData.BadEnumValueMessage)
        });
    }
}