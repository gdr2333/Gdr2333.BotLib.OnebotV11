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
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Gdr2333.BotLib.OnebotV11.Data;

/// <summary>
/// 加群相关请求子类型（加群请求 或 邀请加群）
/// </summary>
[JsonConverter(typeof(GroupAddRequestTypeConverter))]
public enum GroupAddRequestType
{
    /// <summary>
    /// 加群请求
    /// </summary>
    Request,
    /// <summary>
    /// 邀请加群
    /// </summary>
    Invite
}

internal class GroupAddRequestTypeConverter : JsonConverter<GroupAddRequestType>
{
    public override GroupAddRequestType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
        reader.GetString()?.ToLower() switch
        {
            "add" => GroupAddRequestType.Request,
            "invite" => GroupAddRequestType.Invite,
            _ => throw new InvalidDataException(StaticData.BadEnumValueMessage)
        };

    public override void Write(Utf8JsonWriter writer, GroupAddRequestType value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value switch
        {
            GroupAddRequestType.Request => "add",
            GroupAddRequestType.Invite => "invite",
            _ => throw new InvalidDataException(StaticData.BadEnumValueMessage)
        });
    }
}