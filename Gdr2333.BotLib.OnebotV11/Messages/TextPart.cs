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

using Gdr2333.BotLib.OnebotV11.Messages.Payload;
using System.Text.Json.Serialization;

namespace Gdr2333.BotLib.OnebotV11.Messages;

/// <summary>
/// 文本消息段
/// </summary>
public class TextPart : MessagePartBase
{
    [JsonInclude, JsonRequired, JsonPropertyName("data")]
    private TextPartPayload? _data = null;

    /// <summary>
    /// 消息段内容
    /// </summary>
    [JsonIgnore]
    public string Text { get; set; } = "";

    /// <summary>
    /// 构造一个空的文本消息段
    /// </summary>
    [JsonConstructor]
    public TextPart() : base("text")
    {
    }

    /// <summary>
    /// 使用指定文本构造一个文本消息段
    /// </summary>
    /// <param name="text">消息段内容</param>
    public TextPart(string text) : base("text")
    {
        Text = text;
    }

    /// <inheritdoc/>
    public override void OnDeserialized()
    {
        Text = _data!.Text;
        _data = null;
    }

    /// <inheritdoc/>
    public override void OnSerializing() =>
        _data = new() { Text = Text };

    /// <summary>
    /// 返回消息段的文本表现形式
    /// </summary>
    /// <returns>文本表现形式</returns>
    public override string ToString() =>
        Text;
}
