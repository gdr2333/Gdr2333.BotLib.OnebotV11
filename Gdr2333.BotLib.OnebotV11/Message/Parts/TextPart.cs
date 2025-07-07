using Gdr2333.BotLib.OnebotV11.Message.Parts.Base;
using Gdr2333.BotLib.OnebotV11.Message.Parts.Payload;
using System.Text.Json.Serialization;

namespace Gdr2333.BotLib.OnebotV11.Message.Parts;

/// <summary>
/// 文本消息段
/// </summary>
public class TextPart : MessagePartBase
{
    [JsonInclude, JsonRequired, JsonPropertyName("data")]
    private TextPartPayload? _data = null;

    /// <summary>
    /// 消息段内容
    /// </summary>
    [JsonIgnore]
    public string Text { get; set; } = "";

    /// <summary>
    /// 构造一个空的文本消息段
    /// </summary>
    [JsonConstructor]
    public TextPart() : base("text")
    {
    }

    /// <summary>
    /// 使用指定文本构造一个文本消息段
    /// </summary>
    /// <param name="text">消息段内容</param>
    public TextPart(string text) : base("text")
    {
        Text = text;
    }

	/// <inheritdoc/>
	public override void OnDeserialized()
    {
        Text = _data!.Text;
        _data = null;
    }

	/// <inheritdoc/>
	public override void OnSerializing() =>
        _data = new() { Text = Text };

    /// <summary>
    /// 返回消息段的文本表现形式
    /// </summary>
    /// <returns>文本表现形式</returns>
    public override string ToString() =>
        Text;
}
