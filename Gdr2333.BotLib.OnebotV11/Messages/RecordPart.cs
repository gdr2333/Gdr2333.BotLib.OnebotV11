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

using Gdr2333.BotLib.OnebotV11.Messages.Payload;
using Gdr2333.BotLib.OnebotV11.Utils;
using System.Text.Json.Serialization;

namespace Gdr2333.BotLib.OnebotV11.Messages;

/// <summary>
/// 录音消息段
/// </summary>
public class RecordPart : FilePartBase
{
    /// <summary>
    /// 是否使用变音
    /// </summary>
    [JsonIgnore]
    public bool UseMagic = false;

    [JsonInclude, JsonRequired, JsonPropertyName("data")]
    private RecordPayload? _data;

    [JsonConstructor]
    private RecordPart() : base("record")
    {
    }

    /// <summary>
    /// 构建一个语音消息段
    /// </summary>
    /// <param name="fileName">文件名</param>
    /// <param name="useMagic">是否使用变声</param>
    /// <param name="useCache">是否使用缓存</param>
    /// <param name="useProxy">是否使用代理</param>
    /// <param name="timeOut">超时时长</param>
    public RecordPart(string fileName, bool useMagic = false, bool useCache = StaticData.CqUseCahceDefault, bool useProxy = StaticData.CqUseProxyDefault, TimeSpan? timeOut = null)
        : base("record", fileName, useCache, useProxy, timeOut)
    {
        UseMagic = useMagic;
    }

    /// <summary>
    /// 构建一个语音消息段
    /// </summary>
    /// <param name="file">文件URL</param>
    /// <param name="useMagic">是否使用变声</param>
    /// <param name="useCache">是否使用缓存</param>
    /// <param name="useProxy">是否使用代理</param>
    /// <param name="timeOut">超时时长</param>
    public RecordPart(Uri file, bool useMagic = false, bool useCache = StaticData.CqUseCahceDefault, bool useProxy = StaticData.CqUseProxyDefault, TimeSpan? timeOut = null)
        : this(file.AbsoluteUri, useMagic, useCache, useProxy, timeOut)
    {
    }

    /// <summary>
    /// 构建一个语音消息段
    /// </summary>
    /// <param name="file">文件内容</param>
    /// <param name="useMagic">是否使用变声</param>
    public RecordPart(byte[] file, bool useMagic = false)
        : this($"base64://{Convert.ToBase64String(file)}", useMagic)
    {
    }

    /// <inheritdoc/>
    public override string ToString() =>
        "[语音]";

    /// <inheritdoc/>
    public override void OnDeserialized()
    {
        AfterJsonDeserialization(_data!);
        UseMagic = _data!.UseMagic ?? false;
        _data = null;
    }

    /// <inheritdoc/>
    public override void OnSerializing()
    {
        _data = new() { UseMagic = UseMagic };
        BeforeJsonSerialization(_data!);
    }
}
