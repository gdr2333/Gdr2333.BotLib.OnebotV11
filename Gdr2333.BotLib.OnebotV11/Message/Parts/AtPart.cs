using Gdr2333.BotLib.OnebotV11.Message.Parts.Base;
using Gdr2333.BotLib.OnebotV11.Message.Parts.Payload;
using System.Text.Json.Serialization;

namespace Gdr2333.BotLib.OnebotV11.Message.Parts;

/// <summary>
/// @消息段
/// </summary>
public class AtPart : MessagePartBase
{
    /// <summary>
    /// 被@的用户ID
    /// </summary>
    [JsonIgnore]
    public long UserId { get; set; }

    [JsonInclude, JsonRequired, JsonPropertyName("data")]
    private AtPayload? _data;

    /// <summary>
    /// 构造一个at消息段
    /// </summary>
    /// <param name="userId">要@的人</param>
    public AtPart(long userId) : base("at")
    {
        UserId = userId;
    }

    [JsonConstructor]
    private AtPart() : base("at")
    {
    }

    /// <inheritdoc/>
    public override string ToString() =>
        $"@{UserId}";

	/// <inheritdoc/>
	public override void OnDeserialized()
    {
        UserId = _data!.Id;
        _data = null;
    }

	/// <inheritdoc/>
	public override void OnSerializing() =>
        _data = new() { Id = UserId };
}
