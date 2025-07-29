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
using Gdr2333.BotLib.OnebotV11.Messages.Parts.Base;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace Gdr2333.BotLib.OnebotV11.Messages.Parts;

/// <summary>
/// JSON消息段
/// </summary>
public class JsonPart : MessagePartBase
{
    /// <summary>
    /// JSON数据
    /// </summary>
    [JsonInclude, JsonRequired, JsonPropertyName("data")]
    public JsonNode Data { get; set; }

    [JsonConstructor]
    private JsonPart() : base("json")
    {
    }

    /// <summary>
    /// 构建一个JSON消息段
    /// </summary>
    /// <param name="data">JSON数据</param>
    public JsonPart(JsonNode data) : base("json")
    {
        Data = data;
    }

    /// <summary>
    /// 构建一个JSON消息段
    /// </summary>
    /// <param name="data">XML字符串</param>
    public JsonPart(string data) : base("json")
    {
        Data = JsonNode.Parse(data) ?? throw new FormatException("JSON解析失败");
    }

    /// <inheritdoc/>
    public override void OnDeserialized()
    {
    }

    /// <inheritdoc/>
    public override void OnSerializing()
    {
    }

    /// <inheritdoc/>
    public override string ToString() =>
        "[JSON]";
}
