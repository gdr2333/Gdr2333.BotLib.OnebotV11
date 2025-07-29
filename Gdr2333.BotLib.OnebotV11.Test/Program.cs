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

Console.WriteLine("请输入目标URL：");
Uri url = new(Console.ReadLine());
Console.WriteLine("请输入目标AccessToken，没有则留空：");
var ak = Console.ReadLine();
var wsclient = new WebSocketClient(url, 5, ak);
await wsclient.Link();
client = wsclient;
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
while(true)
{
    var s = Console.ReadLine().Split(' ');
    if (s.Length <= 0)
        break;
    switch(s[0].ToLower())
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
        default:
            Console.WriteLine("无效方法或无效参数！");
            break;
    }
}

string GetObjectProps(object obj) =>
    string.Concat(from prop in obj.GetType().GetProperties() where prop.CanRead select $"{prop.Name}={prop.GetValue(obj)}，");