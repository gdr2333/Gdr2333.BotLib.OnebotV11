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
/// 服务器状态上报数据
/// </summary>
public class OnebotV11ServerStatus
{
    /// <summary>
    /// 服务器当前是否在线
    /// </summary>
    [JsonInclude, JsonRequired, JsonPropertyName("online")]
    public bool? IsOnline { get; internal set; }

    /// <summary>
    /// 服务器是否正常工作
    /// </summary>
    [JsonInclude, JsonRequired, JsonPropertyName("good")]
    public bool EverythingIsFine { get; internal set; }

    /// <summary>
    /// 其他信息
    /// </summary>
    [JsonExtensionData]
    public Dictionary<string, JsonElement>? More { get; set; }
}