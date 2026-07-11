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

using Gdr2333.BotLib.OnebotV11.Utils;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Gdr2333.BotLib.OnebotV11.Events;

/// <summary>
/// 事件参数基类
/// </summary>
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

internal sealed class PostTypeConverter : StringEnumJsonConverter<PostType>
{
    public PostTypeConverter() : base(
        fallback: PostType.Message,
        throwOnUnknown: true,
        mapping: new Dictionary<PostType, string>
        {
            { PostType.Message, "message" },
            { PostType.Notice, "notice" },
            { PostType.Request, "request" },
            { PostType.MetaEvent, "meta_event" }
        })
    {
    }
}

/// <summary>
/// OneBot v11 事件反序列化器。
/// <para>使用派发表（dispatch table）将 <c>post_type</c> + 子类型字段路由到具体的事件参数类型。</para>
/// </summary>
internal sealed class OnebotV11EventArgsConverter : JsonConverter<OnebotV11EventArgsBase>
{
    private static readonly JsonConverter<OnebotV11EventArgsBase> _defconv =
        (JsonConverter<OnebotV11EventArgsBase>)JsonSerializerOptions.Default.GetConverter(typeof(OnebotV11EventArgsBase));

    public override OnebotV11EventArgsBase? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var json = JsonElement.ParseValue(ref reader);
        if (!json.TryGetProperty("post_type", out var postTypeEl) || postTypeEl.GetString() is not { } postType)
        {
            // 缺失或非字符串 post_type：返回 null，由上层 ReceiveLoop/EventLoop 跳过
            return null;
        }
        if (!OnebotV11EventDispatch.TryDispatch(postType, json, options, out var result))
        {
            // 未知 post_type 或派发表无对应条目：同上
            return null;
        }
        return result;
    }

    public override void Write(Utf8JsonWriter writer, OnebotV11EventArgsBase value, JsonSerializerOptions options) =>
        _defconv.Write(writer, value, options);
}
