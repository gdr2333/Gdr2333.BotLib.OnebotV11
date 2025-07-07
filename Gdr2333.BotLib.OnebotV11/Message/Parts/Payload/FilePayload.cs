using Gdr2333.BotLib.OnebotV11.Utils;
using System.Text.Json.Serialization;

namespace Gdr2333.BotLib.OnebotV11.Message.Parts.Payload;

/// <summary>
/// 文件负载类
/// </summary>
public class FilePayload
{
    /// <summary>
    /// 文件名
    /// </summary>
    [JsonInclude, JsonRequired, JsonPropertyName("file")]
    public string File { get; set; }

    /// <summary>
    /// 文件URL
    /// </summary>
    // JsonRequired实际上是JsonRequiredWhenDeserializaion。
    [JsonInclude, JsonRequired, JsonPropertyName("url")]
    public Uri? Url { get; set; }

    /// <summary>
    /// （发送时）是否使用缓存
    /// </summary>
    [JsonInclude, JsonPropertyName("cache"), JsonConverter(typeof(OB11JsonBoolConvter))]
    public bool UseCache { get; set; }

    /// <summary>
    /// （发送时）是否使用代理
    /// </summary>
    [JsonInclude, JsonPropertyName("proxy"), JsonConverter(typeof (OB11JsonBoolConvter))]
    public bool UseProxy { get; set; }

    /// <summary>
    /// （发送时）超时时间0
    /// </summary>
    [JsonInclude, JsonPropertyName("timeout"), JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public int? TimeOut { get; set; } = null;
}
