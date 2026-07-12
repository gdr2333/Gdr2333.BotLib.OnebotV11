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
using Gdr2333.BotLib.OnebotV11.Messages;
using Xunit;

namespace Gdr2333.BotLib.OnebotV11.Tests.Messages;

// Message 是 List<MessagePartBase>,序列化直接走基类 List<...> 的契约;
// MessageConverter 只在反序列化时介入(array → 段数组;string → CQ 码)。
public class MessageConverterTests
{
    [Fact]
    public void Read_FromJsonArray_ParsesSegments()
    {
        const string json = """
        [
          {"type":"text","data":{"text":"hello "}},
          {"type":"at","data":{"qq":12345}},
          {"type":"face","data":{"id":14}}
        ]
        """;
        var msg = JsonSerializer.Deserialize<Message>(json, TestJson.Options);
        Assert.NotNull(msg);
        Assert.Equal(3, msg!.Count);
        var t = Assert.IsType<TextPart>(msg[0]);
        Assert.Equal("hello ", t.Text);
        var at = Assert.IsType<AtPart>(msg[1]);
        Assert.Equal(12345L, at.UserId);
        var face = Assert.IsType<FacePart>(msg[2]);
        Assert.Equal(14, face.FaceId);
    }

    [Fact]
    public void Read_FromCqCodeString_RecoversSegments()
    {
        // CqCode 串走 CqCodeToJsonNode + MessageConverter.DeserializeSegments 反序列化。
        // 修复后 CqCodeToJsonNode 把扁平属性塞到 data 子对象,与协议 JSON 形状一致,
        // 因此 AtPart / FacePart 等能正确识别。
        var cq = "hi [CQ:at,qq=12345] [CQ:face,id=14]";
        var msg = JsonSerializer.Deserialize<Message>(JsonSerializer.Serialize(cq), TestJson.Options);
        Assert.NotNull(msg);
        // CqCodeToJsonNode 的段切分:hi[space] | [CQ:at,qq=12345] | [space] | [CQ:face,id=14]
        Assert.Equal(4, msg!.Count);
        Assert.IsType<TextPart>(msg[0]);
        Assert.Equal("hi ", ((TextPart)msg[0]).Text);
        var at = Assert.IsType<AtPart>(msg[1]);
        Assert.Equal(12345L, at.UserId);
        Assert.IsType<TextPart>(msg[2]);
        Assert.Equal(" ", ((TextPart)msg[2]).Text);
        var face = Assert.IsType<FacePart>(msg[3]);
        Assert.Equal(14, face.FaceId);
    }

    [Fact]
    public void Write_SerializesAsArray_WithTypeDiscriminator()
    {
        var msg = new Message(new TextPart("hi"));
        var json = JsonSerializer.Serialize(msg, TestJson.Options);
        Assert.Equal("[{\"type\":\"text\",\"data\":{\"text\":\"hi\"}}]", json);
    }

    [Fact]
    public void Read_BadSegment_WrappedAsTextPart()
    {
        // "type=garbage" 没有对应 JsonDerivedType,但 MessageConverter 的容错
        // 是 catch JsonException 后追加一个 TextPart 保留原 JSON。
        const string json = """
        [
          {"type":"text","data":{"text":"ok "}},
          {"type":"garbage","data":{"x":1}}
        ]
        """;
        var msg = JsonSerializer.Deserialize<Message>(json, TestJson.Options);
        Assert.NotNull(msg);
        Assert.Equal(2, msg!.Count);
        Assert.IsType<TextPart>(msg[0]);
        Assert.IsType<TextPart>(msg[1]); // 坏段被包装回 text
        Assert.StartsWith("[无法解读的消息段", ((TextPart)msg[1]).Text);
    }

    [Fact]
    public void Read_ContactQq_DispatchesToFriendContactPart()
    {
        const string json = """[{"type":"contact","data":{"type":"qq","id":12345}}]""";
        var msg = JsonSerializer.Deserialize<Message>(json, TestJson.Options);
        Assert.NotNull(msg);
        Assert.IsType<FriendContactPart>(msg![0]);
        Assert.Equal(12345L, ((FriendContactPart)msg[0]).Id);
    }

    [Fact]
    public void Read_ContactGroup_DispatchesToGroupContactPart()
    {
        const string json = """[{"type":"contact","data":{"type":"group","id":67890}}]""";
        var msg = JsonSerializer.Deserialize<Message>(json, TestJson.Options);
        Assert.NotNull(msg);
        Assert.IsType<GroupContactPart>(msg![0]);
        Assert.Equal(67890L, ((GroupContactPart)msg[0]).Id);
    }

    [Fact]
    public void Read_ContactUnknownType_StaysAsUnknownContactPart()
    {
        // ContactPartAlt.GetRealPart 的未知 type 路径应当保留为 UnknownContactPart,
        // 让调用方按需处理;不再抛异常打断消息反序列化。
        const string json = """[{"type":"contact","data":{"type":"channel","id":42}}]""";
        var msg = JsonSerializer.Deserialize<Message>(json, TestJson.Options);
        Assert.NotNull(msg);
        var part = msg![0];
        Assert.IsAssignableFrom<ContactPartBase>(part);
        Assert.Equal("channel", ((ContactPartBase)part).ContactType);
        Assert.Equal(42L, ((ContactPartBase)part).Id);
    }

    [Fact]
    public void Read_ForwardNode_DispatchesToCustomForwardNodePart()
    {
        // 协议里 type=node 永远对应 CustomForwardNodePart(包含 user_id/nickname/content),
        // ForwardNodePart 仅用于手动构造发出去(单 id 引用),不在反序列化路径上。
        const string json = """
        [
          {
            "type":"node",
            "data":{
              "user_id":1,
              "nickname":"alice",
              "content":[{"type":"text","data":{"text":"hi"}}]
            }
          }
        ]
        """;
        var msg = JsonSerializer.Deserialize<Message>(json, TestJson.Options);
        Assert.NotNull(msg);
        var node = Assert.IsType<CustomForwardNodePart>(Assert.Single(msg!));
        Assert.Equal(1L, node.UserId);
        Assert.Equal("alice", node.NickName);
        Assert.Single(node.Content);
    }

    [Fact]
    public void ToString_ConcatsAllPartStrings()
    {
        var msg = new Message(new List<MessagePartBase>
        {
            new TextPart("hello "),
            new AtPart(12345),
            new TextPart(" world"),
        });
        Assert.Equal("hello @12345 world", msg.ToString());
    }
}