using Gdr2333.BotLib.OnebotV11.Utils;
using System.Runtime.Serialization;
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

[JsonDerivedType(typeof(FilePartBase))]
[JsonDerivedType(typeof(MessagePartBase))]
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
    /// 不初始化消息段类型的构造函数，只应用于反序列化。
    /// </summary>
    [JsonConstructor]
    protected MessagePartBase()
    {
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
