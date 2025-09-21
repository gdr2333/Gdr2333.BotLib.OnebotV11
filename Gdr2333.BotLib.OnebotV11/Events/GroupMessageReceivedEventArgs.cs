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

using Gdr2333.BotLib.OnebotV11.Data;
using Gdr2333.BotLib.OnebotV11.Messages;
using Gdr2333.BotLib.OnebotV11.Utils;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Gdr2333.BotLib.OnebotV11.Events;

/// <summary>
/// 群消息接收事件
/// </summary>
public class GroupMessageReceivedEventArgs : MessageReceivedEventArgsBase, IUserEventArgs, IGroupEventArgs
{
    [JsonConstructor]
#pragma warning disable CS0618
    internal GroupMessageReceivedEventArgs(GroupMessageReceivedSubType subType, long groupId, AnonymousInfo? anonymous,
                                           GroupMemberInfo? groupSender, MessageType messageType, long messageId,
                                           long userId, Message message, string rawMessage, int font) : base(messageType, messageId, userId, message, rawMessage, font)
    {
        SubType = subType;
        GroupId = groupId;
        Anonymous = anonymous;
        GroupSender = groupSender;
    }
#pragma warning restore CS0618

    /// <summary>
    /// 群消息子类型
    /// </summary>
    [JsonInclude, JsonRequired, JsonPropertyName("sub_type")]
    public GroupMessageReceivedSubType SubType { get; internal set; }

    /// <inheritdoc/>
    [Obsolete("该Sender是基类的实现，建议使用GroupSender。")]
    [JsonIgnore]
#pragma warning disable CS0809
    // 你问警告是吧？我看见了但我就这么设计的
    public override UserInfo? Sender
    {
        get => GroupSender;
        internal set => throw new InvalidOperationException();
    }
#pragma warning restore CS0809

    /// <summary>
    /// 群ID
    /// </summary>
    [JsonInclude, JsonRequired, JsonPropertyName("group_id"), JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public long GroupId { get; internal set; }

    /// <summary>
    /// 匿名信息
    /// </summary>
    [Obsolete(StaticData.AnonymousWarning)]
    [JsonInclude, JsonPropertyName("anonymous")]
    public AnonymousInfo? Anonymous { get; internal set; }

    /// <summary>
    /// 群消息发送者信息
    /// </summary>
    [JsonInclude, JsonPropertyName("sender")]
    public GroupMemberInfo? GroupSender { get; internal set; }
}

/// <summary>
/// 群聊消息子类型
/// </summary>
[JsonConverter(typeof(GroupMessageReceivedSubTypeConverter))]
public enum GroupMessageReceivedSubType
{
    /// <summary>
    /// 正常消息
    /// </summary>
    Normal,
    /// <summary>
    /// 匿名消息
    /// </summary>
    [Obsolete(StaticData.AnonymousWarning)]
    Anonymous,
    /// <summary>
    /// 通知
    /// </summary>
    Notice
};

#pragma warning disable CS0618
internal class GroupMessageReceivedSubTypeConverter : JsonConverter<GroupMessageReceivedSubType>
{
    public override GroupMessageReceivedSubType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
        reader.GetString()?.ToLower() switch
        {
            "normal" => GroupMessageReceivedSubType.Normal,
            "anonumous" => GroupMessageReceivedSubType.Anonymous,
            "notice" => GroupMessageReceivedSubType.Notice,
            _ => throw new InvalidDataException(StaticData.BadEnumValueMessage)
        };

    public override void Write(Utf8JsonWriter writer, GroupMessageReceivedSubType value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value switch
        {
            GroupMessageReceivedSubType.Normal => "normal",
            GroupMessageReceivedSubType.Notice => "notice",
            GroupMessageReceivedSubType.Anonymous => "anonumous",
            _ => throw new InvalidDataException(StaticData.BadEnumValueMessage)
        });
    }
}
#pragma warning restore CS0618