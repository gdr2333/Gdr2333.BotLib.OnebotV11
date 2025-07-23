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
/// “通知”事件数据基类。鬼知道为什么有这个东西。
/// </summary>
public abstract class NotifyEventArgsBase : NoticeEventArgsBase
{
    /// <summary>
    /// 通知子类型
    /// </summary>
    [JsonInclude, JsonRequired, JsonPropertyName("sub_type")]
    public NotifySubtype Subtype { get; internal set; }
}

/// <summary>
/// 通知子类型
/// </summary>
[JsonConverter(typeof(NotifySubtypeConverter))]
public enum NotifySubtype
{
    /// <summary>
    /// （群）戳一戳
    /// </summary>
    Poke,
    /// <summary>
    /// （群红包）运气王
    /// </summary>
    LuckyKing,
    /// <summary>
    /// 群成员荣誉变更
    /// </summary>
    Honor
}

internal class NotifySubtypeConverter : JsonConverter<NotifySubtype>
{
    public override NotifySubtype Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
        reader.GetString()?.ToLower() switch
        {
            "poke" => NotifySubtype.Poke,
            "lucky_king" => NotifySubtype.LuckyKing,
            "honor" => NotifySubtype.Honor,
            _ => throw new InvalidDataException(StaticData.BadEnumValueMessage)
        };

    public override void Write(Utf8JsonWriter writer, NotifySubtype value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value switch
        {
            NotifySubtype.Poke => "poke",
            NotifySubtype.LuckyKing => "lucky_king",
            NotifySubtype.Honor => "honor",
            _ => throw new InvalidDataException(StaticData.BadEnumValueMessage)
        });
    }
}