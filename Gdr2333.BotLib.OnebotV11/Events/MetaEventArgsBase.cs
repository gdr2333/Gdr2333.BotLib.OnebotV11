/*
   Copyright 2025-2026 All contributors of Gdr2333.BotLib

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

using Gdr2333.BotLib.OnebotV11.Utils;
using System.Text.Json.Serialization;

namespace Gdr2333.BotLib.OnebotV11.Events;

/// <summary>
/// 元消息事件数据基类
/// </summary>
[JsonDerivedType(typeof(LifecycleEventArgs))]
[JsonDerivedType(typeof(HeartbeatEventArgs))]
public abstract class MetaEventArgsBase : OnebotV11EventArgsBase
{
    /// <summary>
    /// 元消息类型
    /// </summary>
    [JsonInclude, JsonRequired, JsonPropertyName("meta_event_type")]
    public MetaEventType MetaEventType { get; internal set; } = MetaEventType.Heartbeat;
}

/// <summary>
/// 元消息类型
/// </summary>
[JsonConverter(typeof(MetaEventTypeConverter))]
public enum MetaEventType
{
    /// <summary>
    /// 生命周期通报
    /// </summary>
    Lifecycle,
    /// <summary>
    /// 心跳包
    /// </summary>
    Heartbeat
};

internal sealed class MetaEventTypeConverter : StringEnumJsonConverter<MetaEventType>
{
    public MetaEventTypeConverter() : base(
        fallback: MetaEventType.Heartbeat,
        throwOnUnknown: true,
        mapping: new Dictionary<MetaEventType, string>
        {
            { MetaEventType.Lifecycle, "lifecycle" },
            { MetaEventType.Heartbeat, "heartbeat" }
        })
    {
    }
}