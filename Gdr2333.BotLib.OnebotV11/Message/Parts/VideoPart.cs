using Gdr2333.BotLib.OnebotV11.Message.Parts.Base;
using Gdr2333.BotLib.OnebotV11.Message.Parts.Payload;
using Gdr2333.BotLib.OnebotV11.Utils;
using System.Text.Json.Serialization;

namespace Gdr2333.BotLib.OnebotV11.Message.Parts;

/// <summary>
/// 视频消息段
/// </summary>
public class VideoPart : FilePartBase
{
    [JsonInclude, JsonRequired, JsonPropertyName("data")]
    private FilePayload? _data;

    /// <inheritdoc/>
    public override string ToString() =>
        "[视频]";

    [JsonConstructor]
    private VideoPart() : base("video")
    {
    }

    /// <summary>
    /// 构建一个视频消息段
    /// </summary>
    /// <param name="fileName">文件名</param>
    /// <param name="useCache">是否使用缓存</param>
    /// <param name="useProxy">是否使用代理</param>
    /// <param name="timeOut">超时时长</param>
    public VideoPart(string fileName, bool useCache = StaticData.CqUseCahceDefault, bool useProxy = StaticData.CqUseProxyDefault, TimeSpan? timeOut = null)
        : base("video", fileName, useCache, useProxy, timeOut)
    {
    }

    /// <summary>
    /// 构建一个视频消息段
    /// </summary>
    /// <param name="file">文件URL</param>
    /// <param name="useCache">是否使用缓存</param>
    /// <param name="useProxy">是否使用代理</param>
    /// <param name="timeOut">超时时长</param>
    public VideoPart(Uri file, bool useCache = StaticData.CqUseCahceDefault, bool useProxy = StaticData.CqUseProxyDefault, TimeSpan? timeOut = null)
        : this(file.AbsoluteUri, useCache, useProxy, timeOut)
    {
    }

    /// <summary>
    /// 构建一个视频消息段
    /// </summary>
    /// <param name="file">文件内容</param>
    public VideoPart(byte[] file)
        : this($"base64://{Convert.ToBase64String(file)}")
    {
    }

    /// <inheritdoc/>
    public override void OnDeserialized()
    {
        AfterJsonDeserialization(_data!);
        _data = null;
    }

    /// <inheritdoc/>
    public override void OnSerializing()
    {
        _data = new();
        BeforeJsonSerialization(_data!);
    }
}
