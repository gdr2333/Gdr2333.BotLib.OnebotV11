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
/// “获取登录信息”结果
/// </summary>
public class GetLoginInfoResult
{
    [JsonConstructor]
    internal GetLoginInfoResult(long userId, string nickname)
    {
        UserId = userId;
        Nickname = nickname;
    }

    /// <summary>
    /// 当前账号Id
    /// </summary>
    [JsonInclude, JsonRequired, JsonPropertyName("user_id"), JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public long UserId { get; internal set; }

    /// <summary>
    /// 当前账号昵称
    /// </summary>
    [JsonInclude, JsonRequired, JsonPropertyName("nickname")]
    public string Nickname { get; internal set; }
}