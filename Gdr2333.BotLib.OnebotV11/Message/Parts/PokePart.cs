using Gdr2333.BotLib.OnebotV11.Message.Parts.Base;
using Gdr2333.BotLib.OnebotV11.Message.Parts.Payload;
using System.Text.Json.Serialization;

namespace Gdr2333.BotLib.OnebotV11.Message.Parts;

/// <summary>
/// 戳一戳消息段
/// </summary>
public class PokePart : MessagePartBase
{
    /// <summary>
    /// 戳一戳类型
    /// </summary>
    [JsonIgnore]
    public int PokeType { get; set; }

    /// <summary>
    /// 戳一戳ID
    /// </summary>
    [JsonIgnore]
    public int Id { get; set; }

    /// <summary>
    /// 表情名称
    /// </summary>
    [JsonIgnore]
    public string? Name { get; set; }

    [JsonInclude, JsonRequired, JsonPropertyName("data")]
    private PokePayload? _data;

    [JsonConstructor]
    private PokePart() : base("poke")
    {
    }

    /// <summary>
    /// 构建一个戳一戳消息段
    /// </summary>
    /// <param name="type">戳一戳类型</param>
    /// <param name="id">戳一戳ID</param>
    public PokePart(int type, int id)
    {
        PokeType = type;
        Id = id;
    }

    /// <inheritdoc/>
    public override string ToString() =>
        $"[{Name ?? "戳一戳"}]";

	/// <inheritdoc/>
	public override void OnDeserialized()
    {
        PokeType = _data!.Type;
        Id = _data!.Id;
        Name = _data!.Name;
        _data = null;
    }

	/// <inheritdoc/>
	public override void OnSerializing() =>
        _data = new()
        {
            Type = PokeType,
            Id = Id,
            Name = Name
        };
}
