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

// 我写这玩意单纯是为了简化开发，应该没人闲的没事用这个吧......

/// <summary>
/// 有关文件的消息段基类
/// </summary>
public abstract class FilePartBase : MessagePartBase
{
    /// <summary>
    /// 文件名
    /// </summary>
    [JsonIgnore]
    public string FileName { get; set; }

    /// <summary>
    /// 文件路径
    /// </summary>
    [JsonIgnore]
    public Uri? Url { get; set; }

    /// <summary>
    /// 是否使用缓存
    /// </summary>
    [JsonIgnore]
    public bool UseCache { get; set; }

    /// <summary>
    /// 是否使用代理
    /// </summary>
    [JsonIgnore]
    public bool UseProxy { get; set; }

    /// <summary>
    /// 超时时长
    /// </summary>
    [JsonIgnore]
    public TimeSpan? TimeOut { get; set; }

    /// <summary>
    /// 用于JSON构建的基类构建参数
    /// </summary>
    [JsonConstructor]
    protected FilePartBase(string type) : base()
    {
        FileName = "";
    }

    /// <summary>
    /// 基类构建函数
    /// </summary>
    /// <param name="type">消息段类型</param>
    /// <param name="fileName">要发送的文件名</param>
    /// <param name="useProxy">是否使用代理发送</param>
    /// <param name="useCache">是否使用缓存发送</param>
    /// <param name="timeOut">超时时长设置（null=不超时）</param>
    public FilePartBase(string type, string fileName, bool useCache = StaticData.CqUseCahceDefault, bool useProxy = StaticData.CqUseProxyDefault, TimeSpan? timeOut = null) : base()
    {
        FileName = fileName;
        UseProxy = useProxy;
        UseCache = useCache;
        TimeOut = timeOut;
    }

    /// <summary>
    /// JSON反序列化后的内部钩子
    /// </summary>
    /// <param name="data">文件相关数据</param>
    internal void AfterJsonDeserialization(FilePayload data)
    {
        FileName = data.File;
        Url = data.Url;
        UseProxy = StaticData.CqUseProxyDefault;
        UseCache = StaticData.CqUseCahceDefault;
        TimeOut = null;
    }

    /// <summary>
    /// JSON序列化前的内部钩子
    /// </summary>
    /// <param name="data">文件相关数据</param>
    internal void BeforeJsonSerialization(FilePayload data)
    {
        data.File = FileName;
        data.Url = Url;
        data.UseProxy = UseProxy;
        data.UseCache = UseCache;
        data.TimeOut = (int?)(TimeOut?.TotalSeconds);
    }
}
