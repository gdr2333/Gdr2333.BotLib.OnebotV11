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

namespace Gdr2333.BotLib.OnebotV11.Utils;

internal class MillisecondToTimeSpanConverter : JsonConverter<TimeSpan>
{
    public override TimeSpan Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
        TimeSpan.FromMilliseconds(reader.TokenType switch
        {
            JsonTokenType.String => DayToTimeSpanConverter.ParseStringAsDouble(ref reader, "毫秒数"),
            JsonTokenType.Number => reader.GetDouble(),
            _ => throw new JsonException("毫秒数字段收到非法 JSON 类型。")
        });

    public override void Write(Utf8JsonWriter writer, TimeSpan value, JsonSerializerOptions options) =>
        writer.WriteNumberValue((long)value.TotalMilliseconds);
}