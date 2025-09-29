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
/// 事件参数基类
/// </summary>
[JsonDerivedType(typeof(GroupMessageReceivedEventArgs))]
[JsonDerivedType(typeof(PrivateMessageReceivedEventArgs))]

[JsonDerivedType(typeof(LifecycleEventArgs))]
[JsonDerivedType(typeof(HeartbeatEventArgs))]

[JsonDerivedType(typeof(GroupFileUploadedEventArgs))]
[JsonDerivedType(typeof(GroupAdminChangedEventArgs))]
[JsonDerivedType(typeof(GroupMemberDecreaseEventArgs))]
[JsonDerivedType(typeof(GroupMemberIncreaseEventArgs))]
[JsonDerivedType(typeof(GroupBanStatusChangedEventArgs))]
[JsonDerivedType(typeof(FriendAddedEventArgs))]
[JsonDerivedType(typeof(GroupMessageRecalledEventArgs))]
[JsonDerivedType(typeof(FriendMessageRecalledEventArgs))]
[JsonDerivedType(typeof(GroupPokedEventArgs))]
[JsonDerivedType(typeof(FriendPokedEventArgs))]
[JsonDerivedType(typeof(GroupLuckyKingChangedEventArgs))]
[JsonDerivedType(typeof(GroupMemberHonorChangedEventArgs))]

[JsonDerivedType(typeof(NewFriendRequestEventArgs))]
[JsonDerivedType(typeof(GroupAddRequestEventArgs))]
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

internal class OnebotV11EventArgsConverter : JsonConverter<OnebotV11EventArgsBase>
{
    private static readonly JsonConverter<OnebotV11EventArgsBase> _defconv = (JsonConverter<OnebotV11EventArgsBase>)JsonSerializerOptions.Default.GetConverter(typeof(OnebotV11EventArgsBase));

    public override OnebotV11EventArgsBase? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var json = JsonElement.ParseValue(ref reader);
        // 对的，switch表达式堆屎山，想不到吧
        return json.GetProperty("post_type").GetString()?.ToLower() switch
        {
            "message" => json.GetProperty("message_type").GetString()?.ToLower() switch
            {
                "private" => json.Deserialize<PrivateMessageReceivedEventArgs>(options),
                "group" => json.Deserialize<GroupMessageReceivedEventArgs>(options),
                _ => throw new InvalidDataException($"未知消息事件类型{json.GetProperty("message_type").GetString()?.ToLower()}"),
            },
            "notice" => json.GetProperty("notice_type").GetString()?.ToLower() switch
            {
                "group_upload" => json.Deserialize<GroupFileUploadedEventArgs>(options),
                "group_admin" => json.Deserialize<GroupAdminChangedEventArgs>(options),
                "group_decrease" => json.Deserialize<GroupMemberDecreaseEventArgs>(options),
                "group_increase" => json.Deserialize<GroupMemberIncreaseEventArgs>(options),
                "group_ban" => json.Deserialize<GroupBanStatusChangedEventArgs>(options),
                "friend_add" => json.Deserialize<FriendAddedEventArgs>(options),
                "group_recall" => json.Deserialize<GroupMessageRecalledEventArgs>(options),
                "friend_recall" => json.Deserialize<FriendMessageRecalledEventArgs>(options),
                "notify" => (json.GetProperty("sub_type").GetString()?.ToLower()) switch
                {
                    "poke" => json.TryGetProperty("group_id", out _) ? json.Deserialize<GroupPokedEventArgs>(options) : json.Deserialize<FriendPokedEventArgs>(options),
                    "lucky_king" => json.Deserialize<GroupLuckyKingChangedEventArgs>(options),
                    "honor" => json.Deserialize<GroupMemberHonorChangedEventArgs>(options),
                    _ => throw new InvalidDataException($"未知通知：：notify事件类型{json.GetProperty("sub_type").GetString()?.ToLower()}"),
                },
                _ => throw new InvalidDataException($"未知通知事件类型{json.GetProperty("notice_type").GetString()?.ToLower()}")
            },
            "meta_event" => json.GetProperty("meta_event_type").GetString()?.ToLower() switch
            {
                "heartbeat" => json.Deserialize<HeartbeatEventArgs>(options),
                "lifecycle" => json.Deserialize<LifecycleEventArgs>(options),
                _ => throw new InvalidDataException($"未知meta事件类型{json.GetProperty("meta_event_type").GetString()?.ToLower()}")
            },
            "request" => json.GetProperty("request_type").ToString()?.ToLower() switch
            {
                "friend" => json.Deserialize<NewFriendRequestEventArgs>(options),
                "group" => json.Deserialize<GroupAddRequestEventArgs>(options),
                _ => throw new InvalidDataException($"未知请求事件类型{json.GetProperty("request_type").GetString()?.ToLower()}")
            },
            _ => throw new InvalidDataException($"未知事件类型{json.GetProperty("post_type").GetString()?.ToLower()}")
        };
    }

    public override void Write(Utf8JsonWriter writer, OnebotV11EventArgsBase value, JsonSerializerOptions options)
    {
        _defconv.Write(writer, value, options);
    }
}