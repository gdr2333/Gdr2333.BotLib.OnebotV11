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

using Gdr2333.BotLib.OnebotV11.Utils;
using Xunit;

namespace Gdr2333.BotLib.OnebotV11.Tests.Utils;

// CqCodeToJsonNode 把 CQ-code 字符串转成 JsonArray,每段是 { type, ...props } 或 text 段。
// 这里锁定"反序列化前"那一刻的形状,具体的反序列化由 MessageConverter 继续处理。
public class CqCodeToJsonNodeTests
{
    [Fact]
    public void PlainText_Becomes_TextSegment()
    {
        var node = CqCodeToJsonNode.Convert("hello world");
        var arr = Assert.IsType<System.Text.Json.Nodes.JsonArray>(node);
        Assert.Single(arr);
        var seg = Assert.IsType<System.Text.Json.Nodes.JsonObject>(arr[0]);
        Assert.Equal("text", seg["type"]!.GetValue<string>());
        Assert.Equal("hello world", seg["data"]!["text"]!.GetValue<string>());
    }

    [Fact]
    public void SingleCqCode_Becomes_SingleSegment()
    {
        var node = CqCodeToJsonNode.Convert("[CQ:face,id=14]");
        var arr = Assert.IsType<System.Text.Json.Nodes.JsonArray>(node);
        Assert.Single(arr);
        var seg = Assert.IsType<System.Text.Json.Nodes.JsonObject>(arr[0]);
        Assert.Equal("face", seg["type"]!.GetValue<string>());
        // CQ 码扁平属性被塞到 data 子对象,与协议 JSON 形状保持一致。
        Assert.Equal("14", seg["data"]!["id"]!.GetValue<string>());
    }

    [Fact]
    public void MixedTextAndCqCode_SplitsIntoMultipleSegments()
    {
        var node = CqCodeToJsonNode.Convert("hi [CQ:at,qq=123456] there");
        var arr = Assert.IsType<System.Text.Json.Nodes.JsonArray>(node);
        Assert.Equal(3, arr.Count);
        // 1: 文本 "hi "
        var t1 = Assert.IsType<System.Text.Json.Nodes.JsonObject>(arr[0]);
        Assert.Equal("text", t1["type"]!.GetValue<string>());
        Assert.Equal("hi ", t1["data"]!["text"]!.GetValue<string>());
        // 2: at 段
        var at = Assert.IsType<System.Text.Json.Nodes.JsonObject>(arr[1]);
        Assert.Equal("at", at["type"]!.GetValue<string>());
        Assert.Equal("123456", at["data"]!["qq"]!.GetValue<string>());
        // 3: 文本 " there"
        var t2 = Assert.IsType<System.Text.Json.Nodes.JsonObject>(arr[2]);
        Assert.Equal("text", t2["type"]!.GetValue<string>());
        Assert.Equal(" there", t2["data"]!["text"]!.GetValue<string>());
    }

    [Fact]
    public void MultipleCqCodes_Adjacent_SplitCorrectly()
    {
        var node = CqCodeToJsonNode.Convert("[CQ:face,id=1][CQ:face,id=2]");
        var arr = Assert.IsType<System.Text.Json.Nodes.JsonArray>(node);
        Assert.Equal(2, arr.Count);
    }

    [Fact]
    public void PropValueDecoding_UnescapesEntities()
    {
        // &amp; &#91; &#93; &#44; &quot; 应当被还原为 & [ ] , "
        var node = CqCodeToJsonNode.Convert("[CQ:image,file=a&amp;b&#91;c&#93;d&#44;e&quot;f,url=img.png]");
        var arr = Assert.IsType<System.Text.Json.Nodes.JsonArray>(node);
        var seg = Assert.IsType<System.Text.Json.Nodes.JsonObject>(arr[0]);
        Assert.Equal("a&b[c]d,e\"f", seg["data"]!["file"]!.GetValue<string>());
        Assert.Equal("img.png", seg["data"]!["url"]!.GetValue<string>());
    }

    [Fact]
    public void TextDecoding_UnescapesEntities()
    {
        // 文本段的解码不包括逗号还原(注意区分 prop 和 text 的解码集合)。
        var node = CqCodeToJsonNode.Convert("a&amp;b&#91;c&#93;d&quot;e");
        var arr = Assert.IsType<System.Text.Json.Nodes.JsonArray>(node);
        var seg = Assert.IsType<System.Text.Json.Nodes.JsonObject>(arr[0]);
        Assert.Equal("a&b[c]d\"e", seg["data"]!["text"]!.GetValue<string>());
    }

    [Fact]
    public void BareTypeOnlyCqCode_HasEmptyData()
    {
        // 仅有 [CQ:xxx] 没有属性:切出完整 type + 空 data 对象。
        // (修复后 CqCodeToJsonNode 把 props 移到 data 子对象,空 CQ 码对应 data={})
        var node = CqCodeToJsonNode.Convert("[CQ:anonymous]");
        var arr = Assert.IsType<System.Text.Json.Nodes.JsonArray>(node);
        var seg = Assert.IsType<System.Text.Json.Nodes.JsonObject>(arr[0]);
        Assert.Equal("anonymous", seg["type"]!.GetValue<string>());
        var data = Assert.IsType<System.Text.Json.Nodes.JsonObject>(seg["data"]);
        Assert.Empty(data);
    }

    [Fact]
    public void EmptyString_Becomes_EmptyArray()
    {
        var node = CqCodeToJsonNode.Convert("");
        var arr = Assert.IsType<System.Text.Json.Nodes.JsonArray>(node);
        Assert.Empty(arr);
    }

    [Fact]
    public void UnclosedBracket_TreatedAsTextAfterPoint()
    {
        // 没有匹配的 ']':剩余部分整段作为 CQ 码本体然后结束。
        var node = CqCodeToJsonNode.Convert("abc[def");
        var arr = Assert.IsType<System.Text.Json.Nodes.JsonArray>(node);
        // '[' 之前是 "abc" 文本,然后从 '[' 到末尾作为不闭合的 CQ 码
        Assert.Equal(2, arr.Count);
    }

    [Fact]
    public void PropWithoutEquals_TreatedAsKeyWithNull()
    {
        var node = CqCodeToJsonNode.Convert("[CQ:share,extra,title=hi]");
        var arr = Assert.IsType<System.Text.Json.Nodes.JsonArray>(node);
        var seg = Assert.IsType<System.Text.Json.Nodes.JsonObject>(arr[0]);
        Assert.Equal("share", seg["type"]!.GetValue<string>());
        var data = Assert.IsType<System.Text.Json.Nodes.JsonObject>(seg["data"]);
        Assert.Equal("hi", data["title"]!.GetValue<string>());
        // 无值的属性以 C# null 形式存入 JsonObject(JsonNode API 把 null 值映射到
        // 索引器返回 null)。所以这里用 Count + ContainsKey 间接验证 "extra" 已被记录。
        Assert.Equal(2, data.Count);
        Assert.True(data.ContainsKey("extra"));
        Assert.Null(data["extra"]);
    }
}