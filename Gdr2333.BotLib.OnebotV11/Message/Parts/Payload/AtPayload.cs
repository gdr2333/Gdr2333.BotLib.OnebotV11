using System.Text.Json.Serialization;

namespace Gdr2333.BotLib.OnebotV11.Message.Parts.Payload;

internal class AtPayload
{
    [JsonInclude, JsonRequired, JsonPropertyName("qq"), JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public long Id { get; set; }
}
