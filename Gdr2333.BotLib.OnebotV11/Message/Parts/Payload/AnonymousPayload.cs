using Gdr2333.BotLib.OnebotV11.Utils;
using System.Text.Json.Serialization;

namespace Gdr2333.BotLib.OnebotV11.Message.Parts.Payload;

internal class AnonymousPayload
{
    [JsonInclude, JsonPropertyName("ignore"), JsonConverter(typeof(OB11JsonBoolConvter))]
    public bool Ignore { get; set; }
}
