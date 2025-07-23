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
using Gdr2333.BotLib.OnebotV11.Events.Interfaces;
using Gdr2333.BotLib.OnebotV11.Utils;

namespace Gdr2333.BotLib.OnebotV11.Events;

/// <summary>
/// 群成员荣誉变更事件参数
/// </summary>
public class GroupMemberHonorChangedEventArgs : NotifyEventArgsBase, IUserEventArgs, IGroupEventArgs
{
    /// <summary>
    /// 群Id
    /// </summary>
    [JsonInclude, JsonRequired, JsonPropertyName("group_id"), JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public long GroupId { get; internal set; }

    /// <summary>
    /// 荣誉类型
    /// </summary>
    [JsonInclude, JsonRequired, JsonPropertyName("honor_type")]
    public GroupMemberHonorType HonorType { get; internal set; }

    /// <summary>
    /// 用户Id
    /// </summary>
    [JsonInclude, JsonRequired, JsonPropertyName("user_id"), JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public long UserId { get; internal set; }
}

/// <summary>
/// 荣誉类型
/// </summary>
[JsonConverter(typeof(GroupMemberHonorTypeConverter))]
public enum GroupMemberHonorType
{
    // 我**也不知道怎么写了，以下变量名全部来源于OnebotV11标准，有事找他们去

    /// <summary>
    /// 龙王
    /// </summary>
    Talkative,
    /// <summary>
    /// 群聊之火
    /// </summary>
    Performer,
    /// <summary>
    /// 快乐源泉
    /// </summary>
    Emotion
}

internal class GroupMemberHonorTypeConverter : JsonConverter<GroupMemberHonorType>
{
    public override GroupMemberHonorType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
        reader.GetString()?.ToLower() switch
        {
            "talkative" => GroupMemberHonorType.Talkative,
            "performer" => GroupMemberHonorType.Performer,
            "emotion" => GroupMemberHonorType.Emotion,
            _ => throw new InvalidDataException(StaticData.BadEnumValueMessage)
        };

    public override void Write(Utf8JsonWriter writer, GroupMemberHonorType value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value switch
        {
            GroupMemberHonorType.Talkative => "talkative",
            GroupMemberHonorType.Performer => "performer",
            GroupMemberHonorType.Emotion => "emotion",
            _ => throw new InvalidDataException(StaticData.BadEnumValueMessage)
        });
    }
}