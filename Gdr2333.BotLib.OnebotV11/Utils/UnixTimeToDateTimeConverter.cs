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

internal class UnixTimeToDateTimeConverter : JsonConverter<DateTime>
{
    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
        DateTime.UnixEpoch.AddSeconds(reader.TokenType switch
        {
            JsonTokenType.Number => reader.GetDouble(),
            JsonTokenType.String => double.Parse(reader.GetString() ?? throw new FormatException("这里读不到数据？？？什么情况")),
            _ => throw new FormatException("时间戳解析失败：JSON类型不是数字也不是字符串")
        });

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options) =>
        writer.WriteNumberValue((long)(value - DateTime.UnixEpoch).TotalSeconds);
}
