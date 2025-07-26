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

namespace Gdr2333.BotLib.OnebotV11.Data;

/// <summary>
/// 消息类型
/// </summary>
[JsonConverter(typeof(MessageTypeConverter))]
public enum MessageType
{
    /// <summary>
    /// 私聊消息
    /// </summary>
    Private,
    /// <summary>
    /// 群聊消息
    /// </summary>
    Group
}

internal class MessageTypeConverter : JsonConverter<MessageType>
{
    public override MessageType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
        reader.GetString()?.ToLower() switch
        {
            "private" => MessageType.Private,
            "group" => MessageType.Group,
            _ => throw new InvalidDataException(StaticData.BadEnumValueMessage)
        };

    public override void Write(Utf8JsonWriter writer, MessageType value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value switch
        {
            MessageType.Private => "private",
            MessageType.Group => "group",
            _ => throw new InvalidDataException(StaticData.BadEnumValueMessage)
        });
    }
}