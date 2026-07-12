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
using Xunit;

namespace Gdr2333.BotLib.OnebotV11.Tests.Events;

// OnebotV11EventArgsConverter 用派发表把 post_type + 子类型路由到具体事件类型。
// 这里覆盖每条主分支,确保派发表无遗漏。
public class EventDispatchTests
{
    [Fact]
    public void PrivateMessage_Dispatched()
    {
        const string json = """
        {
          "post_type":"message",
          "message_type":"private",
          "sub_type":"friend",
          "message_id":1,
          "user_id":12345,
          "message":[{"type":"text","data":{"text":"hi"}}],
          "raw_message":"hi",
          "font":0,
          "sender":{"user_id":12345,"nickname":"alice","sex":"unknown","age":0},
          "time":1700000000,
          "self_id":99999
        }
        """;
        using var doc = JsonDocument.Parse(json);
        var ev = doc.RootElement.Deserialize<OnebotV11EventArgsBase>(TestJson.Options);
        var pm = Assert.IsType<PrivateMessageReceivedEventArgs>(ev);
        Assert.Equal(1L, pm.MessageId);
        Assert.Equal(12345L, pm.UserId);
        Assert.Equal(PrivateMessageReceivedSubType.Friend, pm.SubType);
    }

    [Fact]
    public void GroupMessage_Dispatched()
    {
        const string json = """
        {
          "post_type":"message",
          "message_type":"group",
          "sub_type":"normal",
          "message_id":2,
          "user_id":12345,
          "group_id":67890,
          "message":[{"type":"text","data":{"text":"hi"}}],
          "raw_message":"hi",
          "font":0,
          "sender":{"user_id":12345,"nickname":"alice","sex":"unknown","age":0,"card":"","area":"","level":"","role":"","title":""},
          "time":1700000000,
          "self_id":99999
        }
        """;
        using var doc = JsonDocument.Parse(json);
        var ev = doc.RootElement.Deserialize<OnebotV11EventArgsBase>(TestJson.Options);
        var gm = Assert.IsType<GroupMessageReceivedEventArgs>(ev);
        Assert.Equal(2L, gm.MessageId);
        Assert.Equal(67890L, gm.GroupId);
        Assert.Equal(GroupMessageReceivedSubType.Normal, gm.SubType);
    }

    [Fact]
    public void GroupUpload_Dispatched()
    {
        const string json = """
        {
          "post_type":"notice",
          "notice_type":"group_upload",
          "group_id":1,
          "user_id":2,
          "file":{"id":"abc","name":"a.txt","size":10,"busid":1},
          "time":1700000000,
          "self_id":99999
        }
        """;
        using var doc = JsonDocument.Parse(json);
        var ev = doc.RootElement.Deserialize<OnebotV11EventArgsBase>(TestJson.Options);
        Assert.IsType<GroupFileUploadedEventArgs>(ev);
    }

    [Fact]
    public void GroupAdminChange_Dispatched()
    {
        const string json = """
        {
          "post_type":"notice",
          "notice_type":"group_admin",
          "sub_type":"set",
          "group_id":1,
          "user_id":2,
          "time":1700000000,
          "self_id":99999
        }
        """;
        using var doc = JsonDocument.Parse(json);
        var ev = doc.RootElement.Deserialize<OnebotV11EventArgsBase>(TestJson.Options);
        Assert.IsType<GroupAdminChangedEventArgs>(ev);
    }

    [Fact]
    public void GroupMemberIncrease_Dispatched()
    {
        const string json = """
        {
          "post_type":"notice",
          "notice_type":"group_increase",
          "sub_type":"approve",
          "group_id":1,
          "user_id":2,
          "operator_id":3,
          "time":1700000000,
          "self_id":99999
        }
        """;
        using var doc = JsonDocument.Parse(json);
        var ev = doc.RootElement.Deserialize<OnebotV11EventArgsBase>(TestJson.Options);
        Assert.IsType<GroupMemberIncreaseEventArgs>(ev);
    }

    [Fact]
    public void GroupMemberDecrease_Dispatched()
    {
        const string json = """
        {
          "post_type":"notice",
          "notice_type":"group_decrease",
          "sub_type":"leave",
          "group_id":1,
          "user_id":2,
          "operator_id":3,
          "time":1700000000,
          "self_id":99999
        }
        """;
        using var doc = JsonDocument.Parse(json);
        var ev = doc.RootElement.Deserialize<OnebotV11EventArgsBase>(TestJson.Options);
        Assert.IsType<GroupMemberDecreaseEventArgs>(ev);
    }

    [Fact]
    public void GroupBan_Dispatched()
    {
        const string json = """
        {
          "post_type":"notice",
          "notice_type":"group_ban",
          "sub_type":"ban",
          "group_id":1,
          "user_id":2,
          "operator_id":3,
          "duration":60,
          "time":1700000000,
          "self_id":99999
        }
        """;
        using var doc = JsonDocument.Parse(json);
        var ev = doc.RootElement.Deserialize<OnebotV11EventArgsBase>(TestJson.Options);
        Assert.IsType<GroupBanStatusChangedEventArgs>(ev);
    }

    [Fact]
    public void FriendAdd_Dispatched()
    {
        const string json = """
        {
          "post_type":"notice",
          "notice_type":"friend_add",
          "user_id":1,
          "time":1700000000,
          "self_id":99999
        }
        """;
        using var doc = JsonDocument.Parse(json);
        var ev = doc.RootElement.Deserialize<OnebotV11EventArgsBase>(TestJson.Options);
        Assert.IsType<FriendAddedEventArgs>(ev);
    }

    [Fact]
    public void Notify_Poke_GroupRoutedToGroupPoke()
    {
        const string json = """
        {
          "post_type":"notice",
          "notice_type":"notify",
          "sub_type":"poke",
          "group_id":1,
          "user_id":2,
          "target_id":3,
          "time":1700000000,
          "self_id":99999
        }
        """;
        using var doc = JsonDocument.Parse(json);
        var ev = doc.RootElement.Deserialize<OnebotV11EventArgsBase>(TestJson.Options);
        Assert.IsType<GroupPokedEventArgs>(ev);
    }

    [Fact]
    public void Notify_Poke_PrivateRoutedToFriendPoke()
    {
        const string json = """
        {
          "post_type":"notice",
          "notice_type":"notify",
          "sub_type":"poke",
          "user_id":2,
          "target_id":3,
          "time":1700000000,
          "self_id":99999
        }
        """;
        using var doc = JsonDocument.Parse(json);
        var ev = doc.RootElement.Deserialize<OnebotV11EventArgsBase>(TestJson.Options);
        Assert.IsType<FriendPokedEventArgs>(ev);
    }

    [Fact]
    public void Notify_LuckyKing_Dispatched()
    {
        const string json = """
        {
          "post_type":"notice",
          "notice_type":"notify",
          "sub_type":"lucky_king",
          "group_id":1,
          "user_id":2,
          "target_id":3,
          "time":1700000000,
          "self_id":99999
        }
        """;
        using var doc = JsonDocument.Parse(json);
        var ev = doc.RootElement.Deserialize<OnebotV11EventArgsBase>(TestJson.Options);
        Assert.IsType<GroupLuckyKingChangedEventArgs>(ev);
    }

    [Fact]
    public void Notify_Honor_Dispatched()
    {
        const string json = """
        {
          "post_type":"notice",
          "notice_type":"notify",
          "sub_type":"honor",
          "group_id":1,
          "user_id":2,
          "honor_type":"talkative",
          "time":1700000000,
          "self_id":99999
        }
        """;
        using var doc = JsonDocument.Parse(json);
        var ev = doc.RootElement.Deserialize<OnebotV11EventArgsBase>(TestJson.Options);
        Assert.IsType<GroupMemberHonorChangedEventArgs>(ev);
    }

    [Fact]
    public void Notify_UnknownSubtype_ReturnsNull()
    {
        // OneBot 实现的 notify.sub_type 经常新增,派发表之外的子类型应当返回 null
        // 由上层 OnExceptionOccurrence 上报,而不是硬抛。
        const string json = """
        {
          "post_type":"notice",
          "notice_type":"notify",
          "sub_type":"new_unknown_subtype",
          "time":1700000000,
          "self_id":99999
        }
        """;
        using var doc = JsonDocument.Parse(json);
        var ev = doc.RootElement.Deserialize<OnebotV11EventArgsBase>(TestJson.Options);
        Assert.Null(ev);
    }

    [Fact]
    public void Heartbeat_Dispatched()
    {
        const string json = """
        {
          "post_type":"meta_event",
          "meta_event_type":"heartbeat",
          "time":1700000000,
          "self_id":99999,
          "status":{"online":true,"good":true},
          "interval":5000
        }
        """;
        using var doc = JsonDocument.Parse(json);
        var ev = doc.RootElement.Deserialize<OnebotV11EventArgsBase>(TestJson.Options);
        var hb = Assert.IsType<HeartbeatEventArgs>(ev);
        Assert.Equal(TimeSpan.FromMilliseconds(5000), hb.Interval);
    }

    [Fact]
    public void Lifecycle_Dispatched()
    {
        const string json = """
        {
          "post_type":"meta_event",
          "meta_event_type":"lifecycle",
          "sub_type":"connect",
          "time":1700000000,
          "self_id":99999
        }
        """;
        using var doc = JsonDocument.Parse(json);
        var ev = doc.RootElement.Deserialize<OnebotV11EventArgsBase>(TestJson.Options);
        var lc = Assert.IsType<LifecycleEventArgs>(ev);
        Assert.Equal(LifecycleEventSubType.Connect, lc.Subtype);
    }

    [Fact]
    public void FriendRequest_Dispatched()
    {
        const string json = """
        {
          "post_type":"request",
          "request_type":"friend",
          "user_id":1,
          "comment":"hi",
          "flag":"flag-1",
          "time":1700000000,
          "self_id":99999
        }
        """;
        using var doc = JsonDocument.Parse(json);
        var ev = doc.RootElement.Deserialize<OnebotV11EventArgsBase>(TestJson.Options);
        Assert.IsType<NewFriendRequestEventArgs>(ev);
    }

    [Fact]
    public void GroupAddRequest_Dispatched()
    {
        const string json = """
        {
          "post_type":"request",
          "request_type":"group",
          "sub_type":"add",
          "group_id":1,
          "user_id":2,
          "comment":"hi",
          "flag":"flag-1",
          "time":1700000000,
          "self_id":99999
        }
        """;
        using var doc = JsonDocument.Parse(json);
        var ev = doc.RootElement.Deserialize<OnebotV11EventArgsBase>(TestJson.Options);
        Assert.IsType<GroupAddRequestEventArgs>(ev);
    }

    [Fact]
    public void MissingPostType_ReturnsNull()
    {
        // 缺 post_type 或非字符串:返回 null,由上层跳过。
        const string json = """{"time":1700000000,"self_id":1}""";
        using var doc = JsonDocument.Parse(json);
        var ev = doc.RootElement.Deserialize<OnebotV11EventArgsBase>(TestJson.Options);
        Assert.Null(ev);
    }

    [Fact]
    public void UnknownPostType_ReturnsNull()
    {
        // 派发表里没有的 post_type:返回 null。
        const string json = """{"post_type":"unknown_thing","time":1700000000,"self_id":1}""";
        using var doc = JsonDocument.Parse(json);
        var ev = doc.RootElement.Deserialize<OnebotV11EventArgsBase>(TestJson.Options);
        Assert.Null(ev);
    }

    [Fact]
    public void MissingSubtypeKey_ReturnsNull()
    {
        // post_type=message 但缺 message_type:派发表无 key,返回 null。
        const string json = """{"post_type":"message","time":1700000000,"self_id":1}""";
        using var doc = JsonDocument.Parse(json);
        var ev = doc.RootElement.Deserialize<OnebotV11EventArgsBase>(TestJson.Options);
        Assert.Null(ev);
    }
}