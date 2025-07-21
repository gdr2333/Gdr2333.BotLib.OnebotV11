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
using Gdr2333.BotLib.OnebotV11.Utils;

namespace Gdr2333.BotLib.OnebotV11.Events.Data;

/// <summary>
/// 匿名信息
/// </summary>
[Obsolete(StaticData.AnonymousWarning)]
public class AnonymousInfo
{
    /// <summary>
    /// 匿名ID
    /// </summary>
    [JsonInclude, JsonRequired, JsonPropertyName("id"), JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public long Id { get; internal set; }

    /// <summary>
    /// 匿名后的名称
    /// </summary>
    [JsonInclude, JsonRequired, JsonPropertyName("name")]
    public string Name { get; internal set; } = string.Empty;

    /// <summary>
    /// 匿名用户flag
    /// </summary>
    [JsonInclude, JsonRequired, JsonPropertyName("flag")]
    public string Flag { get; internal set; } = string.Empty;
}