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

// 你的直觉没错，是从群成员减少事件直接改的

using System.Text.Json;
using System.Text.Json.Serialization;
using Gdr2333.BotLib.OnebotV11.Events.Base;
using Gdr2333.BotLib.OnebotV11.Utils;

namespace Gdr2333.BotLib.OnebotV11.Events;

/// <summary>
/// 群成员增加事件
/// </summary>
public class GroupMemberIncreaseEventArgs : NoticeEventArgsBase, IUserEventArgs, IGroupEventArgs
{
    /// <summary>
    /// 子类型（管理员同意了/就是管理员干的）
    /// </summary>
    [JsonInclude, JsonRequired, JsonPropertyName("sub_type")]
    public GroupMemberIncreaseSubtype Subtype { get; internal set; }

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
    /// 进群的用户Id
    /// </summary>
    [JsonInclude, JsonRequired, JsonPropertyName("user_id"), JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public long UserId { get; internal set; }
}

/// <summary>
/// 群成员增加事件子类型
/// </summary>
[JsonConverter(typeof(GroupMemberIncreaseSubtypeConverter))]
public enum GroupMemberIncreaseSubtype
{
    /// <summary>
    /// 管理员同意了加群请求
    /// </summary>
    Approve,
    /// <summary>
    /// 管理员拉的人
    /// </summary>
    Invite
}

internal class GroupMemberIncreaseSubtypeConverter : JsonConverter<GroupMemberIncreaseSubtype>
{
    public override GroupMemberIncreaseSubtype Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
        reader.GetString()?.ToLower() switch
        {
            "approve" => GroupMemberIncreaseSubtype.Approve,
            "invite" => GroupMemberIncreaseSubtype.Invite,
            _ => throw new InvalidDataException(StaticData.BadEnumValueMessage)
        };

    public override void Write(Utf8JsonWriter writer, GroupMemberIncreaseSubtype value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value switch
        {
            GroupMemberIncreaseSubtype.Approve => "approve",
            GroupMemberIncreaseSubtype.Invite => "invite",
            _ => throw new InvalidDataException(StaticData.BadEnumValueMessage)
        });
    }
}