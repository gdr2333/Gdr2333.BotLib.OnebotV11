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

using Gdr2333.BotLib.OnebotV11.Message.Parts;
using Gdr2333.BotLib.OnebotV11.Messages.Parts.Base;
using Gdr2333.BotLib.OnebotV11.Messages.Parts.Payload;
using System.Text.Json.Serialization;

namespace Gdr2333.BotLib.OnebotV11.Messages.Parts.TmpAlt;

internal class ContactPartAlt : ContactPartBase
{
    [JsonInclude, JsonRequired, JsonPropertyName("data")]
    private ContactPayload? _data;

    public override string ToString()
    {
        throw new InvalidOperationException("为什么你调用到了这个类的ToString方法？");
    }

    public override void OnDeserialized()
    {
        AfterJsonDeserialization(_data!);
        _data = null;
    }

    public override void OnSerializing()
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
            throw new FormatException($"标准规定推荐类型只能是qq或group，但我们见到了{ContactType}。");
    }
}
