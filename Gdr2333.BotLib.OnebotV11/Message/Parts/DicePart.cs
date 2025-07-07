using Gdr2333.BotLib.OnebotV11.Message.Parts.Base;
using System.Text.Json.Serialization;

namespace Gdr2333.BotLib.OnebotV11.Message.Parts;

/// <summary>
/// 掷骰子魔法表情*消息段
/// </summary>
/// <remarks>
/// *：原文如此
/// </remarks>
[method: JsonConstructor]
public class DicePart() : MessagePartBase("dice")
{
    /// <inheritdoc/>
    public override string ToString() =>
        "[掷骰子]";

	/// <inheritdoc/>
	public override void OnDeserialized()
    {
    }

	/// <inheritdoc/>
	public override void OnSerializing()
    {
    }
}
