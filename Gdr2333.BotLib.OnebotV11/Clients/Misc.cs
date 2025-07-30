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

namespace Gdr2333.BotLib.OnebotV11.Clients;

internal class OnebotV11ApiRequest
{
    [JsonInclude, JsonRequired, JsonPropertyName("action")]
    public required string Action { get; set; }

    [JsonInclude, JsonRequired, JsonPropertyName("echo")]
    public required Guid Guid { get; set; }
}

internal class OnebotV11ApiRequest<T> : OnebotV11ApiRequest
{
    [JsonInclude, JsonRequired, JsonPropertyName("params")]
    public required T Params { get; set; }
}

internal class OnebotV11ApiResult
{
    [JsonInclude, JsonRequired, JsonPropertyName("status")]
    public string Status { get; set; }

    [JsonInclude, JsonRequired, JsonPropertyName("retcode"), JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public int Retcode { get; set; }

    [JsonInclude, JsonPropertyName("data")]
    public JsonElement? Data { get; set; }

    [JsonInclude, JsonRequired, JsonPropertyName("echo")]
    public required Guid Guid { get; set; }

    [JsonInclude, JsonPropertyName("message")]
    public string? ErrorMessage { get; set; }

    [JsonInclude, JsonPropertyName("wording")]
    public string? ErrorMessageEx { get; set; }
}