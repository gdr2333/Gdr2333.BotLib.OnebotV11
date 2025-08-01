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
/// 合并转发消息段
/// </summary>
public class ForwardMessagePart : MessagePartBase
{
    /// <summary>
    /// 合并转发Id
    /// </summary>
    [JsonIgnore]
    public long ForwardId { get; set; }

    [JsonInclude, JsonRequired, JsonPropertyName("data")]
    private Int64IdPayload? _data;

    [JsonConstructor]
    private ForwardMessagePart() : base("forward")
    {
    }

    /// <summary>
    /// 构造一个合并转发消息段
    /// </summary>
    /// <param name="id"></param>
    [Obsolete("标准没允许发这个消息段，这不在支持范围内。目前我没打算单独支持各种扩展，使用后果自负。")]
    public ForwardMessagePart(long id) : base("forward")
    {
        ForwardId = id;
    }

    /// <inheritdoc/>
    public override void OnDeserialized()
    {
        ForwardId = _data!.Id;
        _data = null;
    }

    /// <inheritdoc/>
    public override void OnSerializing()
    {
        _data = new()
        {
            Id = ForwardId,
        };
    }

    /// <inheritdoc/>
    public override string ToString() =>
        "转发消息";
}
