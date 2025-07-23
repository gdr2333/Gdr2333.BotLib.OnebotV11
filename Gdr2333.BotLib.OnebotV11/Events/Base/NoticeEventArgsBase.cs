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

namespace Gdr2333.BotLib.OnebotV11.Events.Base;

/// <summary>
/// 消息事件上报基类
/// </summary>
[JsonDerivedType(typeof(GroupFileUploadedEventArgs))]
[JsonDerivedType(typeof(GroupAdminChangedEventArgs))]
[JsonDerivedType(typeof(GroupMemberDecreaseEventArgs))]
[JsonDerivedType(typeof(GroupMemberIncreaseEventArgs))]
[JsonDerivedType(typeof(GroupBanStatusChangedEventArgs))]
[JsonDerivedType(typeof(FriendAddedEventArgs))]
[JsonDerivedType(typeof(GroupMessageRecalledEventArgs))]
[JsonDerivedType(typeof(FriendMessageRecalledEventArgs))]
[JsonDerivedType(typeof(GroupPokedEventArgs))]
[JsonDerivedType(typeof(GroupLuckyKingChangedEventArgs))]
[JsonDerivedType(typeof(GroupMemberHonorChangedEventArgs))]
[JsonDerivedType(typeof(NotifyEventArgsBase))]
public abstract class NoticeEventArgsBase : OnebotV11EventArgsBase
{
    /// <summary>
    /// 消息类型
    /// </summary>
    [JsonInclude, JsonRequired, JsonPropertyName("notice_type")]
    public NoticeType NoticeType { get; internal set; }
}

/// <summary>
/// 消息类型
/// </summary>
[JsonConverter(typeof(NoticeTypeConverter))]
public enum NoticeType
{
    /// <summary>
    /// 群文件被上传
    /// </summary>
    GroupFileUploaded,
    /// <summary>
    /// 群管理员变动
    /// </summary>
    GroupAdminChanged,
    /// <summary>
    /// 群成员减少
    /// </summary>
    GroupMemberDecrease,
    /// <summary>
    /// 群成员增加
    /// </summary>
    GroupMemberIncrease,
    /// <summary>
    /// 群成员禁言状态更改
    /// </summary>
    GroupBanStatusChanged,
    /// <summary>
    /// 好友增加
    /// </summary>
    FriendAdded,
    /// <summary>
    /// 群消息被撤回
    /// </summary>
    GroupMessageRecalled,
    /// <summary>
    /// 好友消息被撤回
    /// </summary>
    FriendMessageRecalled,
    /// <summary>
    /// “通知”类型，至于通知了什么自己翻Subtype去
    /// </summary>
    Notify
}

internal class NoticeTypeConverter : JsonConverter<NoticeType>
{
    public override NoticeType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
        reader.GetString()?.ToLower() switch
        {
            "group_upload" => NoticeType.GroupFileUploaded,
            "group_admin" => NoticeType.GroupAdminChanged,
            "group_decrease" => NoticeType.GroupMemberDecrease,
            "group_increase" => NoticeType.GroupMemberIncrease,
            "group_ban" => NoticeType.GroupBanStatusChanged,
            "friend_add" => NoticeType.FriendAdded,
            "group_recall" => NoticeType.GroupMessageRecalled,
            "friend_recall" => NoticeType.FriendMessageRecalled,
            "notify" => NoticeType.Notify,
            _ => throw new InvalidDataException(StaticData.BadEnumValueMessage)
        };

    public override void Write(Utf8JsonWriter writer, NoticeType value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value switch
        {
            NoticeType.GroupFileUploaded => "group_upload",
            NoticeType.GroupAdminChanged => "group_admin",
            NoticeType.GroupMemberDecrease => "group_decrease",
            NoticeType.GroupMemberIncrease => "group_increase",
            NoticeType.GroupBanStatusChanged => "group_ban",
            NoticeType.FriendAdded => "friend_add",
            NoticeType.GroupMessageRecalled => "group_recall",
            NoticeType.FriendMessageRecalled => "friend_recall",
            NoticeType.Notify => "notify",
            _ => throw new InvalidDataException(StaticData.BadEnumValueMessage)
        });
    }
}