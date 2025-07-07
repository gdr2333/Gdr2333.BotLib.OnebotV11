using Gdr2333.BotLib.OnebotV11.Message.Parts.Payload;
using System.Text.Json.Serialization;

namespace Gdr2333.BotLib.OnebotV11.Message.Parts.Base;

/// <summary>
/// 好友/群邀请消息段
/// </summary>
public abstract class ContactPartBase : MessagePartBase
{
    /// <summary>
    /// 邀请类型
    /// </summary>
    [JsonIgnore]
    public string ContactType { get; set; }

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
    protected void AfterJsonDeserialization(ContactPayload data)
    {
        ContactType = data.Type;
        Id = data.Id;
    }

    /// <summary>
    /// JSON序列化前的内部钩子
    /// </summary>
    /// <param name="data">邀请相关数据</param>
    protected void BeforeJsonSerialization(out ContactPayload data)
    {
        data = new()
        {
            Id = Id,
            Type = ContactType
        };
    }
}
