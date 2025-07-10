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
using System.Text.Json.Serialization;

namespace Gdr2333.BotLib.OnebotV11.Messages.Parts;

/// <summary>
/// 掷骰子魔法表情*消息段
/// </summary>
/// <remarks>
/// *：原文如此
/// </remarks>
[method: JsonConstructor]
public class DicePart() : MessagePartBase("dice")
{
    /// <inheritdoc/>
    public override string ToString() =>
        "[掷骰子]";

	/// <inheritdoc/>
	public override void OnDeserialized()
    {
    }

	/// <inheritdoc/>
	public override void OnSerializing()
    {
    }
}
