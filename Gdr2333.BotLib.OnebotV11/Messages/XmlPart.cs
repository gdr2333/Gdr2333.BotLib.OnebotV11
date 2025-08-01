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
using System.Xml;

namespace Gdr2333.BotLib.OnebotV11.Messages;

// 虽然反主流，但是说实话，我喜欢XML，特别是跟toml之类的反人类的玩意比
/// <summary>
/// XML消息段
/// </summary>
public class XmlPart : MessagePartBase
{
    [JsonInclude, JsonRequired, JsonPropertyName("data")]
    private string _doc = string.Empty;

    /// <summary>
    /// XML数据
    /// </summary>
    [JsonIgnore]
    public XmlDocument Data { get; set; }

    /// <summary>
    /// 构建一个XML消息段
    /// </summary>
    /// <param name="data">XML数据</param>
    public XmlPart(XmlDocument data) : base("xml")
    {
        Data = data;
    }

    /// <summary>
    /// 构建一个XML消息段
    /// </summary>
    /// <param name="data">XML字符串</param>
    public XmlPart(string data) : base("xml")
    {
        Data = new();
        Data.LoadXml(data);
    }

    /// <inheritdoc/>
    public override void OnDeserialized()
    {
        Data = new();
        Data.LoadXml(_doc);
        _doc = string.Empty;
    }

    /// <inheritdoc/>
    public override void OnSerializing()
    {
        _doc = Data.InnerText;
    }

    /// <inheritdoc/>
    public override string ToString() =>
        "[XML]";
}
