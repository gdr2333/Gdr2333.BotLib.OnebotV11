using Gdr2333.BotLib.OnebotV11.Message.Parts.Base;
using Gdr2333.BotLib.OnebotV11.Message.Parts.Payload;
using System.Text.Json.Serialization;

namespace Gdr2333.BotLib.OnebotV11.Message.Parts;

/// <summary>
/// [弃用的]匿名消息段
/// </summary>
/// <remarks>
/// 创建一个匿名消息段
/// </remarks>
/// <param name="ignore">无法匿名时是否继续发送</param>
[Obsolete("anonymous消息段在几乎所有实现中都不再有用。我仍然按标准实现了它，但你不该用。")]
[method: Obsolete("anonymous消息段在几乎所有实现中都不再有用。我仍然按标准实现了它，但你不该用。")]
public class AnonymousPart(bool ignore) : MessagePartBase("anonymous")
{
    /// <summary>
    /// 无法匿名时是否继续发送
    /// </summary>
    [JsonIgnore]
    public bool Ignore { get; set; } = ignore;

    [JsonInclude, JsonPropertyName("data")]
    private AnonymousPayload? _data;

    /// <summary>
    /// 创建一个匿名消息段
    /// </summary>
    [Obsolete("anonymous消息段在几乎所有实现中都不再有用。我仍然按标准实现了它，但你不该用。")]
    [JsonConstructor]
    public AnonymousPart() : this(false) 
    {
    }

    /// <inheritdoc/>
    public override string ToString() =>
        string.Empty;

    /// <inheritdoc/>
    public override void OnDeserialized()
    {
        Ignore = _data?.Ignore ?? false;
        _data = null;
    }

    /// <inheritdoc/>
    public override void OnSerializing() =>
        _data = new()
        {
            Ignore = Ignore
        };
}
