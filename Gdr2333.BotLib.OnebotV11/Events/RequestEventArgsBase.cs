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
using System.Text.Json.Serialization;

namespace Gdr2333.BotLib.OnebotV11.Events;

/// <summary>
/// 请求事件基类
/// </summary>
[JsonDerivedType(typeof(NewFriendRequestEventArgs))]
[JsonDerivedType(typeof(GroupAddRequestEventArgs))]
public class RequestEventArgsBase : OnebotV11EventArgsBase, IUserEventArgs
{
    /// <summary>
    /// 请求类型
    /// </summary>
    [JsonInclude, JsonRequired, JsonPropertyName("request_type")]
    public RequestType Type { get; internal set; }

    /// <summary>
    /// 发起请求的用户Id
    /// </summary>
    [JsonInclude, JsonRequired, JsonPropertyName("user_id"), JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public long UserId { get; internal set; }

    /// <summary>
    /// 请求Flag
    /// </summary>
    [JsonInclude, JsonRequired, JsonPropertyName("flag")]
    public string Flag { get; internal set; } = string.Empty;
}

/// <summary>
/// 请求类型
/// </summary>
[JsonConverter(typeof(RequestTypeConverter))]
public enum RequestType
{
    /// <summary>
    /// 好友请求
    /// </summary>
    Friend,
    /// <summary>
    /// 请求加群 或 邀请进群
    /// </summary>
    Group
}

internal sealed class RequestTypeConverter : StringEnumJsonConverter<RequestType>
{
    public RequestTypeConverter() : base(
        fallback: RequestType.Friend,
        throwOnUnknown: true,
        mapping: new Dictionary<RequestType, string>
        {
            { RequestType.Friend, "friend" },
            { RequestType.Group, "group" }
        })
    {
    }
}