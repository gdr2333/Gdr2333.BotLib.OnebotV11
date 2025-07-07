using Gdr2333.BotLib.OnebotV11.Message.Parts.Base;
using Gdr2333.BotLib.OnebotV11.Message.Parts.Payload;
using System.Text.Json.Serialization;

namespace Gdr2333.BotLib.OnebotV11.Message.Parts.TmpAlt;

internal class ContactPartAlt : ContactPartBase
{
    [JsonInclude, JsonRequired, JsonPropertyName("data")]
    private ContactPayload? _data;

    public override string ToString()
    {
        throw new InvalidOperationException("为什么你调用到了这个类的ToString方法？");
    }

    public override void OnDeserialized()
    {
        AfterJsonDeserialization(_data!);
        _data = null;
    }

    public override void OnSerializing()
    {
        BeforeJsonSerialization(out _data);
    }
}
