using Gdr2333.BotLib.OnebotV11.Message.Parts.Base;
using System.Text.Json.Serialization;

namespace Gdr2333.BotLib.OnebotV11.Message.Parts;

/// <summary>
/// 猜拳魔法表情*消息段
/// </summary>
/// <remarks>
/// *：原文如此
/// </remarks>
[method: JsonConstructor]
public class RpsPart() : MessagePartBase("rps")
{
    /// <inheritdoc/>
    public override string ToString() =>
        "[猜拳]";

	/// <inheritdoc/>
	public override void OnDeserialized()
    {
    }

	/// <inheritdoc/>
	public override void OnSerializing()
    {
    }
}
