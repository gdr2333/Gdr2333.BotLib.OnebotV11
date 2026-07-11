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

using Gdr2333.BotLib.OnebotV11.Messages.Payload;
using System.Text.Json.Serialization;

namespace Gdr2333.BotLib.OnebotV11.Messages;

/// <summary>
/// 未知类型的 contact 段。
/// <para>当 OneBot 实现在 <c>contact.data.type</c> 上产生非 "qq"/"group" 的值（如新平台扩展）时，
/// 由 <see cref="OnebotV11.Messages.TmpAlt.ContactPartAlt.GetRealPart"/> 兜底产出，避免整条消息反序列化失败。</para>
/// </summary>
public sealed class UnknownContactPart : ContactPartBase
{
    [JsonInclude, JsonRequired, JsonPropertyName("data")]
    private ContactPayload? _data;

    [JsonConstructor]
    private UnknownContactPart() : base()
    {
    }

    /// <summary>
    /// 新建一个未知类型的 contact 段。
    /// </summary>
    /// <param name="contactType">原始 type（例如上游新增的 "channel"）</param>
    /// <param name="id">目标 ID</param>
    public UnknownContactPart(string contactType, long id) : base(contactType, id)
    {
    }

    /// <inheritdoc/>
    protected override void OnDeserialized()
    {
        // 没有强制匹配：保留原始 type，仅在序列化时通过 BeforeJsonSerialization 输出。
        AfterJsonDeserialization(_data!);
        _data = null;
    }

    /// <inheritdoc/>
    protected override void OnSerializing() =>
        BeforeJsonSerialization(out _data!);

    /// <inheritdoc/>
    public override string ToString() =>
        $"[未知推荐：{ContactType}:{Id}]";
}
