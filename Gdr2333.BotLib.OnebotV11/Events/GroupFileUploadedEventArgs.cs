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

namespace Gdr2333.BotLib.OnebotV11.Events;

/// <summary>
/// 群文件上传事件
/// </summary>
public class GroupFileUploadedEventArgs : NoticeEventArgsBase, IUserEventArgs, IGroupEventArgs
{
    [JsonConstructor]
    internal GroupFileUploadedEventArgs(long groupId, long userId, GroupFileInfo fileInfo)
    {
        GroupId = groupId;
        UserId = userId;
        FileInfo = fileInfo;
    }

    /// <summary>
    /// 群ID
    /// </summary>
    [JsonInclude, JsonRequired, JsonPropertyName("group_id")]
    public long GroupId { get; internal set; }

    /// <summary>
    /// 用户ID
    /// </summary>
    [JsonInclude, JsonRequired, JsonPropertyName("user_id")]
    public long UserId { get; internal set; }

    /// <summary>
    /// 文件信息
    /// </summary>
    [JsonInclude, JsonRequired, JsonPropertyName("file")]
    public GroupFileInfo FileInfo { get; internal set; }
}