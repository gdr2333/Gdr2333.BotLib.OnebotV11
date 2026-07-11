/*
   Copyright 2025-2026 All contributors of Gdr2333.BotLib

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

namespace Gdr2333.BotLib.OnebotV11.Events;

/// <summary>
/// OneBot v11 事件反序列化的派发表（dispatch table）。
/// <para>把 <c>post_type</c> + 子类型字段（如 <c>message_type</c>、<c>notice_type</c>、<c>sub_type</c>、<c>meta_event_type</c>、<c>request_type</c>）路由到具体的事件参数类型。</para>
/// </summary>
internal static class OnebotV11EventDispatch
{
    private static readonly Dictionary<string, Func<JsonElement, JsonSerializerOptions, OnebotV11EventArgsBase?>> _messageDispatch = new(StringComparer.Ordinal)
    {
        ["private"] = (json, opt) => json.Deserialize<PrivateMessageReceivedEventArgs>(opt),
        ["group"] = (json, opt) => json.Deserialize<GroupMessageReceivedEventArgs>(opt),
    };

    private static readonly Dictionary<string, Func<JsonElement, JsonSerializerOptions, OnebotV11EventArgsBase?>> _noticeDispatch = new(StringComparer.Ordinal)
    {
        ["group_upload"] = (json, opt) => json.Deserialize<GroupFileUploadedEventArgs>(opt),
        ["group_admin"] = (json, opt) => json.Deserialize<GroupAdminChangedEventArgs>(opt),
        ["group_decrease"] = (json, opt) => json.Deserialize<GroupMemberDecreaseEventArgs>(opt),
        ["group_increase"] = (json, opt) => json.Deserialize<GroupMemberIncreaseEventArgs>(opt),
        ["group_ban"] = (json, opt) => json.Deserialize<GroupBanStatusChangedEventArgs>(opt),
        ["friend_add"] = (json, opt) => json.Deserialize<FriendAddedEventArgs>(opt),
        ["group_recall"] = (json, opt) => json.Deserialize<GroupMessageRecalledEventArgs>(opt),
        ["friend_recall"] = (json, opt) => json.Deserialize<FriendMessageRecalledEventArgs>(opt),
        ["notify"] = DispatchNotify,
    };

    private static readonly Dictionary<string, Func<JsonElement, JsonSerializerOptions, OnebotV11EventArgsBase?>> _notifySubDispatch = new(StringComparer.Ordinal)
    {
        ["poke"] = (json, opt) => json.TryGetProperty("group_id", out _)
            ? json.Deserialize<GroupPokedEventArgs>(opt)
            : json.Deserialize<FriendPokedEventArgs>(opt),
        ["lucky_king"] = (json, opt) => json.Deserialize<GroupLuckyKingChangedEventArgs>(opt),
        ["honor"] = (json, opt) => json.Deserialize<GroupMemberHonorChangedEventArgs>(opt),
    };

    private static readonly Dictionary<string, Func<JsonElement, JsonSerializerOptions, OnebotV11EventArgsBase?>> _metaEventDispatch = new(StringComparer.Ordinal)
    {
        ["heartbeat"] = (json, opt) => json.Deserialize<HeartbeatEventArgs>(opt),
        ["lifecycle"] = (json, opt) => json.Deserialize<LifecycleEventArgs>(opt),
    };

    private static readonly Dictionary<string, Func<JsonElement, JsonSerializerOptions, OnebotV11EventArgsBase?>> _requestDispatch = new(StringComparer.Ordinal)
    {
        ["friend"] = (json, opt) => json.Deserialize<NewFriendRequestEventArgs>(opt),
        ["group"] = (json, opt) => json.Deserialize<GroupAddRequestEventArgs>(opt),
    };

    private static readonly Dictionary<string, Dictionary<string, Func<JsonElement, JsonSerializerOptions, OnebotV11EventArgsBase?>>> _rootDispatch = new(StringComparer.Ordinal)
    {
        ["message"] = _messageDispatch,
        ["notice"] = _noticeDispatch,
        ["meta_event"] = _metaEventDispatch,
        ["request"] = _requestDispatch,
    };

    /// <summary>
    /// 尝试把 JSON 派发到具体事件类型。
    /// </summary>
    public static bool TryDispatch(string postType, JsonElement json, JsonSerializerOptions options, out OnebotV11EventArgsBase? result)
    {
        if (!_rootDispatch.TryGetValue(postType, out var table))
        {
            result = null;
            return false;
        }

        var key = GetDispatchKey(json, postType);
        if (key is null || !table.TryGetValue(key, out var factory))
        {
            result = null;
            return false;
        }

        result = factory(json, options);
        return result is not null;
    }

    private static string? GetDispatchKey(JsonElement json, string postType) => postType switch
    {
        "message" => TryGetStringProperty(json, "message_type"),
        "notice" => TryGetStringProperty(json, "notice_type"),
        "meta_event" => TryGetStringProperty(json, "meta_event_type"),
        "request" => TryGetStringProperty(json, "request_type"),
        _ => null,
    };

    private static string? TryGetStringProperty(JsonElement json, string name)
    {
        // 字段缺失或非字符串都视为未知——派发表会按"未知"返回 false，调用方跳过此条
        if (!json.TryGetProperty(name, out var el) || el.ValueKind != JsonValueKind.String)
            return null;
        return el.GetString()?.ToLowerInvariant();
    }

    private static OnebotV11EventArgsBase? DispatchNotify(JsonElement json, JsonSerializerOptions options)
    {
        // OneBot 实现的 notify.sub_type 经常新增，SDK 的派发表无法覆盖全部。
        // 未知子类型返回 null，由上层 OnExceptionOccurrence 上报后跳过此条，
        // 不应当作致命错误中断事件循环。
        if (TryGetStringProperty(json, "sub_type") is not { } subType)
            return null;
        if (!_notifySubDispatch.TryGetValue(subType, out var factory))
            return null;
        return factory(json, options);
    }
}
