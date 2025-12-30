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

using Gdr2333.BotLib.OnebotV11.Messages.TmpAlt;
using System.Text.Json.Serialization;

namespace Gdr2333.BotLib.OnebotV11.Messages;

/// <summary>
/// 消息段基类
/// </summary>
/// <param name="srcType">消息段类型</param>
// System.Text.Json限制了我们这里的JsonDerivedType不能套娃，但我们可以手动套娃。
[JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]

[JsonDerivedType(typeof(TextPart), typeDiscriminator: "text")]
[JsonDerivedType(typeof(FacePart), typeDiscriminator: "face")]
[JsonDerivedType(typeof(ImagePart), typeDiscriminator: "image")]
[JsonDerivedType(typeof(RecordPart), typeDiscriminator: "record")]
[JsonDerivedType(typeof(VideoPart), typeDiscriminator: "video")]
[JsonDerivedType(typeof(AtPart), typeDiscriminator: "at")]
[JsonDerivedType(typeof(RpsPart), typeDiscriminator: "rps")]
[JsonDerivedType(typeof(DicePart), typeDiscriminator: "dice")]
[JsonDerivedType(typeof(ShakePart), typeDiscriminator: "shake")]
[JsonDerivedType(typeof(PokePart), typeDiscriminator: "poke")]
#pragma warning disable CS0618
[JsonDerivedType(typeof(AnonymousPart), typeDiscriminator: "anonymous")]
#pragma warning restore CS0618
[JsonDerivedType(typeof(SharePart), typeDiscriminator: "share")]
[JsonDerivedType(typeof(ContactPartAlt), typeDiscriminator: "contact")]
[JsonDerivedType(typeof(LocationPart), typeDiscriminator: "location")]
[JsonDerivedType(typeof(ReplyPart), typeDiscriminator: "reply")]
[JsonDerivedType(typeof(ForwardMessagePart), typeDiscriminator: "forward")]
[JsonDerivedType(typeof(CustomForwardNodePart), typeDiscriminator: "node")]
[JsonDerivedType(typeof(XmlPart), typeDiscriminator: "xml")]
[JsonDerivedType(typeof(JsonPart), typeDiscriminator: "json")]

[JsonDerivedType(typeof(FriendContactPart))]
[JsonDerivedType(typeof(GroupContactPart))]
[JsonDerivedType(typeof(MusicSharePart))]
[JsonDerivedType(typeof(CustomMusicSharePart))]
[JsonDerivedType(typeof(ForwardNodePart))]
public abstract class MessagePartBase : IJsonOnSerializing, IJsonOnDeserialized
{
    /// <summary>
    /// JSON反序列化后的钩子
    /// </summary>
    public abstract void OnDeserialized();

    /// <summary>
    /// JSON序列化之前的钩子
    /// </summary>
    public abstract void OnSerializing();

    /// <summary>
    /// 返回消息段的文本表现形式
    /// </summary>
    /// <returns>文本表现形式</returns>
    public abstract override string ToString();
}
