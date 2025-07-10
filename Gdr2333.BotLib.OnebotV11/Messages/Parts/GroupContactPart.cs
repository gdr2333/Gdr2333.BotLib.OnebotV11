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

using Gdr2333.BotLib.OnebotV11.Messages.Parts.Base;
using Gdr2333.BotLib.OnebotV11.Messages.Parts.Payload;
using System.Text.Json.Serialization;

namespace Gdr2333.BotLib.OnebotV11.Messages.Parts;

/// <summary>
/// 群聊推荐消息段
/// </summary>
public class GroupContactPart : ContactPartBase
{
    [JsonInclude, JsonRequired, JsonPropertyName("data")]
    private ContactPayload? _data;

    [JsonConstructor]
    private GroupContactPart() : base("group")
    {
    }

    /// <summary>
    /// 新建一个群推荐消息段
    /// </summary>
    /// <param name="id">要推荐的群号</param>
    public GroupContactPart(long id) : base("group", id)
    {
    }

    /// <inheritdoc/>
    public override void OnDeserialized()
    {
        if (_data!.Type != "group")
            throw new InvalidOperationException("data的类型不是group，你选错了反序列化的目标类型。");
        AfterJsonDeserialization(_data!);
        _data = null;
    }

    /// <inheritdoc/>
    public override void OnSerializing() =>
        BeforeJsonSerialization(out _data);

    /// <inheritdoc/>
    public override string ToString() =>
        $"[推荐群聊：{Id}]";
}
