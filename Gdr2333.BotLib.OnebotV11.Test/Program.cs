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

using Gdr2333.BotLib.OnebotV11.Clients;
using Gdr2333.BotLib.OnebotV11.Messages.Parts;
using System.Text;

OnebotV11ClientBase client;

Uri url;
string? ak = "";
bool human = false;
bool rws = false;

if (!File.Exists("AutoTest.txt"))
{
    Console.WriteLine("是否使用反向Websocket链接？是输入Y。");
    rws = Console.ReadLine().Equals("Y", StringComparison.CurrentCultureIgnoreCase);
    Console.WriteLine("请输入目标URL：");
    url = new(Console.ReadLine());
    Console.WriteLine("请输入目标AccessToken，没有则留空：");
    ak = Console.ReadLine();
    human = true;
}
else
{
    var lines = File.ReadAllLines("AutoTest.txt");
    rws = lines[0].Equals("Y", StringComparison.CurrentCultureIgnoreCase);
    url = new(lines[1]);
    if (lines.Length > 2)
        ak = lines[2];
}

if (!rws)
{
    var wsclient = new WebSocketClient(url, 5, ak);
    await wsclient.LinkAsync();
    client = wsclient;
}
else
{
    var rwsclient = new ReverseWebSocketClient(url, ak);
    rwsclient.Start();
    client = rwsclient;
}
client.OnEventOccurrence += (sender, e) =>
{
    StringBuilder sb = new("事件：");
    var t = e.GetType();
    foreach (var i in t.GetProperties().Where(p => p.CanRead))
        sb.Append($"{i.Name}={i.GetValue(e)}，");
    Console.WriteLine(sb.ToString());
};
client.OnExceptionOccurrence += (sender, e) =>
{
    Console.WriteLine($"异常：{e}");
};
if (human)
{
    while (true)
    {
        var s = Console.ReadLine().Split(' ');
        if (s.Length <= 0)
            break;
        switch (s[0].ToLower())
        {
            case "sendprivatemessage":
                if (s.Length < 3)
                    goto default;
                Console.WriteLine(await client.SendPrivateMessageAsync(long.Parse(s[1]), new(new TextPart(s[2]))));
                break;
            case "sendgroupmessage":
                if (s.Length < 3)
                    goto default;
                Console.WriteLine(await client.SendGroupMessageAsync(long.Parse(s[1]), new(new TextPart(s[2]))));
                break;
            case "recallmessage":
                if (s.Length < 2)
                    goto default;
                await client.RecallMessageAsync(long.Parse(s[1]));
                Console.WriteLine("DONE!");
                break;
            case "getmessage":
                if (s.Length < 2)
                    goto default;
                Console.WriteLine(GetObjectProps(await client.GetMessageAsync(long.Parse(s[1]))));
                break;
            case "getforwardmessage":
                if (s.Length < 2)
                    goto default;
                Console.WriteLine(string.Concat((await client.GetForwardMessageAsync(s[1])).ConvertAll(m => m as CustomForwardNodePart).ConvertAll(cfnp => $"[{GetObjectProps(cfnp)}]")));
                break;
            case "sendlike":
                if (s.Length < 2)
                    goto default;
                await client.SendLikeAsync(long.Parse(s[1]), s.Length >= 3 ? int.Parse(s[2]) : 1);
                Console.WriteLine("DONE!");
                break;
            case "dogroupkick":
                if (s.Length < 3)
                    goto default;
                await client.DoGroupKickAsync(long.Parse(s[1]), long.Parse(s[2]), s.Length >= 4 ? bool.Parse(s[4]) : false);
                break;
            case "dogroupban":
                if (s.Length < 4)
                    goto default;
                await client.DoGroupBanAsync(long.Parse(s[1]), long.Parse(s[2]), int.Parse(s[4]));
                break;
            case "cancelgroupban":
                if (s.Length < 3)
                    goto default;
                await client.CancelGroupBanAsync(long.Parse(s[1]), long.Parse(s[2]));
                break;
            case "dowholegroupban":
                if (s.Length < 2)
                    goto default;
                await client.DoWholeGroupBanAsync(long.Parse(s[1]));
                break;
            case "cancelwholegroupban":
                if (s.Length < 2)
                    goto default;
                await client.CancelWholeGroupBanAsync(long.Parse(s[1]));
                break;
            case "setgroupadmin":
                if (s.Length < 3)
                    goto default;
                await client.SetGroupAdminAsync(long.Parse(s[1]), long.Parse(s[2]));
                break;
            case "dismissgroupadmin":
                if (s.Length < 3)
                    goto default;
                await client.DismissGroupAdminAsync(long.Parse(s[1]), long.Parse(s[2]));
                break;
            case "setgroupcard":
                if (s.Length < 4)
                    goto default;
                await client.SetGroupCardAsync(long.Parse(s[1]), long.Parse(s[2]), s[3]);
                break;
            case "deletegroupcard":
                if (s.Length < 3)
                    goto default;
                await client.DeleteGroupCardAsync(long.Parse(s[1]), long.Parse(s[2]));
                break;
            case "setgroupname":
                if (s.Length < 3)
                    goto default;
                await client.SetGroupNameAsync(long.Parse(s[1]), s[2]);
                break;
            case "leavefromgroup":
                if (s.Length < 2)
                    goto default;
                await client.LeaveFromGroupAsync(long.Parse(s[1]));
                break;
            case "setgroupspecialtitle":
                if (s.Length < 4)
                    goto default;
                await client.SetGroupSpecialTitleAsync(long.Parse(s[1]), long.Parse(s[2]), s[3]);
                break;
            case "deletegroupspecialtitle":
                if (s.Length < 3)
                    goto default;
                await client.DeleteGroupSpecialTitleAsync(long.Parse(s[1]), long.Parse(s[2]));
                break;
            case "acceptfriendaddrequest":
                if (s.Length < 2)
                    goto default;
                await client.AcceptFriendAddRequestAsync(s[1], s.Length >= 3 ? s[2] : null);
                break;
            case "rejectfriendaddrequest":
                if (s.Length < 2)
                    goto default;
                await client.RejectFriendAddRequestAsync(s[1]);
                break;
            case "acceptgrupjoinrequest":
                if (s.Length < 2)
                    goto default;
                await client.AcceptGroupJoinRequestAsync(s[1]);
                break;
            case "rejectgroupjoinrequest":
                if (s.Length < 2)
                    goto default;
                await client.RejectGroupJoinRequestAsync(s[1], s.Length >= 3 ? s[2] : null);
                break;
            case "acceptgrupinviterequest":
                if (s.Length < 2)
                    goto default;
                await client.AcceptGroupInviteRequestAsync(s[1]);
                break;
            case "rejectgroupinviterequest":
                if (s.Length < 2)
                    goto default;
                await client.RejectGroupInviteRequestAsync(s[1], s.Length >= 3 ? s[2] : null);
                break;
            case "getlogininfo":
                Console.WriteLine(GetObjectProps(await client.GetLoginInfoAsync()));
                break;
            case "getstrangerinfo":
                if (s.Length < 2)
                    goto default;
                Console.WriteLine(GetObjectProps(await client.GetStrangerInfoAsync(long.Parse(s[1]), s.Length >= 3 ? bool.Parse(s[2]) : true)));
                break;
            case "getgrouplist":
                Console.WriteLine(string.Concat(Array.ConvertAll(await client.GetGroupList(), (l) => $"【{GetObjectProps(l)}】")));
                break;
            default:
                Console.WriteLine("无效方法或无效参数！");
                break;
        }
    }
}
else
    await Task.Delay(-1);

string GetObjectProps(object obj) =>
    string.Concat(from prop in obj.GetType().GetProperties() where prop.CanRead select $"{prop.Name}={prop.GetValue(obj)}，");