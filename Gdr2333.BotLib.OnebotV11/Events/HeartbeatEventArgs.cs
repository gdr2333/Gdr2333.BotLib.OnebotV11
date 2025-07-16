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

using System.Text.Json.Serialization;
using Gdr2333.BotLib.OnebotV11.Data;
using Gdr2333.BotLib.OnebotV11.Events.Base;
using Gdr2333.BotLib.OnebotV11.Utils;

namespace Gdr2333.BotLib.OnebotV11.Events;

/// <summary>
/// 心跳事件参数
/// </summary>
public class HeartbeatEventArgs : MetaEventArgsBase
{
    /// <summary>
    /// 服务端状态
    /// </summary>
    [JsonInclude, JsonRequired, JsonPropertyName("status")]
    public OnebotV11ServerStatus Status { get; internal set; }

    /// <summary>
    /// 心跳预期间隔
    /// </summary>
    [JsonInclude, JsonRequired, JsonPropertyName("interval"), JsonConverter(typeof(MillisecondToTimeSpanConverter))]
    public TimeSpan Interval { get; internal set; }
}