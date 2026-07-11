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

using Gdr2333.BotLib.OnebotV11.Messages.Payload;
using System.Text.Json.Serialization;

namespace Gdr2333.BotLib.OnebotV11.Messages.TmpAlt;

internal class ContactPartAlt : ContactPartBase
{
    [JsonInclude, JsonRequired, JsonPropertyName("data")]
    private ContactPayload? _data;

    public override string ToString()
    {
        throw new InvalidOperationException("为什么你调用到了这个类的ToString方法？");
    }

    protected override void OnDeserialized()
    {
        AfterJsonDeserialization(_data!);
        _data = null;
    }

    protected override void OnSerializing()
    {
        BeforeJsonSerialization(out _data);
    }

    public ContactPartBase GetRealPart()
    {
        if (ContactType == "qq")
            return new FriendContactPart(Id);
        else if (ContactType == "group")
            return new GroupContactPart(Id);
        else
            // 未知 type：保留为 ContactPartBase 让调用方按需处理；不再抛异常打断消息反序列化。
            // 这是 OneBot 实现在 contact 段上新增 type（如 "channel"）时的兼容路径。
            return new UnknownContactPart(ContactType, Id);
    }
}
