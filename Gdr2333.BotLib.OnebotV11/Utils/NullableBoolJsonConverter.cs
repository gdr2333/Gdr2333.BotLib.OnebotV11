/*
   Copyright 2025-2026 All contributors of Gdr2333.BotLib

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

// bool? 字段的 OneBot v11 bool 转换器。
// 与 OB11JsonBoolConverter 是同义不同型:OB11JsonBoolConverter 派生 JsonConverter<bool>
// 应用到 bool? 字段时会被 STj 包一层 NullableConverter<bool>,反序列化时触发
// "read too much or not enough" 验证失败。本 converter 直接派生 JsonConverter<bool?>,
// 走 STj 自己的 reader 推进协议:
// - True/False/Number/String token: STj 框架在 converter.Read 返回后统一推进一次。
//   converter 内不主动 Read,避免与 STj 的 VerifyRead 冲突。
// - Null token: STj 不会主动推进,converter 必须自己 Read。
//
// 详见 [[stj-jsonconverter-read-protocol]] 与 [[stj-bool-nullable-verify-read-bug]]。
internal class NullableBoolJsonConverter : JsonConverter<bool?>
{
    public override bool? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        switch (reader.TokenType)
        {
            case JsonTokenType.True:
                return true;
            case JsonTokenType.False:
                return false;
            case JsonTokenType.Number:
                var num = reader.GetInt64();
                if (num is < 0 or > 1)
                    throw new JsonException($"布尔字段收到非法整数 {num}，仅接受 0 或 1。");
                return num != 0;
            case JsonTokenType.String:
                var str = reader.GetString()?.ToLower();
                if (str is null)
                    throw new JsonException("布尔字段收到了字符串 null。");
                if (long.TryParse(str, System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture, out var num2)
                    && num2 >= 0 && num2 <= 1)
                    return num2 != 0;
                if (bool.TryParse(str, out var num3))
                    return num3;
                if (str == "yes")
                    return true;
                if (str == "no")
                    return false;
                throw new JsonException($"布尔字段收到非法字符串 \"{str}\"。");
            case JsonTokenType.Null:
                // STj 不会推进 Null token,converter 必须自己 Read。
                reader.Read();
                return null;
            default:
                throw new JsonException($"布尔字段收到非法 token {reader.TokenType}。");
        }
    }

    public override void Write(Utf8JsonWriter writer, bool? value, JsonSerializerOptions options)
    {
        if (value.HasValue)
            writer.WriteBooleanValue(value.Value);
        else
            writer.WriteNullValue();
    }
}