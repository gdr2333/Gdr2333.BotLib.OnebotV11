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
/// 事件参数基类
/// </summary>
[JsonDerivedType(typeof(GroupMessageReceivedEventArgs))]
[JsonDerivedType(typeof(PrivateMessageReceivedEventArgs))]
[JsonDerivedType(typeof(MessageReceivedEventArgsBase))]

[JsonDerivedType(typeof(LifecycleEventArgs))]
[JsonDerivedType(typeof(HeartbeatEventArgs))]
[JsonDerivedType(typeof(MetaEventArgsBase))]

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
[JsonDerivedType(typeof(NoticeEventArgsBase))]

[JsonDerivedType(typeof(NewFriendRequestEventArgs))]
[JsonDerivedType(typeof(GroupAddRequestEventArgs))]
[JsonDerivedType(typeof(RequestEventArgsBase))]
public abstract class OnebotV11EventArgsBase
{
    /// <summary>
    /// 事件发生时间
    /// </summary>
    [JsonInclude, JsonRequired, JsonPropertyName("time"), JsonConverter(typeof(UnixTimeToDateTimeConverter))]
    public DateTime Time { get; internal set; }

    /// <summary>
    /// 机器人用户ID
    /// </summary>
    [JsonInclude, JsonRequired, JsonPropertyName("self_id"), JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public long BotId { get; internal set; }

    /// <summary>
    /// 事件类型
    /// </summary>
    [JsonInclude, JsonRequired, JsonPropertyName("post_type")]
    public PostType PostType { get; internal set; }
}

/// <summary>
/// 事件类型
/// </summary>
[JsonConverter(typeof(PostTypeConverter))]
public enum PostType
{
    /// <summary>
    /// 消息事件
    /// </summary>
    Message,
    /// <summary>
    /// 通知事件
    /// </summary>
    Notice,
    /// <summary>
    /// 请求事件
    /// </summary>
    Request,
    /// <summary>
    /// 元事件
    /// </summary>
    MetaEvent
}

internal class PostTypeConverter : JsonConverter<PostType>
{
    public override PostType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
        reader.GetString()?.ToLower() switch
        {
            "message" => PostType.Message,
            "notice" => PostType.Notice,
            "request" => PostType.Request,
            "meta_event" => PostType.MetaEvent,
            _ => throw new InvalidDataException(StaticData.BadEnumValueMessage)
        };

    public override void Write(Utf8JsonWriter writer, PostType value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value switch
        {
            PostType.Message => "message",
            PostType.Notice => "notice",
            PostType.Request => "request",
            PostType.MetaEvent => "meta_event",
            _ => throw new InvalidDataException(StaticData.BadEnumValueMessage)
        });
    }
}