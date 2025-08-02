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
/// 表情消息段
/// </summary>
public class FacePart : MessagePartBase
{
    /// <summary>
    /// 表情ID
    /// </summary>
    [JsonIgnore]
    public int FaceId { get; set; }

    [JsonInclude, JsonRequired, JsonPropertyName("data")]
    private Int32IdPayload? _data;

    [JsonConstructor]
    private FacePart() : base("face")
    {
        FaceId = 0;
    }

    /// <summary>
    /// 使用一个表情ID构造一个表情消息段
    /// </summary>
    /// <param name="faceId">表情ID</param>
    public FacePart(int faceId) : base("face")
    {
        FaceId = faceId;
    }

    /// <inheritdoc/>
    public override string ToString() =>
        "[表情]";

    /// <inheritdoc/>
    public override void OnDeserialized()
    {
        FaceId = _data!.Id;
        _data = null;
    }

    /// <inheritdoc/>
    public override void OnSerializing() => _data = new() { Id = FaceId };
}
