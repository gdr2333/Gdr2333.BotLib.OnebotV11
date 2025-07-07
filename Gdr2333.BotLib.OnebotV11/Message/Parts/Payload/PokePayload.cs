using System.Text.Json.Serialization;

namespace Gdr2333.BotLib.OnebotV11.Message.Parts.Payload;

internal class PokePayload
{
    [JsonInclude, JsonRequired, JsonPropertyName("type"), JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public int Type { get; set; }

    [JsonInclude, JsonRequired, JsonPropertyName("id"), JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public int Id { get; set; }

    [JsonInclude, JsonPropertyName("name")]
    public string? Name { get; set; }
}
