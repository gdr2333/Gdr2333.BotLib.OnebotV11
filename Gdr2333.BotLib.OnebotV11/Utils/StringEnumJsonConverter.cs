/*
   Copyright 2026 All contributors of Gdr2333.BotLib

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

/// <summary>
/// OneBot v11 枚举的 JSON 转换器基类。
/// <para>支持两种使用方式：</para>
/// <list type="number">
///   <item><description>派生类不指定映射时，默认使用 <c>PascalCase</c> → <c>pascalcase</c> 的简单规则；</description></item>
///   <item><description>派生类通过基类构造函数传入 <see cref="KeyValuePair{TKey,TValue}"/> 列表来自定义 JSON 名称映射。</description></item>
/// </list>
/// </summary>
/// <typeparam name="T">枚举类型</typeparam>
internal abstract class StringEnumJsonConverter<T> : JsonConverter<T> where T : struct, Enum
{
    private readonly Dictionary<T, string>? _toName;
    private readonly Dictionary<string, T>? _toEnum;

    /// <summary>
    /// 默认构造：使用 <c>value.ToString().ToLowerInvariant()</c> 作为 JSON 名称。
    /// 派生类需实现 <see cref="FallbackValue"/> 和（可选）<see cref="ThrowOnUnknown"/>。
    /// </summary>
    protected StringEnumJsonConverter()
    {
    }

    /// <summary>
    /// 自定义映射的构造。
    /// </summary>
    /// <param name="fallback">未知 JSON 值时的兜底枚举值。</param>
    /// <param name="throwOnUnknown">遇到未知值时是否抛 <see cref="InvalidDataException"/>。</param>
    /// <param name="mapping">枚举值到 JSON 字符串的映射。键大小写不敏感。</param>
    protected StringEnumJsonConverter(T fallback, bool throwOnUnknown, IEnumerable<KeyValuePair<T, string>> mapping)
    {
        FallbackValue = fallback;
        ThrowOnUnknown = throwOnUnknown;
        _toName = new Dictionary<T, string>();
        _toEnum = new Dictionary<string, T>(StringComparer.OrdinalIgnoreCase);
        foreach (var kv in mapping)
        {
            _toName[kv.Key] = kv.Value;
            _toEnum[kv.Value] = kv.Key;
        }
    }

    /// <summary>
    /// 遇到未知 JSON 值时使用的兜底枚举值。
    /// </summary>
    protected T FallbackValue { get; set; } = default;

    /// <summary>
    /// <see langword="true"/> 表示遇到未知值时抛 <see cref="InvalidDataException"/>，否则返回 <see cref="FallbackValue"/>。
    /// </summary>
    protected bool ThrowOnUnknown { get; set; } = false;

    public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            // 修复：必须先消耗 Null token 再返回，否则后续解析器读到的 token 会错位
            reader.Read();
            return FallbackValue;
        }
        var s = reader.GetString();
        if (s is null)
        {
            // 字符串被读为 null（如某些 token 不是 String 也调到了这里）：丢弃 token + 兜底
            reader.Read();
            return FallbackValue;
        }
        reader.Read();
        if (_toEnum is not null)
        {
            if (_toEnum.TryGetValue(s, out var mapped))
                return mapped;
        }
        else if (Enum.TryParse(s, ignoreCase: true, out T parsed))
        {
            return parsed;
        }
        if (ThrowOnUnknown)
            throw new InvalidDataException(StaticData.BadEnumValueMessage);
        return FallbackValue;
    }

    public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
    {
        if (_toName is not null && _toName.TryGetValue(value, out var s))
        {
            writer.WriteStringValue(s);
            return;
        }
        writer.WriteStringValue(value.ToString().ToLowerInvariant());
    }
}
