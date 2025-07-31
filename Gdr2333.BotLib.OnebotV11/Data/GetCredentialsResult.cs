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
/// 获取客户端机密信息结果
/// </summary>
public class GetCredentialsResult
{
    /// <summary>
    /// 对该网站的cookies
    /// </summary>
    [JsonInclude, JsonPropertyName("cookies")]
    public string? Cookies { get; internal set; }

    /// <summary>
    /// csrf令牌
    /// </summary>
    [JsonInclude, JsonPropertyName("csrf_token"), JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public int? CsrfToken { get; internal set; }
}