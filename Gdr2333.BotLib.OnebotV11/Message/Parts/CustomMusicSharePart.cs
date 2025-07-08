using Gdr2333.BotLib.OnebotV11.Message.Parts.Base;
using Gdr2333.BotLib.OnebotV11.Message.Parts.Payload;
using System.Text.Json.Serialization;

namespace Gdr2333.BotLib.OnebotV11.Message.Parts;

/// <summary>
/// 自定义音乐分享消息段
/// </summary>
public class CustomMusicSharePart : MusicSharePartBase
{
    /// <summary>
    /// 点击跳转到的URL
    /// </summary>
    [JsonIgnore]
    public Uri ToUrl { get; set; }

    /// <summary>
    /// 音频URL
    /// </summary>
    [JsonIgnore]
    public Uri AudioUrl { get; set; }

    /// <summary>
    /// 分享标题
    /// </summary>
    [JsonIgnore]
    public string Title { get; set; }

    /// <summary>
    /// 分享内容介绍
    /// </summary>
    [JsonIgnore]
    public string? Content { get; set; }

    /// <summary>
    /// 分享封面URL
    /// </summary>
    [JsonIgnore]
    public Uri? ImageUrl { get; set; }

    [JsonInclude, JsonRequired, JsonPropertyName("data")]
    private CustomMusicSharePayload? _data;

    [JsonConstructor]
    private CustomMusicSharePart()
    {
        MusicShareType = "costum";
    }

    /// <summary>
    /// 新建一个自定义音乐分享消息段
    /// </summary>
    /// <param name="toUrl">点击跳转到的URL</param>
    /// <param name="audioUrl">音频URL</param>
    /// <param name="title">分享标题</param>
    /// <param name="content">分享内容介绍</param>
    /// <param name="imageUrl">分享封面URL</param>
    public CustomMusicSharePart(Uri toUrl, Uri audioUrl, string title, string? content = null, Uri? imageUrl = null)
    {
        MusicShareType = "costum";
        ToUrl = toUrl;
        AudioUrl = audioUrl;
        Title = title;
        Content = content;
        ImageUrl = imageUrl;
    }

    /// <inheritdoc/>
    public override void OnDeserialized()
    {
        ToUrl = _data!.Url;
        AudioUrl = _data!.AudioUrl;
        Title = _data!.Title;
        Content = _data!.Content;
        ImageUrl = _data!.ImageUrl;
        _data = null;
    }

    /// <inheritdoc/>
    public override void OnSerializing()
    {
        _data = new()
        {
            Url = ToUrl,
            AudioUrl = _data!.AudioUrl,
            Title = _data!.Title,
            Content = _data!.Content,
            ImageUrl = _data!.ImageUrl,
        };
    }

    /// <inheritdoc/>
    public override string ToString() =>
        "[自定义音乐分享]";
}
