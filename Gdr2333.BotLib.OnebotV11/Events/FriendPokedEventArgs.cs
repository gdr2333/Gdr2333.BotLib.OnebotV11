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

namespace Gdr2333.BotLib.OnebotV11.Events;

/// <summary>
/// 好友戳一戳事件
/// </summary>
public class FriendPokedEventArgs : PokedEventArgsBase
{
    /// <summary>
    /// 发送者 QQ 号
    /// </summary>
    /// <remarks>
    /// 你问我这玩意跟<c>UserId</c>有什么不一样？我不到啊，<a href="https://napneko.github.io/onebot/event#%E6%88%B3%E4%B8%80%E6%88%B3%E9%80%9A%E7%9F%A5">文档</a>这么写的
    /// </remarks>
    [JsonInclude, JsonPropertyName("sender_id")]
    public long? SenderId { get; internal set; } = null;
}