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

using Gdr2333.BotLib.OnebotV11.Message.Parts.TmpAlt;
using System.Text.Json.Serialization;

namespace Gdr2333.BotLib.OnebotV11.Message.Parts.Base;

/// <summary>
/// 消息段基类
/// </summary>
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
[JsonDerivedType(typeof(AnonymousPart), typeDiscriminator: "anonymous")]
[JsonDerivedType(typeof(SharePart), typeDiscriminator: "share")]
[JsonDerivedType(typeof(ContactPartAlt), typeDiscriminator: "contact")]
[JsonDerivedType(typeof(LocationPart), typeDiscriminator: "location")]
[JsonDerivedType(typeof(ReplyPart), typeDiscriminator: "reply")]
[JsonDerivedType(typeof(ForwardMessagePart), typeDiscriminator: "forward")]

[JsonDerivedType(typeof(FilePartBase))]
[JsonDerivedType(typeof(ContactPartBase))]
[JsonDerivedType(typeof(MusicSharePartBase))]

[JsonDerivedType(typeof(FriendContactPart))]
[JsonDerivedType(typeof(GroupContactPart))]
[JsonDerivedType(typeof(MusicSharePart))]
[JsonDerivedType(typeof(CustomMusicSharePart))]
public abstract class MessagePartBase : IJsonOnSerializing, IJsonOnDeserialized
{
    /// <summary>
    /// 消息类型
    /// </summary>
    [JsonInclude, JsonRequired, JsonPropertyName("type")]
    public string Type { get; protected set; }

    /// <summary>
    /// 构建消息段基类初始化函数
    /// </summary>
    /// <param name="type">消息段类型</param>
    public MessagePartBase(string type)
    {
        Type = type;
    }

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
