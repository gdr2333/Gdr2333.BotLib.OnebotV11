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
using Gdr2333.BotLib.OnebotV11.Messages.Parts.Payload;
using System.Text.Json.Serialization;

namespace Gdr2333.BotLib.OnebotV11.Messages.Parts;

/// <summary>
/// 自定义转发消息节点
/// </summary>
public class CustomForwardNodePart : MessagePartBase
{
    /// <summary>
    /// 发送者UID
    /// </summary>
    [JsonIgnore]
    public long UserId { get; set; }

    /// <summary>
    /// 发送者昵称
    /// </summary>
    [JsonIgnore]
    public string NickName { get; set; } = string.Empty;

    /// <summary>
    /// 转发的内容
    /// </summary>
    [JsonIgnore]
    public required Message Content { get; set; }

    [JsonInclude, JsonRequired, JsonPropertyName("data")]
    private CustomForwardNodePayload? _data;

    [JsonConstructor]
    private CustomForwardNodePart() : base("node")
    {
    }

    /// <summary>
    /// 构造一个自定义转发消息节点
    /// </summary>
    /// <param name="message">转发的内容</param>
    /// <param name="nickName">发送者昵称</param>
    /// <param name="uid">发送者UID</param>
    public CustomForwardNodePart(Message message, string nickName, long uid) : base("node")
    {
        UserId = uid;
        NickName = nickName;
        Content = message;
    }

    /// <inheritdoc/>
    public override void OnDeserialized()
    {
        UserId = _data!.UserId;
        NickName = _data!.NickName;
        Content = _data!.Content;
        _data = null;
    }

    /// <inheritdoc/>
    public override void OnSerializing()
    {
        _data = new()
        {
            Content = _data!.Content,
            NickName = NickName,
            UserId = UserId
        };
    }

    /// <inheritdoc/>
    public override string ToString() =>
        $"{NickName}：{Content}";
}
