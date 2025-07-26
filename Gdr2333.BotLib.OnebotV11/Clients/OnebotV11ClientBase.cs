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
using Gdr2333.BotLib.OnebotV11.Messages;
using Gdr2333.BotLib.OnebotV11.Utils;

namespace Gdr2333.BotLib.OnebotV11.Clients;

public abstract class OnebotV11ClientBase
{
    public abstract Task<long> SendPrivateMessageAsync(long userId, Message message, CancellationToken? cancellationToken = null);

    public abstract Task<long> SendGroupMessage(long groupId, Message message, CancellationToken? cancellationToken = null);

    public abstract Task<long> SendMessage(long? userId, long? groupId, Message message, CancellationToken? cancellationToken = null);

    public abstract Task RecallMessageAsync(long messageId, CancellationToken? cancellationToken = null);

    public abstract Task<GetMessageResult> GetMessageAsync(long messageId, CancellationToken? cancellationToken = null);

    public abstract Task<Message> GetForwardMessageAsync(string forwardId, CancellationToken? cancellationToken = null);

    public abstract Task SendLikeAsync(long userId, int times = 1, CancellationToken? cancellationToken = null);

    public abstract Task DoGroupKickAsync(long groupId, long userId, bool rejectJoinRequests = false, CancellationToken? cancellationToken = null);

    public abstract Task DoGroupBanAsync(long groupId, long userId, int seconds = 1800, CancellationToken? cancellationToken = null);

    public Task DoGroupBanAsync(long groupId, long userId, TimeSpan duration, CancellationToken? cancellationToken = null) =>
        DoGroupBanAsync(groupId, userId, (int)duration.TotalSeconds, cancellationToken);

    public Task CancelGroupBanAsync(long groupId, long userId, CancellationToken? cancellationToken = null) =>
        DoGroupBanAsync(groupId, userId, 0, cancellationToken);

    [Obsolete(StaticData.AnonymousWarning)]
    public abstract Task DoGroupAnonymousBanAsync(long groupId, string flag, int seconds = 1800, CancellationToken? cancellationToken = null);

    [Obsolete(StaticData.AnonymousWarning)]
    public Task DoGroupAnonymousBanAsync(long groupId, string flag, TimeSpan duration, CancellationToken? cancellationToken = null) =>
        DoGroupAnonymousBanAsync(groupId, flag, (int)duration.TotalSeconds, cancellationToken);

    protected abstract Task ALT_DoWholeGroupBanAsync(long groupId, bool doit, CancellationToken? cancellationToken = null);

    public Task DoWholeGroupBanAsync(long groupId, CancellationToken? cancellationToken = null) =>
        ALT_DoWholeGroupBanAsync(groupId, true, cancellationToken);

    public Task CancelWholeGroupBanAsync(long groupId, CancellationToken? cancellationToken = null) =>
        ALT_DoWholeGroupBanAsync(groupId, false, cancellationToken);

    protected abstract Task ALT_SetGroupAdminAsync(long groupId, long UserId, bool enable, CancellationToken? cancellationToken = null);

    public Task SetGroupAdminAsync(long groupId, long userId, CancellationToken? cancellationToken = null) =>
        ALT_SetGroupAdminAsync(groupId, userId, true, cancellationToken);

    public Task DismissGroupAdminAsync(long groupId, long userId, CancellationToken? cancellationToken = null) =>
        ALT_SetGroupAdminAsync(groupId, userId, false, cancellationToken);

    [Obsolete(StaticData.AnonymousWarning)]
    protected abstract Task ALT_SetGroupAnonymousAsync(long groupId, bool enable, CancellationToken? cancellationToken = null);

    [Obsolete(StaticData.AnonymousWarning)]
    public Task AllowAnonymousAsync(long groupId, CancellationToken? cancellationToken = null) =>
        ALT_SetGroupAnonymousAsync(groupId, true, cancellationToken);

    [Obsolete(StaticData.AnonymousWarning)]
    public Task DisallowAnonmyousAsync(long groupId, CancellationToken? cancellationToken = null) =>
        ALT_SetGroupAnonymousAsync(groupId, false, cancellationToken);

    public abstract Task SetGroupCardAsync(long groupId, long userId, string? card, CancellationToken? cancellationToken = null);

    public Task DeleteGroupCardAsync(long groupId, long userId, CancellationToken? cancellationToken = null) =>
        SetGroupCardAsync(groupId, userId, null, cancellationToken);

    public abstract Task SetGroupNameAsync(long groupId, string name);

    [Obsolete("绝大多数实现不支持解散群聊。")]
    public abstract Task LeaveFromGroupAsync(long groupId, bool dismiss = false, CancellationToken? cancellationToken = null);

    public Task LeaveFromGroupAsync(long groupId, CancellationToken? cancellationToken = null) =>
        LeaveFromGroupAsync(groupId, false, cancellationToken);

    [Obsolete("新版本QQ不支持设置头衔有效时间")]
    public abstract Task SetGroupSpecialTitleAsync(long groupId, long userId, string? specialTitle, int seconds = -1, CancellationToken? cancellationToken = null);

    public Task SetGroupSpecialTitleAsync(long groupId, long userId, string specialTitle, CancellationToken? cancellationToken = null) =>
        SetGroupSpecialTitleAsync(groupId, userId, specialTitle, -1, cancellationToken);

    public Task DeleteGroupSpecialTitleAsync(long groupId, long userId, CancellationToken? cancellationToken = null) =>
        SetGroupSpecialTitleAsync(groupId, userId, null, -1, cancellationToken);

    protected abstract Task ALT_ProcessFriendAddRequestAsync(string flag, bool approve = true, string? remark = null, CancellationToken? cancellationToken = null);

    public Task AcceptFriendAddRequestAsync(string flag, string? remark = null, CancellationToken? cancellationToken = null) =>
        ALT_ProcessFriendAddRequestAsync(flag, true, remark, cancellationToken);

    public Task RejectFriendAddRequestAsync(string flag, CancellationToken? cancellationToken = null) =>
        ALT_ProcessFriendAddRequestAsync(flag, false, null, cancellationToken);

    protected abstract Task ALT_ProcessGroupAddRequestAsync(string flag, GroupAddRequestType type, bool accept, string? reason, CancellationToken? cancellationToken = null);

    public Task AcceptGroupJoinRequestAsync(string flag, CancellationToken? cancellationToken = null) =>
        ALT_ProcessGroupAddRequestAsync(flag, GroupAddRequestType.Request, true, null, cancellationToken);

    public Task RejectGroupJoinRequestAsync(string flag, string? reason = null, CancellationToken? cancellationToken = null) =>
        ALT_ProcessGroupAddRequestAsync(flag, GroupAddRequestType.Request, false, reason, cancellationToken);

    public Task AcceptGroupInviteRequestAsync(string flag, CancellationToken? cancellationToken = null) =>
        ALT_ProcessGroupAddRequestAsync(flag, GroupAddRequestType.Invite, true, null, cancellationToken);

    public Task RejectGroupInviteRequestAsync(string flag, string? reason, CancellationToken? cancellationToken = null) =>
        ALT_ProcessGroupAddRequestAsync(flag, GroupAddRequestType.Invite, false, reason, cancellationToken);

    public abstract Task<GetLoginInfoResult> GetLoginInfoAsync(CancellationToken? cancellationToken = null);

    public abstract Task<UserInfo> GetStrangerInfoAsync(long userId, bool useCache = true, CancellationToken? cancellationToken = null);

    public abstract Task<FriendInfo[]> GetFriendListAsync(CancellationToken? cancellationToken = null);

    public abstract Task<GroupInfo> GetGroupInfoAsync(long groupId, bool useCache = true, CancellationToken? cancellationToken = null);

    public abstract Task<GroupInfo[]> GetGroupList(CancellationToken? cancellationToken = null);

    public abstract Task<GroupMemberInfoEx> GetGroupMemberInfoAsync(long groupId, long userId, bool useCache = true, CancellationToken? cancellationToken = null);

    public abstract Task<GroupMemberInfoEx[]> GetGroupMemberListAsync(long groupId, CancellationToken? cancellationToken = null);

    protected abstract Task<GetGroupHonorInfoResult> ALT_GetGroupHonorInfoAsync(long groupId, string type, CancellationToken? cancellationToken = null);

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

    public Task<GetGroupHonorInfoResult> GetGroupHonorInfoAsync(long groupId, CancellationToken? cancellationToken = null) =>
        ALT_GetGroupHonorInfoAsync(groupId, "all", cancellationToken);

    public abstract Task<string> GetCookiesAsync(string domain, CancellationToken? cancellationToken);

    public abstract Task<int> GetCsrfTokenAsync(CancellationToken? cancellationToken);

    public abstract Task<GetCredentialsResult> GetCredentialsAsync(string? domain = null, CancellationToken? cancellationToken = null);

    public abstract Task<FileInfo> GetRecordAsync(string file, string format = "wav", CancellationToken? cancellationToken = null);

    public abstract Task<FileInfo> GetImageAsync(string file, CancellationToken? cancellationToken = null);

    public abstract Task<bool> CheckCanSendImageAsync(CancellationToken? cancellationToken);

    public abstract Task<bool> CheckCanSendRecordAsync(CancellationToken? cancellationToken);

    public abstract Task<OnebotV11ServerStatus> GetStatusAsync(CancellationToken? cancellationToken);

    public abstract Task<GetVersionInfoResult> GetVersionInfoAsync(CancellationToken? cancellationToken);

    public abstract Task DoRestartAsync(int msDelay = 0, CancellationToken? cancellationToken = null);

    public Task DoRestartAsync(TimeSpan delay, CancellationToken? cancellationToken = null) =>
        DoRestartAsync((int)delay.TotalMilliseconds, cancellationToken);

    public abstract Task DoCleanCache(CancellationToken? cancellationToken = null);
}