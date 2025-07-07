using System.Text.Json.Serialization;

namespace Gdr2333.BotLib.OnebotV11.Message.Parts.Payload;

public class ContactPayload
{
    [JsonInclude, JsonRequired, JsonPropertyName("type")]
    public string Type { get; set; }

    [JsonInclude, JsonRequired, JsonPropertyName("id"), JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public long Id { get; set; }
}
