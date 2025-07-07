using System.Text.Json.Serialization;

namespace Gdr2333.BotLib.OnebotV11.Message.Parts.Payload;

internal class ImagePayload : FilePayload
{
    [JsonInclude, JsonPropertyName("type")]
    public string? Type { get; set; }
}
