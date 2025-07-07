using Gdr2333.BotLib.OnebotV11.Message.Parts.Base;
using System.Text.Json.Serialization;

namespace Gdr2333.BotLib.OnebotV11.Message.Parts;

/// <summary>
/// 窗口抖动消息段
/// </summary>
[method: JsonConstructor]
public class ShakePart() : MessagePartBase("shake")
{
    /// <inheritdoc/>
    public override string ToString() =>
        "[窗口抖动]";

	/// <inheritdoc/>
	public override void OnDeserialized()
    {
    }

	/// <inheritdoc/>
	public override void OnSerializing()
    {
    }
}
