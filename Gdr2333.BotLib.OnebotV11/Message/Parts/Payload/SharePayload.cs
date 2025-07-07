using System.Text.Json.Serialization;

namespace Gdr2333.BotLib.OnebotV11.Message.Parts.Payload;

internal class SharePayload
{
    [JsonInclude, JsonRequired, JsonPropertyName("url")]
    public required Uri Url { get; set; }

    [JsonInclude, JsonRequired, JsonPropertyName("title")]
    public required string Title { get; set; }

    [JsonInclude, JsonPropertyName("content")]
    public string? Content { get; set; }

    [JsonInclude, JsonPropertyName("image")]
    public Uri? ImageUrl { get; set; }
}
