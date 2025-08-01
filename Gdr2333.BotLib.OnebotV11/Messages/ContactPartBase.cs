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
/// 好友/群邀请消息段
/// </summary>
public abstract class ContactPartBase : MessagePartBase
{
    /// <summary>
    /// 邀请类型
    /// </summary>
    [JsonIgnore]
    public string ContactType { get; set; } = string.Empty;

    /// <summary>
    /// 邀请目标ID
    /// </summary>
    [JsonIgnore]
    public long Id { get; set; }

    /// <summary>
    /// JSON用初始化函数
    /// </summary>
    [JsonConstructor]
    protected ContactPartBase() : base("contact")
    {
    }

    /// <summary>
    /// 新建一个好友/群邀请消息段（子类JSON序列化用）
    /// </summary>
    /// <param name="contactType">邀请类型</param>
    protected ContactPartBase(string contactType) : base("contact")
    {
        ContactType = contactType;
    }

    /// <summary>
    /// 新建一个好友/群邀请消息段
    /// </summary>
    /// <param name="contactType">邀请类型</param>
    /// <param name="id">邀请目标ID</param>
    public ContactPartBase(string contactType, long id) : base("contact")
    {
        ContactType = contactType;
        Id = id;
    }

    /// <summary>
    /// JSON反序列化后的内部钩子
    /// </summary>
    /// <param name="data">邀请相关数据</param>
    internal void AfterJsonDeserialization(ContactPayload data)
    {
        ContactType = data.Type;
        Id = data.Id;
    }

    /// <summary>
    /// JSON序列化前的内部钩子
    /// </summary>
    /// <param name="data">邀请相关数据</param>
    internal void BeforeJsonSerialization(out ContactPayload data)
    {
        data = new()
        {
            Id = Id,
            Type = ContactType
        };
    }
}
