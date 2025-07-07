/*
   Copyright 2025 All contributors of Gdr2333.BotLib

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
*/

using Gdr2333.BotLib.OnebotV11.Message.Parts.Base;
using Gdr2333.BotLib.OnebotV11.Message.Parts.Payload;
using System.Text.Json.Serialization;

namespace Gdr2333.BotLib.OnebotV11.Message.Parts;

/// <summary>
/// 链接分享消息段
/// </summary>
public class SharePart : MessagePartBase
{
    /// <summary>
    /// 要分享的链接
    /// </summary>
    [JsonIgnore]
    public Uri Url { get; set; }

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
    /// 内容图片URL
    /// </summary>
    [JsonIgnore]
    public Uri? ImageUrl { get; set; }

    [JsonInclude, JsonRequired, JsonPropertyName("data")]
    private SharePayload? _data;

    [JsonConstructor]
    private SharePart() : base("share")
    {
    }

    /// <summary>
    /// 构建一个链接分享消息段
    /// </summary>
    /// <param name="url">要分享的链接</param>
    /// <param name="title">分享标题</param>
    /// <param name="content">分享内容介绍</param>
    /// <param name="imageUrl">内容图片URL</param>
    public SharePart(Uri url, string title, string? content = null, Uri? imageUrl = null)
    {
        Url = url;
        Title = title;
        Content = content;
        ImageUrl = imageUrl;
    }

    /// <inheritdoc/>
    public override string ToString() =>
        $"[链接：{Url}]";

    /// <inheritdoc/>
    public override void OnDeserialized()
    {
        Url = _data!.Url;
        Title = _data!.Title;
        Content = _data!.Content;
        ImageUrl = _data!.ImageUrl;
        _data = null;
    }

    /// <inheritdoc/>
    public override void OnSerializing() =>
        _data = new()
        {
            Url = Url,
            Title = Title,
            Content = Content,
            ImageUrl = ImageUrl
        };
}
