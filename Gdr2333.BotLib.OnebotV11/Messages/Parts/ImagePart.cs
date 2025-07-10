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

using Gdr2333.BotLib.OnebotV11.Messages.Parts.Base;
using Gdr2333.BotLib.OnebotV11.Messages.Parts.Payload;
using Gdr2333.BotLib.OnebotV11.Utils;
using System.Text.Json.Serialization;

namespace Gdr2333.BotLib.OnebotV11.Messages.Parts;

/// <summary>
/// 图片消息段
/// </summary>
public class ImagePart : FilePartBase
{
    /// <summary>
    /// 是否为闪照
    /// </summary>
    [JsonIgnore]
    public bool IsFlash { get; set; }

    [JsonInclude, JsonRequired, JsonPropertyName("data")]
    private ImagePayload? _data;

    [JsonConstructor]
    private ImagePart() : base("image")
    {
    }

    /// <summary>
    /// 构建一个图片消息段
    /// </summary>
    /// <param name="fileName">文件名</param>
    /// <param name="isFlash">是否为闪照</param>
    /// <param name="useCache">是否使用缓存</param>
    /// <param name="useProxy">是否使用代理</param>
    /// <param name="timeOut">超时时长</param>
    public ImagePart(string fileName, bool isFlash = false, bool useCache = StaticData.CqUseCahceDefault, bool useProxy = StaticData.CqUseProxyDefault, TimeSpan? timeOut = null)
        : base("image", fileName, useCache, useProxy, timeOut)
    {
        IsFlash = isFlash;
    }

    /// <summary>
    /// 构建一个图片消息段
    /// </summary>
    /// <param name="file">文件URL</param>
    /// <param name="isFlash">是否为闪照</param>
    /// <param name="useCache">是否使用缓存</param>
    /// <param name="useProxy">是否使用代理</param>
    /// <param name="timeOut">超时时长</param>
    public ImagePart(Uri file, bool isFlash = false, bool useCache = StaticData.CqUseCahceDefault, bool useProxy = StaticData.CqUseProxyDefault, TimeSpan? timeOut = null)
        : this(file.AbsoluteUri, isFlash, useCache, useProxy, timeOut)
    {
    }

    /// <summary>
    /// 构建一个图片消息段
    /// </summary>
    /// <param name="file">文件内容</param>
    /// <param name="isFlash">是否为闪照</param>
    public ImagePart(byte[] file, bool isFlash)
        : this($"base64://{Convert.ToBase64String(file)}", isFlash)
    {
    }

    /// <inheritdoc/>
    public override string ToString() =>
        "[图片]";

	/// <inheritdoc/>
	public override void OnDeserialized()
    {
        AfterJsonDeserialization(_data!);
        IsFlash = _data?.Type == "flash";
        _data = null;
    }

	/// <inheritdoc/>
	public override void OnSerializing()
    {
        var data = new ImagePayload()
        {
            Type = IsFlash ? "flash" : null
        };
        BeforeJsonSerialization(data);
        _data = data;
    }
}
