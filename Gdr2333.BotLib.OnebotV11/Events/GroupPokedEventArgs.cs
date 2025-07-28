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
using Gdr2333.BotLib.OnebotV11.Events.Base;
using Gdr2333.BotLib.OnebotV11.Events.Interfaces;

namespace Gdr2333.BotLib.OnebotV11.Events;

/// <summary>
/// 群内戳一戳事件参数
/// </summary>
public class GroupPokedEventArgs : NotifyEventArgsBase, IUserEventArgs, IGroupEventArgs
{
    /// <summary>
    /// 群Id
    /// </summary>
    [JsonInclude, JsonRequired, JsonPropertyName("group_id"), JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public long GroupId { get; internal set; }

    /// <summary>
    /// 发出戳一戳的用户Id
    /// </summary>
    /// <remarks>
    /// 请注意：该字段与IUserEventArgs.UserId不同，后者实际上是本类中的TargetId字段。
    /// </remarks>
    [JsonInclude, JsonRequired, JsonPropertyName("user_id"), JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public long UserId { get; internal set; }

    /// <summary>
    /// 目标用户Id
    /// </summary>
    [JsonInclude, JsonRequired, JsonPropertyName("target_id"), JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public long TargetId { get; internal set; }

    long IUserEventArgs.UserId => TargetId;
}