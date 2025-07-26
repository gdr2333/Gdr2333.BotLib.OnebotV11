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
using Gdr2333.BotLib.OnebotV11.Utils;

namespace Gdr2333.BotLib.OnebotV11.Data;

/// <summary>
/// 荣誉类型
/// </summary>
[JsonConverter(typeof(GroupHonorTypeConverter))]
public enum GroupHonorType
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
    Emotion,
    /// <summary>
    /// 群聊炽焰
    /// </summary>
    Legend,
    /// <summary>
    /// 冒尖小春笋
    /// </summary>
    StrongNewbie
}

internal class GroupHonorTypeConverter : JsonConverter<GroupHonorType>
{
    public override GroupHonorType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
        reader.GetString()?.ToLower() switch
        {
            "talkative" => GroupHonorType.Talkative,
            "performer" => GroupHonorType.Performer,
            "emotion" => GroupHonorType.Emotion,
            "legend" => GroupHonorType.Legend,
            "strong_newbie" => GroupHonorType.StrongNewbie,
            _ => throw new InvalidDataException(StaticData.BadEnumValueMessage)
        };

    public override void Write(Utf8JsonWriter writer, GroupHonorType value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value switch
        {
            GroupHonorType.Talkative => "talkative",
            GroupHonorType.Performer => "performer",
            GroupHonorType.Emotion => "emotion",
            GroupHonorType.Legend => "legend",
            GroupHonorType.StrongNewbie => "strong_newbie",
            _ => throw new InvalidDataException(StaticData.BadEnumValueMessage)
        });
    }
}