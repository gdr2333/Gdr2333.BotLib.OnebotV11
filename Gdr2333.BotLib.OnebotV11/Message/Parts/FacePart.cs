using Gdr2333.BotLib.OnebotV11.Message.Parts.Base;
using Gdr2333.BotLib.OnebotV11.Message.Parts.Payload;
using System.Text.Json.Serialization;

namespace Gdr2333.BotLib.OnebotV11.Message.Parts;

/// <summary>
/// 表情消息段
/// </summary>
public class FacePart : MessagePartBase
{
    /// <summary>
    /// 表情ID
    /// </summary>
    [JsonIgnore]
    public int FaceId { get; set; }

    [JsonInclude, JsonRequired, JsonPropertyName("data")]
    private Int32IdPayload? _data;

    [JsonConstructor]
    private FacePart() : base("face")
    {
        FaceId = 0;
    }

    /// <summary>
    /// 使用一个表情ID构造一个表情消息段
    /// </summary>
    /// <param name="faceId">表情ID</param>
    public FacePart(int faceId) : base("face")
    {
        FaceId = faceId;
    }

    /// <inheritdoc/>
    public override string ToString() =>
        "[表情]";

	/// <inheritdoc/>
	public override void OnDeserialized()
    {
        FaceId = _data!.Id;
        _data = null;
    }

	/// <inheritdoc/>
	public override void OnSerializing() => _data = new() { Id = FaceId };
}
