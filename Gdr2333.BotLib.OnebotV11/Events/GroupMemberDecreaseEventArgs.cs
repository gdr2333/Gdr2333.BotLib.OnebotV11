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
/// 群成员减少事件
/// </summary>
public class GroupMemberDecreaseEventArgs : NoticeEventArgsBase, IUserEventArgs, IGroupEventArgs
{
    /// <summary>
    /// 子类型（自愿退出/被踢/自己被踢）
    /// </summary>
    [JsonInclude, JsonRequired, JsonPropertyName("sub_type")]
    public GroupMemberDecreaseSubtype Subtype { get; internal set; }

    /// <summary>
    /// 群ID
    /// </summary>
    [JsonInclude, JsonRequired, JsonPropertyName("group_id"), JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public long GroupId { get; internal set; }

    /// <summary>
    /// 操作者Id
    /// </summary>
    [JsonInclude, JsonRequired, JsonPropertyName("operator_id"), JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public long OperatorId { get; internal set; }

    /// <summary>
    /// 退群的用户Id
    /// </summary>
    [JsonInclude, JsonRequired, JsonPropertyName("user_id"), JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public long UserId { get; internal set; }
}

/// <summary>
/// 群成员减少事件子类型
/// </summary>
[JsonConverter(typeof(GroupMemberDecreaseSubtypeConverter))]
public enum GroupMemberDecreaseSubtype
{
    /// <summary>
    /// 自愿退出
    /// </summary>
    Leave,
    /// <summary>
    /// 别人被踢
    /// </summary>
    Kick,
    /// <summary>
    /// 自己被踢
    /// </summary>
    SelfKicked
}

internal class GroupMemberDecreaseSubtypeConverter : JsonConverter<GroupMemberDecreaseSubtype>
{
    public override GroupMemberDecreaseSubtype Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
        reader.GetString()?.ToLower() switch
        {
            "leave" => GroupMemberDecreaseSubtype.Leave,
            "kick" => GroupMemberDecreaseSubtype.Kick,
            "kick_me" => GroupMemberDecreaseSubtype.SelfKicked,
            _ => throw new InvalidDataException(StaticData.BadEnumValueMessage)
        };

    public override void Write(Utf8JsonWriter writer, GroupMemberDecreaseSubtype value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value switch
        {
            GroupMemberDecreaseSubtype.Kick => "kick",
            GroupMemberDecreaseSubtype.Leave => "leave",
            GroupMemberDecreaseSubtype.SelfKicked => "kick_me",
            _ => throw new InvalidDataException(StaticData.BadEnumValueMessage)
        });
    }
}