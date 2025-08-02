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

namespace Gdr2333.BotLib.OnebotV11.Events;

/// <summary>
/// 群管理员变更事件
/// </summary>
public class GroupAdminChangedEventArgs : NoticeEventArgsBase, IUserEventArgs, IGroupEventArgs
{
    /// <summary>
    /// 子类型（任命/撤销）
    /// </summary>
    [JsonInclude, JsonRequired, JsonPropertyName("sub_type")]
    public GroupAdminChangedSubtype Subtype { get; internal set; }

    /// <summary>
    /// 群Id
    /// </summary>
    [JsonInclude, JsonRequired, JsonPropertyName("group_id"), JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public long GroupId { get; internal set; }

    /// <summary>
    /// 被任命或撤销的用户Id
    /// </summary>
    [JsonInclude, JsonRequired, JsonPropertyName("user_id"), JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public long UserId { get; internal set; }
}

// 看着我用跟标准里的东西不是一个词不爽？自己fork去
/// <summary>
/// 群管理员变更事件子类型
/// </summary>
[JsonConverter(typeof(GroupAdminChangedSubtypeConverter))]
public enum GroupAdminChangedSubtype
{
    /// <summary>
    /// 任命
    /// </summary>
    Appoint,
    /// <summary>
    /// 撤销
    /// </summary>
    Dismiss
}

internal class GroupAdminChangedSubtypeConverter : JsonConverter<GroupAdminChangedSubtype>
{
    public override GroupAdminChangedSubtype Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
        reader.GetString()?.ToLower() switch
        {
            "set" => GroupAdminChangedSubtype.Appoint,
            "unset" => GroupAdminChangedSubtype.Dismiss,
            _ => throw new InvalidDataException(StaticData.BadEnumValueMessage)
        };

    public override void Write(Utf8JsonWriter writer, GroupAdminChangedSubtype value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value switch
        {
            GroupAdminChangedSubtype.Appoint => "set",
            GroupAdminChangedSubtype.Dismiss => "unset",
            _ => throw new InvalidDataException(StaticData.BadEnumValueMessage)
        });
    }
}