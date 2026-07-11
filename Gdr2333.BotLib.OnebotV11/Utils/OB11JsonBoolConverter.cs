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

// true和false是标准的JSON格式，1和0是C语言流传下来的格式，那请问**的Yes和No是什么鬼？？？害的我还得写这个破玩意
internal class OB11JsonBoolConverter : JsonConverter<bool>
{
    public override bool Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        switch (reader.TokenType)
        {
            case JsonTokenType.True:
                reader.Read();
                return true;
            case JsonTokenType.False:
                reader.Read();
                return false;
            case JsonTokenType.Number:
                var num = reader.GetInt64();
                reader.Read();
                return num != 0;
            case JsonTokenType.String:
                var str = reader.GetString()?.ToLower();
                reader.Read();
                if (str is null)
                    throw new JsonException("布尔字段收到了字符串 null。");
                // 字符串里允许 0/1（如 "1"）、true/false（大小写不敏感）、yes/no 等。
                // 不接受小数、任意整数（即便溢出）一律按非法处理。
                if (long.TryParse(str, System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture, out var num2)
                    && num2 >= 0 && num2 <= 1)
                    return num2 != 0;
                if (bool.TryParse(str, out var num3))
                    return num3;
                if (str == "yes")
                    return true;
                if (str == "no")
                    return false;
                // 非法字符串：保留硬失败语义（让上层 OnExceptionOccurrence 收到），
                // 但用 JsonException 让父级 STj 通道统一处理；事件循环不再因此终结。
                throw new JsonException($"布尔字段收到非法字符串 \"{str}\"。");
            case JsonTokenType.Null:
                reader.Read();
                return false;
            default:
                throw new JsonException($"布尔字段收到非法 token {reader.TokenType}。");
        }
    }

    public override void Write(Utf8JsonWriter writer, bool value, JsonSerializerOptions options) =>
        writer.WriteBooleanValue(value);
}
