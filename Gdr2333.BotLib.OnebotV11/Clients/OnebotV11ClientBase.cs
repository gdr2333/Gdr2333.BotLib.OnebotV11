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

using Gdr2333.BotLib.OnebotV11.Clients.Result;
using Gdr2333.BotLib.OnebotV11.Data;
using Gdr2333.BotLib.OnebotV11.Events.Base;
using Gdr2333.BotLib.OnebotV11.Messages;
using Gdr2333.BotLib.OnebotV11.Utils;

namespace Gdr2333.BotLib.OnebotV11.Clients;

/// <summary>
/// OnebotV11客户端基类
/// </summary>
public abstract class OnebotV11ClientBase
{
    /// <summary>
    /// 发送私聊消息
    /// </summary>
    /// <param name="userId">目标用户Id</param>
    /// <param name="message">消息内容</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>消息Id</returns>
    public abstract Task<long> SendPrivateMessageAsync(long userId, Message message, CancellationToken? cancellationToken = null);

    /// <summary>
    /// 发送群聊消息
    /// </summary>
    /// <param name="groupId">群Id</param>
    /// <param name="message">消息内容</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>消息Id</returns>
    public abstract Task<long> SendGroupMessage(long groupId, Message message, CancellationToken? cancellationToken = null);

    /// <summary>
    /// 发送消息
    /// </summary>
    /// <param name="userId">目标用户Id</param>
    /// <param name="groupId">目标群Id</param>
    /// <param name="message">消息内容</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>消息Id</returns>
    /// <remarks>
    /// 同时传入<paramref name="groupId"/>和<paramref name="userId"/>会引发System.InvalidOperationException！都不传也一样！
    /// </remarks>
    public abstract Task<long> SendMessage(long? userId, long? groupId, Message message, CancellationToken? cancellationToken = null);

    /// <summary>
    /// 撤回消息
    /// </summary>
    /// <param name="messageId">要撤回的消息Id</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>任务</returns>
    public abstract Task RecallMessageAsync(long messageId, CancellationToken? cancellationToken = null);

    /// <summary>
    /// 获取消息信息
    /// </summary>
    /// <param name="messageId">要获取的消息的Id</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>消息信息</returns>
    public abstract Task<GetMessageResult> GetMessageAsync(long messageId, CancellationToken? cancellationToken = null);

    /// <summary>
    /// 获取转发消息信息
    /// </summary>
    /// <param name="forwardId">转发消息Id</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>转发消息内容</returns>
    public abstract Task<Message> GetForwardMessageAsync(string forwardId, CancellationToken? cancellationToken = null);

    /// <summary>
    /// 给好友点赞
    /// </summary>
    /// <param name="userId">要点赞的好友</param>
    /// <param name="times">点赞次数</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>任务</returns>
    public abstract Task SendLikeAsync(long userId, int times = 1, CancellationToken? cancellationToken = null);

    /// <summary>
    /// 从群聊里踢人
    /// </summary>
    /// <param name="groupId">群聊Id</param>
    /// <param name="userId">要踢的用户Id</param>
    /// <param name="rejectJoinRequests">拒绝别人的加群请求</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>人物</returns>
    public abstract Task DoGroupKickAsync(long groupId, long userId, bool rejectJoinRequests = false, CancellationToken? cancellationToken = null);

    /// <summary>
    /// 从群聊里禁言
    /// </summary>
    /// <param name="groupId">群聊Id</param>
    /// <param name="userId">要禁言的用户Id</param>
    /// <param name="seconds">禁言秒数</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>任务</returns>
    public abstract Task DoGroupBanAsync(long groupId, long userId, int seconds = 1800, CancellationToken? cancellationToken = null);

    /// <summary>
    /// 从群聊里禁言
    /// </summary>
    /// <param name="groupId">群聊Id</param>
    /// <param name="userId">要禁言的用户Id</param>
    /// <param name="duration">禁言时长</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>任务</returns>
    public Task DoGroupBanAsync(long groupId, long userId, TimeSpan duration, CancellationToken? cancellationToken = null) =>
        DoGroupBanAsync(groupId, userId, (int)duration.TotalSeconds, cancellationToken);

    /// <summary>
    /// 从群聊里取消禁言
    /// </summary>
    /// <param name="groupId">群聊Id</param>
    /// <param name="userId">要取消禁言的用户Id</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>任务</returns>
    public Task CancelGroupBanAsync(long groupId, long userId, CancellationToken? cancellationToken = null) =>
        DoGroupBanAsync(groupId, userId, 0, cancellationToken);

    /// <summary>
    /// 执行群聊匿名禁言
    /// </summary>
    /// <param name="groupId">群聊Id</param>
    /// <param name="flag">匿名标识</param>
    /// <param name="seconds">禁言秒数</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>任务</returns>
    [Obsolete(StaticData.AnonymousWarning)]
    public abstract Task DoGroupAnonymousBanAsync(long groupId, string flag, int seconds = 1800, CancellationToken? cancellationToken = null);

    /// <summary>
    /// 执行群聊匿名禁言
    /// </summary>
    /// <param name="groupId">群聊Id</param>
    /// <param name="flag">匿名标识</param>
    /// <param name="duration">禁言时长</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>任务</returns>
    [Obsolete(StaticData.AnonymousWarning)]
    public Task DoGroupAnonymousBanAsync(long groupId, string flag, TimeSpan duration, CancellationToken? cancellationToken = null) =>
        DoGroupAnonymousBanAsync(groupId, flag, (int)duration.TotalSeconds, cancellationToken);

    /// <summary>
    /// 替代函数：执行全群禁言
    /// </summary>
    /// <param name="groupId">群聊Id</param>
    /// <param name="doit">是否执行全群禁言</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>任务</returns>
    protected abstract Task ALT_DoWholeGroupBanAsync(long groupId, bool doit, CancellationToken? cancellationToken = null);

    /// <summary>
    /// 执行全群禁言
    /// </summary>
    /// <param name="groupId">群聊Id</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>任务</returns>
    public Task DoWholeGroupBanAsync(long groupId, CancellationToken? cancellationToken = null) =>
        ALT_DoWholeGroupBanAsync(groupId, true, cancellationToken);

    /// <summary>
    /// 取消全群禁言
    /// </summary>
    /// <param name="groupId">群聊Id</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>任务</returns>
    public Task CancelWholeGroupBanAsync(long groupId, CancellationToken? cancellationToken = null) =>
        ALT_DoWholeGroupBanAsync(groupId, false, cancellationToken);

    /// <summary>
    /// 替代函数：更改管理员状态
    /// </summary>
    /// <param name="groupId">群聊Id</param>
    /// <param name="UserId">要更改的用户Id</param>
    /// <param name="enable">是否使用</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>任务</returns>
    protected abstract Task ALT_SetGroupAdminAsync(long groupId, long UserId, bool enable, CancellationToken? cancellationToken = null);

    /// <summary>
    /// 设置群管理员
    /// </summary>
    /// <param name="groupId">群聊Id</param>
    /// <param name="userId">要设为管理员的用户Id</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>任务</returns>
    public Task SetGroupAdminAsync(long groupId, long userId, CancellationToken? cancellationToken = null) =>
        ALT_SetGroupAdminAsync(groupId, userId, true, cancellationToken);

    /// <summary>
    /// 罢免群管理员
    /// </summary>
    /// <param name="groupId">群聊Id</param>
    /// <param name="userId">要罢免的管理员用户Id</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>任务</returns>
    public Task DismissGroupAdminAsync(long groupId, long userId, CancellationToken? cancellationToken = null) =>
        ALT_SetGroupAdminAsync(groupId, userId, false, cancellationToken);

    /// <summary>
    /// 替代函数：更改群匿名规则
    /// </summary>
    /// <param name="groupId">群聊Id</param>
    /// <param name="enable">是否允许匿名</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>任务</returns>
    [Obsolete(StaticData.AnonymousWarning)]
    protected abstract Task ALT_SetGroupAnonymousAsync(long groupId, bool enable, CancellationToken? cancellationToken = null);

    /// <summary>
    /// 允许匿名发言
    /// </summary>
    /// <param name="groupId">群聊Id</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>任务</returns>
    [Obsolete(StaticData.AnonymousWarning)]
    public Task AllowAnonymousAsync(long groupId, CancellationToken? cancellationToken = null) =>
        ALT_SetGroupAnonymousAsync(groupId, true, cancellationToken);

    /// <summary>
    /// 禁止匿名发言
    /// </summary>
    /// <param name="groupId">群聊Id</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>任务</returns>
    [Obsolete(StaticData.AnonymousWarning)]
    public Task DisallowAnonmyousAsync(long groupId, CancellationToken? cancellationToken = null) =>
        ALT_SetGroupAnonymousAsync(groupId, false, cancellationToken);

    /// <summary>
    /// 更改群聊卡片
    /// </summary>
    /// <param name="groupId">群聊Id</param>
    /// <param name="userId">目标用户Id</param>
    /// <param name="card">卡片内容</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>任务</returns>
    public abstract Task SetGroupCardAsync(long groupId, long userId, string? card, CancellationToken? cancellationToken = null);

    /// <summary>
    /// 删除群聊卡片
    /// </summary>
    /// <param name="groupId">群聊Id</param>
    /// <param name="userId">用户Id</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>任务</returns>
    public Task DeleteGroupCardAsync(long groupId, long userId, CancellationToken? cancellationToken = null) =>
        SetGroupCardAsync(groupId, userId, null, cancellationToken);

    /// <summary>
    /// 更改群名
    /// </summary>
    /// <param name="groupId">群聊Id</param>
    /// <param name="name">新群名</param>
    /// <returns>任务</returns>
    public abstract Task SetGroupNameAsync(long groupId, string name);

    /// <summary>
    /// 退出群聊
    /// </summary>
    /// <param name="groupId">群聊Id</param>
    /// <param name="dismiss">是否解散群聊</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>任务</returns>
    [Obsolete("绝大多数实现不支持解散群聊。")]
    public abstract Task LeaveFromGroupAsync(long groupId, bool dismiss = false, CancellationToken? cancellationToken = null);

    /// <summary>
    /// 退出群聊
    /// </summary>
    /// <param name="groupId">群聊Id</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>任务</returns>
    public Task LeaveFromGroupAsync(long groupId, CancellationToken? cancellationToken = null) =>
        LeaveFromGroupAsync(groupId, false, cancellationToken);

    /// <summary>
    /// 设置群头衔
    /// </summary>
    /// <param name="groupId">群聊Id</param>
    /// <param name="userId">用户Id</param>
    /// <param name="specialTitle">要设置的头衔</param>
    /// <param name="seconds">有效秒数（NTQQ中无效）</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>任务</returns>
    [Obsolete("新版本QQ不支持设置头衔有效时间")]
    public abstract Task SetGroupSpecialTitleAsync(long groupId, long userId, string? specialTitle, int seconds = -1, CancellationToken? cancellationToken = null);

    /// <summary>
    /// 设置群头衔
    /// </summary>
    /// <param name="groupId">群聊Id</param>
    /// <param name="userId">用户Id</param>
    /// <param name="specialTitle">新头衔</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>任务</returns>
    public Task SetGroupSpecialTitleAsync(long groupId, long userId, string specialTitle, CancellationToken? cancellationToken = null) =>
        SetGroupSpecialTitleAsync(groupId, userId, specialTitle, -1, cancellationToken);

    /// <summary>
    /// 删除群头衔
    /// </summary>
    /// <param name="groupId">群聊Id</param>
    /// <param name="userId">用户Id</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>任务</returns>
    public Task DeleteGroupSpecialTitleAsync(long groupId, long userId, CancellationToken? cancellationToken = null) =>
        SetGroupSpecialTitleAsync(groupId, userId, null, -1, cancellationToken);

    /// <summary>
    /// 替代函数：处理加好友请求
    /// </summary>
    /// <param name="flag">请求标识</param>
    /// <param name="approve">是否接受</param>
    /// <param name="remark">备注</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>任务</returns>
    protected abstract Task ALT_ProcessFriendAddRequestAsync(string flag, bool approve = true, string? remark = null, CancellationToken? cancellationToken = null);

    /// <summary>
    /// 接受加好友请求
    /// </summary>
    /// <param name="flag">请求标识</param>
    /// <param name="remark">备注</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>任务</returns>
    public Task AcceptFriendAddRequestAsync(string flag, string? remark = null, CancellationToken? cancellationToken = null) =>
        ALT_ProcessFriendAddRequestAsync(flag, true, remark, cancellationToken);

    /// <summary>
    /// 拒绝加好友请求
    /// </summary>
    /// <param name="flag">请求标识</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>任务</returns>
    public Task RejectFriendAddRequestAsync(string flag, CancellationToken? cancellationToken = null) =>
        ALT_ProcessFriendAddRequestAsync(flag, false, null, cancellationToken);

    /// <summary>
    /// 替代函数：处理加群请求/进群邀请
    /// </summary>
    /// <param name="flag">请求标识</param>
    /// <param name="type">请求类型</param>
    /// <param name="accept">是否接受</param>
    /// <param name="reason">拒绝原因</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>任务</returns>
    protected abstract Task ALT_ProcessGroupAddRequestAsync(string flag, GroupAddRequestType type, bool accept, string? reason, CancellationToken? cancellationToken = null);

    /// <summary>
    /// 接受加群请求
    /// </summary>
    /// <param name="flag">请求标识</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>任务</returns>
    public Task AcceptGroupJoinRequestAsync(string flag, CancellationToken? cancellationToken = null) =>
        ALT_ProcessGroupAddRequestAsync(flag, GroupAddRequestType.Request, true, null, cancellationToken);

    /// <summary>
    /// 拒绝加群请求
    /// </summary>
    /// <param name="flag">请求标识</param>
    /// <param name="reason">拒绝原因</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>任务</returns>
    public Task RejectGroupJoinRequestAsync(string flag, string? reason = null, CancellationToken? cancellationToken = null) =>
        ALT_ProcessGroupAddRequestAsync(flag, GroupAddRequestType.Request, false, reason, cancellationToken);

    /// <summary>
    /// 接受加群邀请
    /// </summary>
    /// <param name="flag">请求标识</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>任务</returns>
    public Task AcceptGroupInviteRequestAsync(string flag, CancellationToken? cancellationToken = null) =>
        ALT_ProcessGroupAddRequestAsync(flag, GroupAddRequestType.Invite, true, null, cancellationToken);

    /// <summary>
    /// 拒绝加群邀请
    /// </summary>
    /// <param name="flag">请求标识</param>
    /// <param name="reason">拒绝原因</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>任务</returns>
    public Task RejectGroupInviteRequestAsync(string flag, string? reason, CancellationToken? cancellationToken = null) =>
        ALT_ProcessGroupAddRequestAsync(flag, GroupAddRequestType.Invite, false, reason, cancellationToken);

    /// <summary>
    /// 获取登录信息
    /// </summary>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>登录信息</returns>
    public abstract Task<GetLoginInfoResult> GetLoginInfoAsync(CancellationToken? cancellationToken = null);

    /// <summary>
    /// 获取陌生人信息
    /// </summary>
    /// <param name="userId">用户Id</param>
    /// <param name="useCache">是否使用缓存</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>用户信息</returns>
    public abstract Task<UserInfo> GetStrangerInfoAsync(long userId, bool useCache = true, CancellationToken? cancellationToken = null);

    /// <summary>
    /// 获取好友列表
    /// </summary>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>好友列表</returns>
    public abstract Task<FriendInfo[]> GetFriendListAsync(CancellationToken? cancellationToken = null);

    /// <summary>
    /// 获取群聊信息
    /// </summary>
    /// <param name="groupId">群聊Id</param>
    /// <param name="useCache">是否使用缓存</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>群聊信息</returns>
    public abstract Task<GroupInfo> GetGroupInfoAsync(long groupId, bool useCache = true, CancellationToken? cancellationToken = null);

    /// <summary>
    /// 获取群聊列表
    /// </summary>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>群聊列表</returns>
    public abstract Task<GroupInfo[]> GetGroupList(CancellationToken? cancellationToken = null);

    /// <summary>
    /// 获取群成员信息
    /// </summary>
    /// <param name="groupId">群聊Id</param>
    /// <param name="userId">用户Id</param>
    /// <param name="useCache">是否使用缓存</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>群成员信息</returns>
    public abstract Task<GroupMemberInfoEx> GetGroupMemberInfoAsync(long groupId, long userId, bool useCache = true, CancellationToken? cancellationToken = null);

    /// <summary>
    /// 获取群成员信息列表
    /// </summary>
    /// <param name="groupId">群聊Id</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>群成员信息列表</returns>
    public abstract Task<GroupMemberInfoEx[]> GetGroupMemberListAsync(long groupId, CancellationToken? cancellationToken = null);

    /// <summary>
    /// 替代函数：获取群荣耀信息
    /// </summary>
    /// <param name="groupId">群聊Id</param>
    /// <param name="type">要获取的群荣耀类型</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>群荣耀信息</returns>
    protected abstract Task<GetGroupHonorInfoResult> ALT_GetGroupHonorInfoAsync(long groupId, string type, CancellationToken? cancellationToken = null);

    /// <summary>
    /// 获取指定类型的群荣耀信息
    /// </summary>
    /// <param name="groupId">群聊Id</param>
    /// <param name="honorType">群荣耀类型</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>群荣耀信息</returns>
    /// <exception cref="InvalidDataException">无法接受的群荣耀类型</exception>
    public Task<GetGroupHonorInfoResult> GetGroupHonorInfoAsync(long groupId, GroupHonorType honorType, CancellationToken? cancellationToken = null) =>
        ALT_GetGroupHonorInfoAsync(groupId, honorType switch
        {
            GroupHonorType.Talkative => "talkative",
            GroupHonorType.Performer => "performer",
            GroupHonorType.Emotion => "emotion",
            GroupHonorType.Legend => "legend",
            GroupHonorType.StrongNewbie => "strong_newbie",
            _ => throw new InvalidDataException(StaticData.BadEnumValueMessage)
        }, cancellationToken);

    /// <summary>
    /// 获取所有群荣耀信息
    /// </summary>
    /// <param name="groupId">群聊Id</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>群荣耀信息</returns>
    public Task<GetGroupHonorInfoResult> GetGroupHonorInfoAsync(long groupId, CancellationToken? cancellationToken = null) =>
        ALT_GetGroupHonorInfoAsync(groupId, "all", cancellationToken);

    /// <summary>
    /// 获取Cookies
    /// </summary>
    /// <param name="domain">要获取Cookies的域名</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>Cookies</returns>
    public abstract Task<string> GetCookiesAsync(string domain, CancellationToken? cancellationToken);

    /// <summary>
    /// 获取CSRF令牌
    /// </summary>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>CSRF令牌</returns>
    public abstract Task<int> GetCsrfTokenAsync(CancellationToken? cancellationToken);

    /// <summary>
    /// 获取机密信息
    /// </summary>
    /// <param name="domain">要获取Cookies的域名</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>机密信息</returns>
    public abstract Task<GetCredentialsResult> GetCredentialsAsync(string? domain = null, CancellationToken? cancellationToken = null);

    /// <summary>
    /// 获取转码后的本地录音文件
    /// </summary>
    /// <param name="file">收到的文件名</param>
    /// <param name="format">输出格式</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>文件信息</returns>
    public abstract Task<FileInfo> GetRecordAsync(string file, string format = "wav", CancellationToken? cancellationToken = null);

    /// <summary>
    /// 获取图片文件
    /// </summary>
    /// <param name="file">收到的文件名</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>文件信息</returns>
    public abstract Task<FileInfo> GetImageAsync(string file, CancellationToken? cancellationToken = null);

    /// <summary>
    /// 检测是否可以发送图片
    /// </summary>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>指示是否可以发送图片</returns>
    public abstract Task<bool> CheckCanSendImageAsync(CancellationToken? cancellationToken);

    /// <summary>
    /// 检测是否可以发送录音
    /// </summary>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>指示是否可以发送语音</returns>
    public abstract Task<bool> CheckCanSendRecordAsync(CancellationToken? cancellationToken);

    /// <summary>
    /// 获取onebot运行状态
    /// </summary>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>运行状态指示</returns>
    public abstract Task<OnebotV11ServerStatus> GetStatusAsync(CancellationToken? cancellationToken);

    /// <summary>
    /// 获取版本信息
    /// </summary>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>版本信息</returns>
    public abstract Task<GetVersionInfoResult> GetVersionInfoAsync(CancellationToken? cancellationToken);

    /// <summary>
    /// 调用重启
    /// </summary>
    /// <param name="msDelay">重启延迟</param>
    /// <param name="cancellationToken">取消令牌（不能取消重启）</param>
    /// <returns>任务</returns>
    public abstract Task DoRestartAsync(int msDelay = 0, CancellationToken? cancellationToken = null);

    /// <summary>
    /// 调用重启
    /// </summary>
    /// <param name="delay">重启延迟</param>
    /// <param name="cancellationToken">取消令牌（不能取消重启）</param>
    /// <returns>任务</returns>
    public Task DoRestartAsync(TimeSpan delay, CancellationToken? cancellationToken = null) =>
        DoRestartAsync((int)delay.TotalMilliseconds, cancellationToken);

    /// <summary>
    /// 调用清理缓存
    /// </summary>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>任务</returns>
    public abstract Task DoCleanCache(CancellationToken? cancellationToken = null);

    /// <summary>
    /// 调用API
    /// </summary>
    /// <param name="apiName">API名称</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>任务</returns>
    public abstract Task CallApiAsync(string apiName, CancellationToken? cancellationToken = null);

    /// <summary>
    /// 调用API
    /// </summary>
    /// <typeparam name="TRequest">请求内容类型</typeparam>
    /// <param name="apiName">API名称</param>
    /// <param name="requestData">请求数据</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>任务</returns>
    public abstract Task CallApiAsync<TRequest>(string apiName, TRequest requestData, CancellationToken? cancellationToken = null);

    /// <summary>
    /// 调用API
    /// </summary>
    /// <typeparam name="TResult">响应内容类型</typeparam>
    /// <param name="apiName">API名称</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>响应数据</returns>
    public abstract Task<TResult> InvoleApiAsync<TResult>(string apiName, CancellationToken? cancellationToken = null);

    /// <summary>
    /// 调用API
    /// </summary>
    /// <typeparam name="TRequest">请求内容类型</typeparam>
    /// <typeparam name="TResult">响应内容类型</typeparam>
    /// <param name="apiName">API名称</param>
    /// <param name="requestData">请求数据</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>响应数据</returns>
    public abstract Task<TResult> InvokeApiAsync<TRequest, TResult>(string apiName, TRequest requestData, CancellationToken? cancellationToken = null);

    /// <summary>
    /// 当接受到Onebot事件时触发的事件
    /// </summary>
    public abstract event EventHandler<OnebotV11EventArgsBase> OnEventOccurrence;
}