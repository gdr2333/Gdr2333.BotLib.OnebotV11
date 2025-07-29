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
using Gdr2333.BotLib.OnebotV11.Events.Interfaces;
using Gdr2333.BotLib.OnebotV11.Utils;

namespace Gdr2333.BotLib.OnebotV11.Events.Base;

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

internal class RequestTypeConverter : JsonConverter<RequestType>
{
    public override RequestType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
        reader.GetString()?.ToLower() switch
        {
            "friend" => RequestType.Friend,
            "group" => RequestType.Group,
            _ => throw new InvalidDataException(StaticData.BadEnumValueMessage)
        };

    public override void Write(Utf8JsonWriter writer, RequestType value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value switch
        {
            RequestType.Friend => "friend",
            RequestType.Group => "group",
            _ => throw new InvalidDataException(StaticData.BadEnumValueMessage)
        });
    }
}