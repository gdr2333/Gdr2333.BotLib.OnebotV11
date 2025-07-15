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

namespace Gdr2333.BotLib.OnebotV11.Events.Data;

/// <summary>
/// 消息发送者信息
/// </summary>
[JsonDerivedType(typeof(GroupSender))]
public class Sender
{
    /// <summary>
    /// 用户ID
    /// </summary>
    [JsonInclude, JsonPropertyName("user_id"), JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public long UserId { get; internal set; } = -1;

    /// <summary>
    /// 昵称
    /// </summary>
    [JsonInclude, JsonPropertyName("nickname")]
    public string Nickname { get; internal set; } = string.Empty;

    /// <summary>
    /// 性别
    /// </summary>
    [JsonInclude, JsonPropertyName("sex")]
    public Gender Gender { get; internal set; } = Gender.Unknow;

    /// <summary>
    /// 年龄
    /// </summary>
    [JsonInclude, JsonPropertyName("age"), JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public int Age { get; internal set; } = -1;
}

/// <summary>
/// 性别枚举
/// </summary>
[JsonConverter(typeof(GenderConverter))]
public enum Gender
{
    /// <summary>
    /// 男
    /// </summary>
    Male,
    /// <summary>
    /// 女
    /// </summary>
    Female,
    /// <summary>
    /// 没写
    /// </summary>
    Unknow
};

internal class GenderConverter : JsonConverter<Gender>
{
    public override Gender Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
    reader.GetString()?.ToLower() switch
    {
        "male" => Gender.Male,
        "female" => Gender.Female,
        _ => Gender.Unknow,
    };

    public override void Write(Utf8JsonWriter writer, Gender value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value switch
        {
            Gender.Male => "male",
            Gender.Female => "female",
            _ => "unknow"
        });
    }
}