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
using Gdr2333.BotLib.OnebotV11.Utils;
using Xunit;

namespace Gdr2333.BotLib.OnebotV11.Tests.Utils;

// OB11JsonBoolConverter 的 Read 路径覆盖 OneBot v11 实际实现里出现过的所有布尔表达:
// 原生 true/false、整数 0/1、"1"/"0"、"true"/"false"、"yes"/"no"、null。
// 非法输入(超出 0/1 的整数、奇怪字符串、其它 token 类型)必须抛 JsonException。
public class OB11JsonBoolConverterTests
{
    [Theory]
    [InlineData("true", true)]
    [InlineData("false", false)]
    [InlineData("0", false)]
    [InlineData("1", true)]
    [InlineData("\"0\"", false)]
    [InlineData("\"1\"", true)]
    [InlineData("\"true\"", true)]
    [InlineData("\"False\"", false)]
    [InlineData("\"yes\"", true)]
    [InlineData("\"no\"", false)]
    [InlineData("\"YES\"", true)]
    [InlineData("null", false)]
    public void Read_Accepts_AllKnownForms(string literal, bool expected)
    {
        var converter = new OB11JsonBoolConverter();
        var reader = new Utf8JsonReader(System.Text.Encoding.UTF8.GetBytes(literal));
        reader.Read();
        Assert.Equal(expected, converter.Read(ref reader, typeof(bool), TestJson.Options));
    }

    [Theory]
    [InlineData("2")]
    [InlineData("-1")]
    [InlineData("100")]
    public void Read_NumberOutOfRange_Throws(string literal)
    {
        var converter = new OB11JsonBoolConverter();
        var reader = new Utf8JsonReader(System.Text.Encoding.UTF8.GetBytes(literal));
        reader.Read();
        try
        {
            converter.Read(ref reader, typeof(bool), TestJson.Options);
            Assert.Fail($"应抛 JsonException,但 reader 在 \"{literal}\" 上未抛。");
        }
        catch (JsonException)
        {
            // 预期
        }
    }

    [Theory]
    [InlineData("\"maybe\"")]
    [InlineData("\"yesno\"")]
    [InlineData("\"\"")]
    [InlineData("\"null\"")]
    public void Read_BadString_Throws(string literal)
    {
        var converter = new OB11JsonBoolConverter();
        var reader = new Utf8JsonReader(System.Text.Encoding.UTF8.GetBytes(literal));
        reader.Read();
        try
        {
            converter.Read(ref reader, typeof(bool), TestJson.Options);
            Assert.Fail($"应抛 JsonException,但 reader 在 {literal} 上未抛。");
        }
        catch (JsonException)
        {
            // 预期
        }
    }

    [Fact]
    public void Read_ArrayToken_Throws()
    {
        var converter = new OB11JsonBoolConverter();
        var reader = new Utf8JsonReader(System.Text.Encoding.UTF8.GetBytes("[1,2]"));
        reader.Read();
        try
        {
            converter.Read(ref reader, typeof(bool), TestJson.Options);
            Assert.Fail("应抛 JsonException,但 reader 在数组 token 上未抛。");
        }
        catch (JsonException)
        {
            // 预期
        }
    }

    [Theory]
    [InlineData(true, "true")]
    [InlineData(false, "false")]
    public void Write_EmitsNativeBoolean(bool value, string expected)
    {
        var converter = new OB11JsonBoolConverter();
        var buffer = new System.Buffers.ArrayBufferWriter<byte>();
        using (var writer = new Utf8JsonWriter(buffer))
        {
            converter.Write(writer, value, TestJson.Options);
        }
        Assert.Equal(expected, System.Text.Encoding.UTF8.GetString(buffer.WrittenSpan));
    }
}