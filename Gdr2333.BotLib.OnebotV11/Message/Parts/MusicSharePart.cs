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
/// （预定义）音乐分享消息段
/// </summary>
public class MusicSharePart : MusicSharePartBase
{
    /// <summary>
    /// 音乐ID
    /// </summary>
    [JsonIgnore]
    public long MusicId { get; set; }

    [JsonInclude, JsonRequired, JsonPropertyName("data")]
    private MusicSharePayload? _data;

    [JsonConstructor]
    private MusicSharePart()
    {
    }

    /// <summary>
    /// 新建一个音乐分享消息段
    /// </summary>
    /// <param name="type">平台类型</param>
    /// <param name="id">音乐ID</param>
    public MusicSharePart(string type, long id)
    {
        MusicShareType = type;
        MusicId = id;
    }

    /// <inheritdoc/>
    public override void OnDeserialized()
    {
        MusicShareType = _data.Type;
        MusicId = _data.Id;
        _data = null;
    }

    /// <inheritdoc/>
    public override void OnSerializing()
    {
        _data = new()
        {
            Type = MusicShareType,
            Id = MusicId
        };
    }

    /// <inheritdoc/>
    public override string ToString() =>
        $"[分享音乐{MusicId}来自{MusicShareType}]";
}
