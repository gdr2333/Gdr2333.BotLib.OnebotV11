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
using Gdr2333.BotLib.OnebotV11.Messages.Parts.Payload;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Gdr2333.BotLib.OnebotV11.Utils;

// 写Onebot v11标准的神人非得给预定义音乐分享的类型也塞进type字段里，我对此表示谴责。
internal class MusicSharePayloadConverter : JsonConverter<MusicSharePayloadBase>
{
    public override MusicSharePayloadBase? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var node = JsonElement.ParseValue(ref reader);
        if (node.TryGetProperty("type", out var type1))
            return type1.GetString() switch
            {
                "qq" or "163" or "xm" => JsonSerializer.Deserialize<MusicSharePayload>(node),
                "custom" => JsonSerializer.Deserialize<CustomMusicSharePayload>(node),
                _ => throw new InvalidDataException($"音乐分享消息段：未知的类型{type1}"),
            };
        else
            throw new FormatException("JSON中不存在type字段");
    }

    public override void Write(Utf8JsonWriter writer, MusicSharePayloadBase value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, value, options);
    }
}
