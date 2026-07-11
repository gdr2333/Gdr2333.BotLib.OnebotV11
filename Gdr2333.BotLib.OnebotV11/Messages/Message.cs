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
using Gdr2333.BotLib.OnebotV11.Messages.TmpAlt;
using Gdr2333.BotLib.OnebotV11.Utils;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

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
    /// 构造一条空消息。仅由反序列化器内部使用。
    /// </summary>
    internal Message() : base()
    {
    }

    /// <summary>
    /// 用指定文本初始化一条文本消息
    /// </summary>
    /// <param name="text">消息中的文本</param>
    public Message(string text) : this([new TextPart(text)])
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
    public override Message? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String)
        {
            // CQ-code 字符串：解码为 JsonNode 走统一段循环
            var cqNode = CqCodeToJsonNode.Convert(reader.GetString()!);
            if (cqNode is not JsonArray cqArr)
                return new Message();
            return DeserializeSegments(cqArr, options);
        }
        if (reader.TokenType == JsonTokenType.StartArray)
        {
            // 已经是 JSON 数组：用 JsonDocument 拿到 JsonArray
            using var doc = JsonDocument.ParseValue(ref reader);
            if (doc.RootElement.ValueKind != JsonValueKind.Array)
                return new Message();
            var arr = JsonNode.Parse(doc.RootElement.GetRawText()) as JsonArray;
            if (arr is null)
                return new Message();
            return DeserializeSegments(arr, options);
        }
        throw new FormatException("这什么消息段编码方式？");
    }

    private static Message DeserializeSegments(JsonArray arr, JsonSerializerOptions options)
    {
        var result = new Message();
        // 单段反序列化失败时跳过该段，保留其余段；不再让坏段炸整条消息。
        // 坏掉的段会用 TextPart 保留原始 JSON 文本，便于上游排查；无法获得原文时直接丢弃。
        foreach (var element in arr)
        {
            if (element is null)
                continue;
            var raw = element.ToJsonString();
            try
            {
                var part = JsonSerializer.Deserialize<MessagePartBase>(raw, options);
                if (part is null)
                    throw new JsonException("消息段反序列化为 null。");
                result.Add(part is TmpAlt.ContactPartAlt alt ? alt.GetRealPart() : part);
            }
            catch (Exception ex) when (ex is JsonException || ex is FormatException || ex is InvalidDataException)
            {
                result.Add(new TextPart($"[无法解读的消息段：{raw}]"));
            }
        }
        return result;
    }

    public override void Write(Utf8JsonWriter writer, Message value, JsonSerializerOptions options) =>
        JsonSerializer.Serialize<List<MessagePartBase>>(writer, value);
}