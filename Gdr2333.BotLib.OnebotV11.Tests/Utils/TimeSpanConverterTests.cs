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

// *TimeSpanConverter 共享 DayToTimeSpanConverter.ParseStringAsDouble,
// 这里覆盖 Number 分支、String 分支、非法 token 三种路径,以及 Write 输出。
public class TimeSpanConverterTests
{
    [Fact]
    public void SecondConverter_Read_Number()
    {
        var c = new SecondToTimeSpanConverter();
        var r = new Utf8JsonReader(System.Text.Encoding.UTF8.GetBytes("30"));
        r.Read();
        Assert.Equal(TimeSpan.FromSeconds(30), c.Read(ref r, typeof(TimeSpan), TestJson.Options));
    }

    [Fact]
    public void SecondConverter_Read_String()
    {
        var c = new SecondToTimeSpanConverter();
        var r = new Utf8JsonReader(System.Text.Encoding.UTF8.GetBytes("\"30.5\""));
        r.Read();
        Assert.Equal(TimeSpan.FromSeconds(30.5), c.Read(ref r, typeof(TimeSpan), TestJson.Options));
    }

    [Fact]
    public void SecondConverter_Read_BadString_Throws()
    {
        var c = new SecondToTimeSpanConverter();
        var r = new Utf8JsonReader(System.Text.Encoding.UTF8.GetBytes("\"abc\""));
        r.Read();
        try
        {
            c.Read(ref r, typeof(TimeSpan), TestJson.Options);
            Assert.Fail("应抛 JsonException。");
        }
        catch (JsonException)
        {
            // 预期
        }
    }

    [Fact]
    public void SecondConverter_Read_ObjectToken_Throws()
    {
        var c = new SecondToTimeSpanConverter();
        var r = new Utf8JsonReader(System.Text.Encoding.UTF8.GetBytes("{}"));
        r.Read();
        try
        {
            c.Read(ref r, typeof(TimeSpan), TestJson.Options);
            Assert.Fail("应抛 JsonException。");
        }
        catch (JsonException)
        {
            // 预期
        }
    }

    [Fact]
    public void SecondConverter_Write_TruncatesToLongSeconds()
    {
        var c = new SecondToTimeSpanConverter();
        var buffer = new System.Buffers.ArrayBufferWriter<byte>();
        using (var w = new Utf8JsonWriter(buffer))
        {
            c.Write(w, TimeSpan.FromSeconds(42), TestJson.Options);
        }
        Assert.Equal("42", System.Text.Encoding.UTF8.GetString(buffer.WrittenSpan));
    }

    [Fact]
    public void MillisecondConverter_Read_AndWrite()
    {
        var c = new MillisecondToTimeSpanConverter();
        var r = new Utf8JsonReader(System.Text.Encoding.UTF8.GetBytes("1500"));
        r.Read();
        Assert.Equal(TimeSpan.FromMilliseconds(1500), c.Read(ref r, typeof(TimeSpan), TestJson.Options));

        var buffer = new System.Buffers.ArrayBufferWriter<byte>();
        using (var w = new Utf8JsonWriter(buffer))
        {
            c.Write(w, TimeSpan.FromMilliseconds(2500), TestJson.Options);
        }
        Assert.Equal("2500", System.Text.Encoding.UTF8.GetString(buffer.WrittenSpan));
    }

    [Fact]
    public void MillisecondConverter_Read_String()
    {
        var c = new MillisecondToTimeSpanConverter();
        var r = new Utf8JsonReader(System.Text.Encoding.UTF8.GetBytes("\"123.4\""));
        r.Read();
        Assert.Equal(TimeSpan.FromMilliseconds(123.4), c.Read(ref r, typeof(TimeSpan), TestJson.Options));
    }

    [Fact]
    public void DayConverter_Read_AndWrite()
    {
        var c = new DayToTimeSpanConverter();
        var r = new Utf8JsonReader(System.Text.Encoding.UTF8.GetBytes("3"));
        r.Read();
        Assert.Equal(TimeSpan.FromDays(3), c.Read(ref r, typeof(TimeSpan), TestJson.Options));

        var buffer = new System.Buffers.ArrayBufferWriter<byte>();
        using (var w = new Utf8JsonWriter(buffer))
        {
            c.Write(w, TimeSpan.FromDays(7), TestJson.Options);
        }
        Assert.Equal("7", System.Text.Encoding.UTF8.GetString(buffer.WrittenSpan));
    }

    [Fact]
    public void DayConverter_Read_String()
    {
        var c = new DayToTimeSpanConverter();
        var r = new Utf8JsonReader(System.Text.Encoding.UTF8.GetBytes("\"2.5\""));
        r.Read();
        Assert.Equal(TimeSpan.FromDays(2.5), c.Read(ref r, typeof(TimeSpan), TestJson.Options));
    }

    [Fact]
    public void UnixTimeConverter_Read_Number()
    {
        var c = new UnixTimeToDateTimeConverter();
        var r = new Utf8JsonReader(System.Text.Encoding.UTF8.GetBytes("0"));
        r.Read();
        Assert.Equal(DateTime.UnixEpoch, c.Read(ref r, typeof(DateTime), TestJson.Options));
    }

    [Fact]
    public void UnixTimeConverter_Read_String()
    {
        var c = new UnixTimeToDateTimeConverter();
        var r = new Utf8JsonReader(System.Text.Encoding.UTF8.GetBytes("\"1700000000\""));
        r.Read();
        var actual = c.Read(ref r, typeof(DateTime), TestJson.Options);
        Assert.Equal(DateTime.UnixEpoch.AddSeconds(1700000000), actual);
    }

    [Fact]
    public void UnixTimeConverter_Read_BadType_Throws()
    {
        var c = new UnixTimeToDateTimeConverter();
        var r = new Utf8JsonReader(System.Text.Encoding.UTF8.GetBytes("true"));
        r.Read();
        try
        {
            c.Read(ref r, typeof(DateTime), TestJson.Options);
            Assert.Fail("应抛 JsonException。");
        }
        catch (JsonException)
        {
            // 预期
        }
    }

    [Fact]
    public void UnixTimeConverter_Write()
    {
        var c = new UnixTimeToDateTimeConverter();
        var buffer = new System.Buffers.ArrayBufferWriter<byte>();
        using (var w = new Utf8JsonWriter(buffer))
        {
            c.Write(w, DateTime.UnixEpoch.AddSeconds(42), TestJson.Options);
        }
        Assert.Equal("42", System.Text.Encoding.UTF8.GetString(buffer.WrittenSpan));
    }
}