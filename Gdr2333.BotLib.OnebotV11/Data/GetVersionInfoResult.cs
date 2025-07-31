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

using System.Text.Json;
using System.Text.Json.Serialization;

namespace Gdr2333.BotLib.OnebotV11.Data;

/// <summary>
/// “获取版本”结果
/// </summary>
public class GetVersionInfoResult
{
    /// <summary>
    /// 实现名称
    /// </summary>
    [JsonInclude, JsonRequired, JsonPropertyName("app_name")]
    public string Name { get; internal set; }

    /// <summary>
    /// 实现版本
    /// </summary>
    [JsonInclude, JsonRequired, JsonPropertyName("app_version")]
    public string Version { get; internal set; }

    /// <summary>
    /// 协议版本
    /// </summary>
    [JsonInclude, JsonRequired, JsonPropertyName("protocol_version")]
    public string ProtocolVersion { get; internal set; }

    /// <summary>
    /// 其他信息
    /// </summary>
    [JsonExtensionData]
    public Dictionary<string, JsonElement> Others { get; internal set; }
}