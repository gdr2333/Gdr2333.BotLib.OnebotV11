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

using Gdr2333.BotLib.OnebotV11.Utils;
using System.Text.Json.Serialization;

namespace Gdr2333.BotLib.OnebotV11.Messages.Parts.Payload;

/// <summary>
/// 文件负载类
/// </summary>
internal class FilePayload
{
    /// <summary>
    /// 文件名
    /// </summary>
    [JsonInclude, JsonRequired, JsonPropertyName("file")]
    public string File { get; set; } = string.Empty;

    /// <summary>
    /// 文件URL
    /// </summary>
    // JsonRequired实际上是JsonRequiredWhenDeserializaion。
    [JsonInclude, JsonRequired, JsonPropertyName("url")]
    public Uri? Url { get; set; }

    /// <summary>
    /// （发送时）是否使用缓存
    /// </summary>
    [JsonInclude, JsonPropertyName("cache"), JsonConverter(typeof(OB11JsonBoolConverter))]
    public bool? UseCache { get; set; }

    /// <summary>
    /// （发送时）是否使用代理
    /// </summary>
    [JsonInclude, JsonPropertyName("proxy"), JsonConverter(typeof (OB11JsonBoolConverter))]
    public bool? UseProxy { get; set; }

    /// <summary>
    /// （发送时）超时时间0
    /// </summary>
    [JsonInclude, JsonPropertyName("timeout"), JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public int? TimeOut { get; set; } = null;
}
