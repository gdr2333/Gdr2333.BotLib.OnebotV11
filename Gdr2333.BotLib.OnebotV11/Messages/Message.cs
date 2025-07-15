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
using Gdr2333.BotLib.OnebotV11.Messages.Parts.Base;
using Gdr2333.BotLib.OnebotV11.Utils;

namespace Gdr2333.BotLib.OnebotV11.Messages;

/// <summary>
/// 消息类
/// </summary>
[JsonConverter(typeof(MessageConverter))]
public class Message : List<MessagePartBase>
{
    /// <summary>
    /// 使用一些消息段初始化一条消息
    /// </summary>
    /// <param name="parts">消息段</param>
    public Message(IEnumerable<MessagePartBase> parts) : base(parts)
    {
    }

    /// <summary>
    /// 使用一个消息段初始化一条消息
    /// </summary>
    /// <param name="part">消息段</param>
    public Message(MessagePartBase part) : base([part])
    {
    }

    /// <summary>
    /// 初始化一个具有指定容量的消息
    /// </summary>
    /// <param name="capacity">容量</param>
    public Message(int capacity) : base(capacity)
    {
    }

    /// <summary>
    /// 将消息转为文本表现形式
    /// </summary>
    /// <returns>消息文本</returns>
    public override string ToString() =>
        // LINQ的奇怪用法增加了
        string.Concat(from part in this select part.ToString());
}

internal class MessageConverter : JsonConverter<Message>
{
    public override Message? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
        new(reader.TokenType switch
        {
            JsonTokenType.StartArray => JsonSerializer.Deserialize<MessagePartBase[]>(ref reader, options) ?? throw new InvalidDataException("消息段解码失败！"),
            JsonTokenType.String => CqCodeToJsonNode.Convert(reader.GetString()!).Deserialize<MessagePartBase[]>() ?? throw new FormatException("消息段无法解码！"),
            _ => throw new FormatException("这什么消息段编码方式？")
        });

    public override void Write(Utf8JsonWriter writer, Message value, JsonSerializerOptions options) =>
        JsonSerializer.Serialize<List<MessagePartBase>>(writer, value);
}