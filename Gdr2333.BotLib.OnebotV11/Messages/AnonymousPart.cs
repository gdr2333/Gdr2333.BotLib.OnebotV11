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
using Gdr2333.BotLib.OnebotV11.Utils;
using System.Text.Json.Serialization;

namespace Gdr2333.BotLib.OnebotV11.Messages;

/// <summary>
/// [弃用的]匿名消息段
/// </summary>
/// <param name="ignore">无法匿名时是否继续发送</param>
[Obsolete(StaticData.AnonymousWarning)]
[method: Obsolete(StaticData.AnonymousWarning)]
public class AnonymousPart(bool ignore) : MessagePartBase
{
    /// <summary>
    /// 无法匿名时是否继续发送
    /// </summary>
    [JsonIgnore]
    public bool Ignore { get; set; } = ignore;

    [JsonInclude, JsonPropertyName("data")]
    private AnonymousPayload? _data;

    /// <summary>
    /// 创建一个匿名消息段
    /// </summary>
    [Obsolete("anonymous消息段在几乎所有实现中都不再有用。我仍然按标准实现了它，但你不该用。")]
    [JsonConstructor]
    public AnonymousPart() : this(false)
    {
    }

    /// <inheritdoc/>
    public override string ToString() =>
        string.Empty;

    /// <inheritdoc/>
    public override void OnDeserialized()
    {
        Ignore = _data?.Ignore ?? false;
        _data = null;
    }

    /// <inheritdoc/>
    public override void OnSerializing() =>
        _data = new()
        {
            Ignore = Ignore
        };
}
