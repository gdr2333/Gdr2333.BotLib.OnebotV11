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

namespace Gdr2333.BotLib.OnebotV11.Messages.Payload;

internal class CustomForwardNodePayload
{
    [JsonInclude, JsonPropertyName("user_id"), JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public long UserId { get; set; }

    [JsonInclude, JsonPropertyName("nickname")]
    public string NickName { get; set; } = string.Empty;

    [JsonInclude, JsonRequired, JsonPropertyName("content")]
    public required Message Content { get; set; }
}
