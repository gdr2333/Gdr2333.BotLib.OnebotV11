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

using System.Text.Json.Serialization;

namespace Gdr2333.BotLib.OnebotV11.Data;

/// <summary>
/// 群文件信息
/// </summary>
public class GroupFileInfo
{
    /// <summary>
    /// 文件Id
    /// </summary>
    [JsonInclude, JsonRequired, JsonPropertyName("id")]
    public string Id { get; internal set; }

    /// <summary>
    /// 文件名
    /// </summary>
    [JsonInclude, JsonRequired, JsonPropertyName("name")]
    public string Name { get; internal set; }

    /// <summary>
    /// 文件大小
    /// </summary>
    [JsonInclude, JsonRequired, JsonPropertyName("size"), JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public long Size { get; internal set; }

    /// <summary>
    /// 鬼知道有什么用的一个玩意
    /// </summary>
    [JsonInclude, JsonPropertyName("busid"), JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public long? BusId { get; internal set; }
}