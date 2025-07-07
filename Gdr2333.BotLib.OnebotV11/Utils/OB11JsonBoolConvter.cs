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
internal class OB11JsonBoolConvter : JsonConverter<bool>
{
    public override bool Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        switch (reader.TokenType)
        {
            case JsonTokenType.True:
                return true;
            case JsonTokenType.False:
                return false;
            case JsonTokenType.Number:
                var num = reader.GetInt64();
                return num != 0;
            case JsonTokenType.String:
                var str = reader.GetString()?.ToLower();
                if (long.TryParse(str, out var num2))
                    return num2 != 0;
                // 能写出"true"的也是神人了
                else if (bool.TryParse(str, out var num3))
                    return num3;
                else if (str == "yes")
                    return true;
                else if (str == "no")
                    return false;
                else
                    throw new FormatException($"{str}是个啥啊？？？");
            case JsonTokenType.Null:
                return false;
            default:
                throw new FormatException($"{reader.TokenType}又是啥？？？");
        }
    }

    public override void Write(Utf8JsonWriter writer, bool value, JsonSerializerOptions options) =>
        writer.WriteBooleanValue(value);
}
