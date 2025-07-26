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

namespace Gdr2333.BotLib.OnebotV11.Data;

/// <summary>
/// 群消息发送者信息
/// </summary>
public class GroupMemberInfo : UserInfo
{
    /// <summary>
    /// 群卡片内容
    /// </summary>
    [JsonInclude, JsonPropertyName("card")]
    public string? CardInfo { get; internal set; } = null;

    /// <summary>
    /// 地区
    /// </summary>
    [JsonInclude, JsonPropertyName("area")]
    public string? Area { get; internal set; } = null;

    /// <summary>
    /// 群等级（也不知道为啥设计成字符串了）
    /// </summary>
    [JsonInclude, JsonPropertyName("level")]
    public string? Level { get; internal set; } = null;

    /// <summary>
    /// 群角色（群主/管理/成员）
    /// </summary>
    [JsonInclude, JsonPropertyName("role")]
    public MemberRole? Role { get; internal set; } = null;

    /// <summary>
    /// 群头衔
    /// </summary>
    [JsonInclude, JsonPropertyName("title")]
    public string? Title { get; internal set; } = null;
}

/// <summary>
/// 群角色
/// </summary>
[JsonConverter(typeof(MemberRoleConverter))]
public enum MemberRole
{
    /// <summary>
    /// 群主
    /// </summary>
    Owner,
    /// <summary>
    /// 管理员
    /// </summary>
    Admin,
    /// <summary>
    /// 成员
    /// </summary>
    Member
};

internal class MemberRoleConverter : JsonConverter<MemberRole>
{
    public override MemberRole Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
    reader.GetString()?.ToLower() switch
    {
        "owner" => MemberRole.Owner,
        "admin" => MemberRole.Admin,
        _ => MemberRole.Member,
    };

    public override void Write(Utf8JsonWriter writer, MemberRole value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value switch
        {
            MemberRole.Owner => "owner",
            MemberRole.Admin => "admin",
            _ => "member"
        });
    }
}