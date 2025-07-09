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
using Gdr2333.BotLib.OnebotV11.Message.Parts.Base;
using Gdr2333.BotLib.OnebotV11.Message.Parts.Payload;
using System.Text.Json.Serialization;

namespace Gdr2333.BotLib.OnebotV11.Message.Parts;

/// <summary>
/// 回复消息段
/// </summary>
public class ReplyPart : MessagePartBase
{
    /// <summary>
    /// 要回复的消息ID
    /// </summary>
    [JsonIgnore]
    public long ReplyId { get; set; }

    [JsonInclude, JsonRequired, JsonPropertyName("data")]
    private Int64IdPayload? _data;

    [JsonConstructor]
    private ReplyPart() : base("reply")
    {
    }

    /// <summary>
    /// 构造一个回复消息段
    /// </summary>
    /// <param name="id">要回复的消息ID</param>
    public ReplyPart(long id) : base("reply")
    {
        ReplyId = id;
    }

    /// <inheritdoc/>
    public override void OnDeserialized()
    {
        ReplyId = _data!.Id;
        _data = null;
    }

    /// <inheritdoc/>
    public override void OnSerializing()
    {
        _data = new() { Id = ReplyId };
    }

    /// <inheritdoc/>
    public override string ToString() =>
        string.Empty;
}
