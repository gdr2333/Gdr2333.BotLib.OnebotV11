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

[method: JsonConstructor]
internal class CustomMusicSharePayload(Uri url, Uri audioUrl, string title, string? content, Uri? imageUrl) : MusicSharePayloadBase
{
    [JsonInclude, JsonRequired, JsonPropertyName("url")]
    public Uri Url { get; set; } = url;

    [JsonInclude, JsonRequired, JsonPropertyName("audio")]
    public Uri AudioUrl { get; set; } = audioUrl;

    [JsonInclude, JsonRequired, JsonPropertyName("title")]
    public string Title { get; set; } = title;

    [JsonInclude, JsonPropertyName("content")]
    public string? Content { get; set; } = content;

    [JsonInclude, JsonPropertyName("image")]
    public Uri? ImageUrl { get; set; } = imageUrl;
}
