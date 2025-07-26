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
using Gdr2333.BotLib.OnebotV11.Messages;
using Gdr2333.BotLib.OnebotV11.Utils;

namespace Gdr2333.BotLib.OnebotV11.Clients.Result;

public class GetMessageResult
{
    [JsonInclude, JsonRequired, JsonPropertyName("time"), JsonConverter(typeof(UnixTimeToDateTimeConverter))]
    public DateTime SendTime { get; internal set; }

    [JsonInclude, JsonRequired, JsonPropertyName("message_type")]
    public MessageType MessageType { get; internal set; }

    [JsonInclude, JsonRequired, JsonPropertyName("message_id"), JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public long MessageId { get; internal set; }

    [JsonInclude, JsonPropertyName("real_id"), JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public long? RealId { get; internal set; }

    [JsonInclude, JsonRequired, JsonPropertyName("sender"), JsonConverter(typeof(AutoDecisionUserInfoConverter))]
    public UserInfo Sender { get; internal set; }

    [JsonInclude, JsonRequired, JsonPropertyName("message")]
    public Message Messsage { get; internal set; }
}