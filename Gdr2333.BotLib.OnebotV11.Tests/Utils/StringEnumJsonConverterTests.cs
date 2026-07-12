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
using Gdr2333.BotLib.OnebotV11.Events;
using Gdr2333.BotLib.OnebotV11.Utils;
using Xunit;

namespace Gdr2333.BotLib.OnebotV11.Tests.Utils;

// PostTypeConverter 是 StringEnumJsonConverter<PostType> 的派生,
// 喂 mapping + throwOnUnknown=true:用于覆盖自定义映射分支。
public class StringEnumJsonConverterTests
{
    [Theory]
    [InlineData("\"message\"", PostType.Message)]
    [InlineData("\"notice\"", PostType.Notice)]
    [InlineData("\"request\"", PostType.Request)]
    [InlineData("\"meta_event\"", PostType.MetaEvent)]
    // 大小写不敏感
    [InlineData("\"MESSAGE\"", PostType.Message)]
    public void PostTypeConverter_Read_KnownValues(string literal, PostType expected)
    {
        var converter = new PostTypeConverter();
        var reader = new Utf8JsonReader(System.Text.Encoding.UTF8.GetBytes(literal));
        reader.Read();
        Assert.Equal(expected, converter.Read(ref reader, typeof(PostType), TestJson.Options));
    }

    [Fact]
    public void PostTypeConverter_Read_UnknownThrows()
    {
        // PostTypeConverter 用 throwOnUnknown=true,所以未知值抛 InvalidDataException,
        // 由外层 JsonConverter 包装机制上报,符合 CLAUDE.md 关于 "坏枚举硬失败" 的语义。
        var converter = new PostTypeConverter();
        var reader = new Utf8JsonReader(System.Text.Encoding.UTF8.GetBytes("\"nonexistent\""));
        reader.Read();
        try
        {
            converter.Read(ref reader, typeof(PostType), TestJson.Options);
            Assert.Fail("应抛 InvalidDataException,但未抛。");
        }
        catch (InvalidDataException)
        {
            // 预期
        }
    }

    [Fact]
    public void PostTypeConverter_Read_NullToken_ReturnsFallback()
    {
        // Null token 走的是 StringEnumJsonConverter.Read 里单独的 Null 分支,
        // 必须前进 reader 并返回 FallbackValue(Mapping 模式里默认是 default(T) = Message)。
        var converter = new PostTypeConverter();
        var reader = new Utf8JsonReader(System.Text.Encoding.UTF8.GetBytes("null"));
        reader.Read();
        Assert.Equal(PostType.Message, converter.Read(ref reader, typeof(PostType), TestJson.Options));
        // reader 推进位置不再断言具体 token — 只断言 Read 不抛且产生 fallback。
    }

    [Theory]
    [InlineData(PostType.Message, "\"message\"")]
    [InlineData(PostType.Notice, "\"notice\"")]
    [InlineData(PostType.Request, "\"request\"")]
    [InlineData(PostType.MetaEvent, "\"meta_event\"")]
    public void PostTypeConverter_Write_AlwaysLowercase(PostType value, string expected)
    {
        var converter = new PostTypeConverter();
        var buffer = new System.Buffers.ArrayBufferWriter<byte>();
        using (var writer = new Utf8JsonWriter(buffer))
        {
            converter.Write(writer, value, TestJson.Options);
        }
        Assert.Equal(expected, System.Text.Encoding.UTF8.GetString(buffer.WrittenSpan));
    }

    // 默认构造(无 mapping)的派生类走 Enum.TryParse 分支,
    // 写时按 ToString().ToLowerInvariant() 输出。
    // 这里写一个最小派生,在测试内部以实现生产代码用映射、测试代码用默认的能力对比。
    private sealed class DefaultConverter : StringEnumJsonConverter<TestEnum> { }

    public enum TestEnum
    {
        Foo,
        BarBaz,
    }

    [Theory]
    [InlineData("\"foo\"", TestEnum.Foo)]
    [InlineData("\"barbaz\"", TestEnum.BarBaz)]
    [InlineData("\"FOO\"", TestEnum.Foo)]   // ignoreCase 路径
    public void DefaultConverter_Read(string literal, TestEnum expected)
    {
        var converter = new DefaultConverter();
        var reader = new Utf8JsonReader(System.Text.Encoding.UTF8.GetBytes(literal));
        reader.Read();
        Assert.Equal(expected, converter.Read(ref reader, typeof(TestEnum), TestJson.Options));
    }

    [Theory]
    [InlineData(TestEnum.Foo, "\"foo\"")]
    [InlineData(TestEnum.BarBaz, "\"barbaz\"")]
    public void DefaultConverter_Write_LowercasesViaToString(TestEnum value, string expected)
    {
        var converter = new DefaultConverter();
        var buffer = new System.Buffers.ArrayBufferWriter<byte>();
        using (var writer = new Utf8JsonWriter(buffer))
        {
            converter.Write(writer, value, TestJson.Options);
        }
        Assert.Equal(expected, System.Text.Encoding.UTF8.GetString(buffer.WrittenSpan));
    }
}