using System.Text.Json.Serialization;

namespace Gdr2333.BotLib.OnebotV11.Message.Parts.Payload;

internal class TextPartPayload
{
    [JsonInclude, JsonRequired, JsonPropertyName("text")]
    public string Text { get; set; }
}
