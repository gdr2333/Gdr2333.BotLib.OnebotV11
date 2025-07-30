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

    #region 沟槽的请求和响应类型
    private struct MessageIdData
    {
        [JsonInclude, JsonRequired, JsonPropertyName("message_id"), JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
        public required long MessageId;
    }

    private struct DoGroupEnableRequest
    {
        [JsonInclude, JsonRequired, JsonPropertyName("group_id"), JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
        public required long GroupId;

        [JsonInclude, JsonRequired, JsonPropertyName("enable"), JsonConverter(typeof(OB11JsonBoolConverter))]
        public required bool Enable;
    }

    private struct FileData
    {
        [JsonInclude, JsonRequired, JsonPropertyName("file")]
        public required string File;
    }

    private struct CheckStatusResult
    {
        [JsonInclude, JsonRequired, JsonPropertyName("yes"), JsonConverter(typeof(OB11JsonBoolConverter))]
        public bool YesWeCan;
    }

    private struct SendPrivateMessageRequest
    {
        [JsonInclude, JsonRequired, JsonPropertyName("user_id"), JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
        public required long UserId;

        [JsonInclude, JsonRequired, JsonPropertyName("message")]
        public required Message Message;
    }

    private struct SendGroupMessageRequest
    {
        [JsonInclude, JsonRequired, JsonPropertyName("group_id"), JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
        public required long GroupId;

        [JsonInclude, JsonRequired, JsonPropertyName("message")]
        public required Message Message;
    }

    private struct GetForwardMessageRequest
    {
        [JsonInclude, JsonRequired, JsonPropertyName("id")]
        public required string Id;
    }

    private struct GetForwardMessageResult
    {
        [JsonInclude, JsonRequired, JsonPropertyName("message")]
        public required Message Message;
    }

    private struct SendLikeRequest
    {
        [JsonInclude, JsonRequired, JsonPropertyName("user_id"), JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
        public required long UserId;

        [JsonInclude, JsonRequired, JsonPropertyName("times"), JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
        public required int Times;
    }

    private struct DoGroupKickRequest
    {
        [JsonInclude, JsonRequired, JsonPropertyName("group_id"), JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
        public required long GroupId;

        [JsonInclude, JsonRequired, JsonPropertyName("user_id"), JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
        public required long UserId;

        [JsonInclude, JsonRequired, JsonPropertyName("reject_add_request"), JsonConverter(typeof(OB11JsonBoolConverter))]
        public required bool RejectJoinRequests;
    }

    private struct DoGroupBanRequest
    {
        [JsonInclude, JsonRequired, JsonPropertyName("group_id"), JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
        public required long GroupId;

        [JsonInclude, JsonRequired, JsonPropertyName("user_id"), JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
        public required long UserId;

        [JsonInclude, JsonRequired, JsonPropertyName("duration"), JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
        public required long Seconds;
    }

    private struct DoGroupAnonymousBanRequest
    {
        [JsonInclude, JsonRequired, JsonPropertyName("group_id"), JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
        public required long GroupId;

        [JsonInclude, JsonRequired, JsonPropertyName("anonymous_flag")]
        public required string Flag;

        [JsonInclude, JsonRequired, JsonPropertyName("duration"), JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
        public required long Seconds;
    }

    private struct ALT_SetGroupAdminRequest
    {
        [JsonInclude, JsonRequired, JsonPropertyName("group_id"), JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
        public required long GroupId;

        [JsonInclude, JsonRequired, JsonPropertyName("user_id"), JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
        public required long UserId;

        [JsonInclude, JsonRequired, JsonPropertyName("enable"), JsonConverter(typeof(OB11JsonBoolConverter))]
        public required bool Enable;
    }

    private struct SetGroupCardRequest
    {
        [JsonInclude, JsonRequired, JsonPropertyName("group_id"), JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
        public required long GroupId;

        [JsonInclude, JsonRequired, JsonPropertyName("user_id"), JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
        public required long UserId;

        [JsonInclude, JsonPropertyName("card")]
        public required string? Card;
    }

    private struct SetGroupNameRequest
    {
        [JsonInclude, JsonRequired, JsonPropertyName("group_id"), JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
        public required long GroupId;

        [JsonInclude, JsonRequired, JsonPropertyName("group_name")]
        public required string Name;
    }

    private struct LeaveFromGroupRequest
    {
        [JsonInclude, JsonRequired, JsonPropertyName("group_id"), JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
        public required long GroupId;

        [JsonInclude, JsonRequired, JsonPropertyName("is_dismiss"), JsonConverter(typeof(OB11JsonBoolConverter))]
        public required bool Dismiss;
    }

    private struct SetGroupSpecialTitleRequest
    {
        [JsonInclude, JsonRequired, JsonPropertyName("group_id"), JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
        public required long GroupId;

        [JsonInclude, JsonRequired, JsonPropertyName("user_id"), JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
        public required long UserId;

        [JsonInclude, JsonPropertyName("special_title")]
        public required string? SpecialTitle;

        [JsonInclude, JsonRequired, JsonPropertyName("duration"), JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
        public required long Seconds;
    }

    private struct ALT_ProcessFriendAddRequestRequest
    {
        [JsonInclude, JsonRequired, JsonPropertyName("flag")]
        public required string Flag;

        [JsonInclude, JsonRequired, JsonPropertyName("approve"), JsonConverter(typeof(OB11JsonBoolConverter))]
        public required bool Approve;

        [JsonInclude, JsonPropertyName("remark")]
        public required string? Remark;
    }

    private struct ALT_ProcessGroupAddRequestRequest
    {
        [JsonInclude, JsonRequired, JsonPropertyName("flag")]
        public required string Flag;

        [JsonInclude, JsonRequired, JsonPropertyName("sub_type")]
        public required GroupAddRequestType Subtype;

        [JsonInclude, JsonRequired, JsonPropertyName("approve"), JsonConverter(typeof(OB11JsonBoolConverter))]
        public required bool Approve;

        [JsonInclude, JsonPropertyName("reason")]
        public required string? Reason;
    }

    private struct GetStrangerInfoRequest
    {
        [JsonInclude, JsonRequired, JsonPropertyName("user_id"), JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
        public required long UserId;

        [JsonInclude, JsonRequired, JsonPropertyName("no_cache"), JsonConverter(typeof(OB11JsonBoolConverter))]
        public required bool NoCache;
    }

    private struct GetGroupInfoRequest
    {
        [JsonInclude, JsonRequired, JsonPropertyName("group_id"), JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
        public required long GroupId;

        [JsonInclude, JsonRequired, JsonPropertyName("no_cache"), JsonConverter(typeof(OB11JsonBoolConverter))]
        public required bool NoCache;
    }

    private struct GetGroupMemberInfoRequest
    {
        [JsonInclude, JsonRequired, JsonPropertyName("group_id"), JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
        public required long GroupId;

        [JsonInclude, JsonRequired, JsonPropertyName("user_id"), JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
        public required long UserId;

        [JsonInclude, JsonRequired, JsonPropertyName("no_cache"), JsonConverter(typeof(OB11JsonBoolConverter))]
        public required bool NoCache;
    }

    private struct GetGroupMemberListRequest
    {
        [JsonInclude, JsonRequired, JsonPropertyName("group_id"), JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
        public required long GroupId;
    }

    private struct ALT_GetGroupHonorInfoRequest
    {
        [JsonInclude, JsonRequired, JsonPropertyName("group_id"), JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
        public required long GroupId;

        [JsonInclude, JsonRequired, JsonPropertyName("type")]
        public required string Type;
    }

    private struct GetCookiesRequest
    {
        [JsonInclude, JsonRequired, JsonPropertyName("domain")]
        public required string Domain;
    }

    private struct GetCookiesResult
    {
        [JsonInclude, JsonRequired, JsonPropertyName("cookies")]
        public required string Cookies;
    }

    private struct GetCsrfTokenResult
    {
        [JsonInclude, JsonRequired, JsonPropertyName("token"), JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
        public int Token;
    }

    private struct GetCredentialsRequest
    {
        [JsonInclude, JsonPropertyName("domain")]
        public required string? Domain;
    }

    private struct GetRecordRequest
    {
        [JsonInclude, JsonRequired, JsonPropertyName("file")]
        public required string File;

        [JsonInclude, JsonRequired, JsonPropertyName("out_format")]
        public required string Format;
    }

    private struct DoRestartRequest
    {
        [JsonInclude, JsonRequired, JsonPropertyName("delay"), JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
        public required int MsDelay;
    }
    #endregion
    
    /// <summary>
    /// 发送私聊消息
    /// </summary>
    /// <param name="userId">目标用户Id</param>
    /// <param name="message">消息内容</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>消息Id</returns>
    public async Task<long> SendPrivateMessageAsync(long userId, Message message, CancellationToken? cancellationToken = null) =>
        (await InvokeApiAsync<SendPrivateMessageRequest, MessageIdData>("send_private_msg", new() { UserId = userId, Message = message }, cancellationToken)).MessageId;

    /// <summary>
    /// 发送群聊消息
    /// </summary>
    /// <param name="groupId">群Id</param>
    /// <param name="message">消息内容</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>消息Id</returns>
    public async Task<long> SendGroupMessageAsync(long groupId, Message message, CancellationToken? cancellationToken = null) =>
        (await InvokeApiAsync<SendGroupMessageRequest, MessageIdData>("send_group_msg", new() { GroupId = groupId, Message = message }, cancellationToken)).MessageId;

    /// <summary>
    /// 发送消息
    /// </summary>
    /// <param name="userId">目标用户Id</param>
    /// <param name="groupId">目标群Id</param>
    /// <param name="message">消息内容</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>消息Id</returns>
    /// <remarks>
    /// 实际上我没调send_msg你信吗？
    /// </remarks>
    public Task<long> SendMessage(long? userId, long? groupId, Message message, CancellationToken? cancellationToken = null) =>
        userId.HasValue ? SendPrivateMessageAsync(userId.Value, message, cancellationToken) :
        groupId.HasValue ? SendGroupMessageAsync(groupId.Value, message, cancellationToken) :
        throw new ArgumentNullException($"{nameof(userId)}和{nameof(groupId)}都是{null}！");

    /// <summary>
    /// 撤回消息
    /// </summary>
    /// <param name="messageId">要撤回的消息Id</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>任务</returns>
    public Task RecallMessageAsync(long messageId, CancellationToken? cancellationToken = null) =>
        CallApiAsync<MessageIdData>("delete_msg", new() { MessageId = messageId }, cancellationToken);

    /// <summary>
    /// 获取消息信息
    /// </summary>
    /// <param name="messageId">要获取的消息的Id</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>消息信息</returns>
    public Task<GetMessageResult> GetMessageAsync(long messageId, CancellationToken? cancellationToken = null) =>
        InvokeApiAsync<MessageIdData, GetMessageResult>("get_msg", new() { MessageId = messageId }, cancellationToken);

    /// <summary>
    /// 获取转发消息信息
    /// </summary>
    /// <param name="forwardId">转发消息Id</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>转发消息内容</returns>
    public async Task<Message> GetForwardMessageAsync(string forwardId, CancellationToken? cancellationToken = null) =>
        (await InvokeApiAsync<GetForwardMessageRequest, GetForwardMessageResult>("get_forward_msg", new() { Id = forwardId }, cancellationToken)).Message;

    /// <summary>
    /// 给好友点赞
    /// </summary>
    /// <param name="userId">要点赞的好友</param>
    /// <param name="times">点赞次数</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>任务</returns>
    public Task SendLikeAsync(long userId, int times = 1, CancellationToken? cancellationToken = null) =>
        CallApiAsync<SendLikeRequest>("send_like", new() { UserId = userId, Times = times }, cancellationToken);

    /// <summary>
    /// 从群聊里踢人
    /// </summary>
    /// <param name="groupId">群聊Id</param>
    /// <param name="userId">要踢的用户Id</param>
    /// <param name="rejectJoinRequests">拒绝别人的加群请求</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>任务</returns>
    public Task DoGroupKickAsync(long groupId, long userId, bool rejectJoinRequests = false, CancellationToken? cancellationToken = null) =>
        CallApiAsync<DoGroupKickRequest>("set_group_kick", new() { GroupId = groupId, UserId = userId, RejectJoinRequests = rejectJoinRequests }, cancellationToken);

    /// <summary>
    /// 从群聊里禁言
    /// </summary>
    /// <param name="groupId">群聊Id</param>
    /// <param name="userId">要禁言的用户Id</param>
    /// <param name="seconds">禁言秒数</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>任务</returns>
    public Task DoGroupBanAsync(long groupId, long userId, int seconds = 1800, CancellationToken? cancellationToken = null) =>
        CallApiAsync<DoGroupBanRequest>("set_group_ban", new() { GroupId = groupId, UserId = userId, Seconds = seconds }, cancellationToken);

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
    public Task DoGroupAnonymousBanAsync(long groupId, string flag, int seconds = 1800, CancellationToken? cancellationToken = null) =>
        CallApiAsync<DoGroupAnonymousBanRequest>("set_group_anonymous_ban", new() { GroupId = groupId, Flag = flag, Seconds = seconds }, cancellationToken);

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
    protected Task ALT_DoWholeGroupBanAsync(long groupId, bool doit, CancellationToken? cancellationToken = null) =>
        CallApiAsync<DoGroupEnableRequest>("set_group_whole_ban", new() { GroupId = groupId, Enable = doit }, cancellationToken);

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
    protected Task ALT_SetGroupAdminAsync(long groupId, long UserId, bool enable, CancellationToken? cancellationToken = null) =>
        CallApiAsync<ALT_SetGroupAdminRequest>("set_group_admin", new() { GroupId = groupId, UserId = UserId, Enable = enable }, cancellationToken);

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
    protected Task ALT_SetGroupAnonymousAsync(long groupId, bool enable, CancellationToken? cancellationToken = null) =>
        CallApiAsync<DoGroupEnableRequest>("set_group_anonymous", new() { GroupId = groupId, Enable = enable }, cancellationToken);

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
    public Task SetGroupCardAsync(long groupId, long userId, string? card, CancellationToken? cancellationToken = null) =>
        CallApiAsync<SetGroupCardRequest>("set_group_card", new() { GroupId = groupId, UserId = userId, Card = card }, cancellationToken);

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
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>任务</returns>
    public Task SetGroupNameAsync(long groupId, string name, CancellationToken? cancellationToken = null) =>
        CallApiAsync<SetGroupNameRequest>("set_group_name", new() { GroupId = groupId, Name = name }, cancellationToken);

    /// <summary>
    /// 退出群聊
    /// </summary>
    /// <param name="groupId">群聊Id</param>
    /// <param name="dismiss">是否解散群聊</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>任务</returns>
    [Obsolete("绝大多数实现不支持解散群聊。")]
    public Task LeaveFromGroupAsync(long groupId, bool dismiss, CancellationToken? cancellationToken = null) =>
        CallApiAsync<LeaveFromGroupRequest>("set_group_leave", new() { GroupId = groupId, Dismiss = dismiss }, cancellationToken);

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
    public Task SetGroupSpecialTitleAsync(long groupId, long userId, string? specialTitle, int seconds, CancellationToken? cancellationToken = null) =>
        CallApiAsync<SetGroupSpecialTitleRequest>("set_group_special_title", new() { GroupId = groupId, UserId = userId, SpecialTitle = specialTitle, Seconds = seconds }, cancellationToken);

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
    protected Task ALT_ProcessFriendAddRequestAsync(string flag, bool approve = true, string? remark = null, CancellationToken? cancellationToken = null) =>
        CallApiAsync<ALT_ProcessFriendAddRequestRequest>("set_friend_add_request", new() { Flag = flag, Approve = approve, Remark = remark }, cancellationToken);

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
    protected Task ALT_ProcessGroupAddRequestAsync(string flag, GroupAddRequestType type, bool accept, string? reason, CancellationToken? cancellationToken = null) =>
        CallApiAsync<ALT_ProcessGroupAddRequestRequest>("set_group_add_request", new() { Flag = flag, Subtype = type, Approve = accept, Reason = reason }, cancellationToken);

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
    public Task<GetLoginInfoResult> GetLoginInfoAsync(CancellationToken? cancellationToken = null) =>
        InvokeApiAsync<GetLoginInfoResult>("get_login_info", cancellationToken);

    /// <summary>
    /// 获取陌生人信息
    /// </summary>
    /// <param name="userId">用户Id</param>
    /// <param name="useCache">是否使用缓存</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>用户信息</returns>
    public Task<UserInfo> GetStrangerInfoAsync(long userId, bool useCache = true, CancellationToken? cancellationToken = null) =>
        InvokeApiAsync<GetStrangerInfoRequest, UserInfo>("get_stranger_info", new() { UserId = userId, NoCache = !useCache }, cancellationToken);

    /// <summary>
    /// 获取好友列表
    /// </summary>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>好友列表</returns>
    public Task<FriendInfo[]> GetFriendListAsync(CancellationToken? cancellationToken = null) =>
        InvokeApiAsync<FriendInfo[]>("get_friend_list", cancellationToken);

    /// <summary>
    /// 获取群聊信息
    /// </summary>
    /// <param name="groupId">群聊Id</param>
    /// <param name="useCache">是否使用缓存</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>群聊信息</returns>
    public Task<GroupInfo> GetGroupInfoAsync(long groupId, bool useCache = true, CancellationToken? cancellationToken = null) =>
        InvokeApiAsync<GetGroupInfoRequest, GroupInfo>("get_group_info", new() { GroupId = groupId, NoCache = !useCache }, cancellationToken);

    /// <summary>
    /// 获取群聊列表
    /// </summary>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>群聊列表</returns>
    public Task<GroupInfo[]> GetGroupList(CancellationToken? cancellationToken = null) =>
        InvokeApiAsync<GroupInfo[]>("get_group_list", cancellationToken);

    /// <summary>
    /// 获取群成员信息
    /// </summary>
    /// <param name="groupId">群聊Id</param>
    /// <param name="userId">用户Id</param>
    /// <param name="useCache">是否使用缓存</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>群成员信息</returns>
    public Task<GroupMemberInfoEx> GetGroupMemberInfoAsync(long groupId, long userId, bool useCache = true, CancellationToken? cancellationToken = null) =>
        InvokeApiAsync<GetGroupMemberInfoRequest, GroupMemberInfoEx>("get_group_member_info", new() { GroupId = groupId, UserId = userId, NoCache = !useCache }, cancellationToken);

    /// <summary>
    /// 获取群成员信息列表
    /// </summary>
    /// <param name="groupId">群聊Id</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>群成员信息列表</returns>
    public Task<GroupMemberInfoEx[]> GetGroupMemberListAsync(long groupId, CancellationToken? cancellationToken = null) =>
        InvokeApiAsync<GetGroupMemberListRequest, GroupMemberInfoEx[]>("get_group_member_list", new() { GroupId = groupId }, cancellationToken);

    /// <summary>
    /// 替代函数：获取群荣耀信息
    /// </summary>
    /// <param name="groupId">群聊Id</param>
    /// <param name="type">要获取的群荣耀类型</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>群荣耀信息</returns>
    protected Task<GetGroupHonorInfoResult> ALT_GetGroupHonorInfoAsync(long groupId, string type, CancellationToken? cancellationToken = null) =>
        InvokeApiAsync<ALT_GetGroupHonorInfoRequest, GetGroupHonorInfoResult>("get_group_honor_info", new() { GroupId = groupId, Type = type }, cancellationToken);

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
    public async Task<string> GetCookiesAsync(string domain, CancellationToken? cancellationToken) =>
        (await InvokeApiAsync<GetCookiesRequest, GetCookiesResult>("get_cookies", new() { Domain = domain }, cancellationToken)).Cookies;

    /// <summary>
    /// 获取CSRF令牌
    /// </summary>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>CSRF令牌</returns>
    public async Task<int> GetCsrfTokenAsync(CancellationToken? cancellationToken) =>
        (await InvokeApiAsync<GetCsrfTokenResult>("get_csrf_token", cancellationToken)).Token;

    /// <summary>
    /// 获取机密信息
    /// </summary>
    /// <param name="domain">要获取Cookies的域名</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>机密信息</returns>
    public Task<GetCredentialsResult> GetCredentialsAsync(string? domain = null, CancellationToken? cancellationToken = null) =>
        InvokeApiAsync<GetCredentialsRequest, GetCredentialsResult>("get_credentials", new() { Domain = domain }, cancellationToken);

    /// <summary>
    /// 获取转码后的本地录音文件
    /// </summary>
    /// <param name="file">收到的文件名</param>
    /// <param name="format">输出格式</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>文件信息</returns>
    public async Task<FileInfo> GetRecordAsync(string file, string format = "wav", CancellationToken? cancellationToken = null) =>
        new((await InvokeApiAsync<GetRecordRequest, FileData>("get_record", new() { File = file, Format = format }, cancellationToken)).File);

    /// <summary>
    /// 获取图片文件
    /// </summary>
    /// <param name="file">收到的文件名</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>文件信息</returns>
    public async Task<FileInfo> GetImageAsync(string file, CancellationToken? cancellationToken = null) =>
        new((await InvokeApiAsync<FileData, FileData>("get_image", new() { File = file }, cancellationToken)).File);

    /// <summary>
    /// 检测是否可以发送图片
    /// </summary>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>指示是否可以发送图片</returns>
    public async Task<bool> CheckCanSendImageAsync(CancellationToken? cancellationToken) =>
        (await InvokeApiAsync<CheckStatusResult>("can_send_image", cancellationToken)).YesWeCan;

    /// <summary>
    /// 检测是否可以发送录音
    /// </summary>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>指示是否可以发送语音</returns>
    public async Task<bool> CheckCanSendRecordAsync(CancellationToken? cancellationToken) =>
        (await InvokeApiAsync<CheckStatusResult>("can_send_record", cancellationToken)).YesWeCan;

    /// <summary>
    /// 获取onebot运行状态
    /// </summary>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>运行状态指示</returns>
    public Task<OnebotV11ServerStatus> GetStatusAsync(CancellationToken? cancellationToken) =>
        InvokeApiAsync<OnebotV11ServerStatus>("get_status", cancellationToken);

    /// <summary>
    /// 获取版本信息
    /// </summary>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>版本信息</returns>
    public Task<GetVersionInfoResult> GetVersionInfoAsync(CancellationToken? cancellationToken) =>
        InvokeApiAsync<GetVersionInfoResult>("get_version_info", cancellationToken);

    /// <summary>
    /// 调用重启
    /// </summary>
    /// <param name="msDelay">重启延迟</param>
    /// <param name="cancellationToken">取消令牌（不能取消重启）</param>
    /// <returns>任务</returns>
    public Task DoRestartAsync(int msDelay = 0, CancellationToken? cancellationToken = null) =>
        CallApiAsync<DoRestartRequest>("set_restart", new() { MsDelay = msDelay }, cancellationToken);

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
    public Task DoCleanCache(CancellationToken? cancellationToken = null) =>
        CallApiAsync("clean_cache", cancellationToken);

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
    public abstract Task<TResult> InvokeApiAsync<TResult>(string apiName, CancellationToken? cancellationToken = null);

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
    /// <param name="sender">接收到消息的客户端</param>
    /// <param name="e">事件内容</param>
    public delegate void OnebotEventOccurrence(OnebotV11ClientBase sender, OnebotV11EventArgsBase e);

    /// <summary>
    /// 当接受到Onebot事件时触发的事件
    /// </summary>
    public abstract event OnebotEventOccurrence? OnEventOccurrence;

    /// <summary>
    /// 当事件接收器出现异常时触发的事件
    /// </summary>
    /// <param name="sender">接收到消息的客户端</param>
    /// <param name="e">异常内容</param>
    public delegate void OnExceptionInLoop(OnebotV11ClientBase sender, Exception e);

    /// <summary>
    /// 当事件接收器出现异常时触发的事件
    /// </summary>
    public abstract event OnExceptionInLoop OnExceptionOccurrence;
}