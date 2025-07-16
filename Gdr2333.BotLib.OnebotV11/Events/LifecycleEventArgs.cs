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

using System.Text.Json;
using System.Text.Json.Serialization;
using Gdr2333.BotLib.OnebotV11.Events.Base;

/// <summary>
/// 生命周期通报事件参数
/// </summary>
public class LifecycleEventArgs : MetaEventArgsBase
{
    /// <summary>
    /// 生命周期通报子类型
    /// </summary>
    [JsonInclude, JsonRequired, JsonPropertyName("sub_type")]
    public LifecycleEventSubType Subtype { get; internal set; }
}

/// <summary>
/// 生命周期通报子类型
/// </summary>
[JsonConverter(typeof(LifecycleEventSubTypeConverter))]
public enum LifecycleEventSubType
{
    /// <summary>
    /// 启用（HTTP）
    /// </summary>
    Enable,
    /// <summary>
    /// 禁用（HTTP）
    /// </summary>
    Disable,
    /// <summary>
    /// 已连接（Websocket）
    /// </summary>
    Connect
}

internal class LifecycleEventSubTypeConverter : JsonConverter<LifecycleEventSubType>
{
    public override LifecycleEventSubType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
        reader.GetString()?.ToLower() switch
        {
            "enable" => LifecycleEventSubType.Enable,
            "disable" => LifecycleEventSubType.Disable,
            "connect" => LifecycleEventSubType.Connect,
            _ => throw new InvalidCastException("未知枚举类型！")
        };

    public override void Write(Utf8JsonWriter writer, LifecycleEventSubType value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value switch
        {
            LifecycleEventSubType.Enable => "enable",
            LifecycleEventSubType.Disable => "disable",
            LifecycleEventSubType.Connect => "connect",
            _ => throw new InvalidDataException("未知枚举类型！")
        });
    }
}