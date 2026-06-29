/*
   Copyright 2025-2026 All contributors of Gdr2333.BotLib

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

using System.Text.Json.Nodes;

namespace Gdr2333.BotLib.OnebotV11.Utils;

internal static class CqCodeToJsonNode
{
    public static JsonNode Convert(string cqCode)
    {
        static JsonNode ConvertPart(string cqCode)
        {
            static string PropValueDecode(string value) =>
                value
                    .Replace("&amp;", "&")
                    .Replace("&#91;", "[")
                    .Replace("&#93;", "]")
                    .Replace("&#44;", ",");
            static string TextDecode(string text) =>
                text
                    .Replace("&amp;", "&")
                    .Replace("&#91;", "[")
                    .Replace("&#93;", "]");
            if (cqCode.StartsWith('[') && cqCode.EndsWith(']'))
            {
                var ret = new JsonObject();
                // [CQ:
                var nowBegin = 4;
                var nowEnd = cqCode.IndexOf(',', nowBegin);
                if (nowEnd == -1)
                {
                    nowEnd = cqCode.Length - 1;
                    ret.Add("type", cqCode[nowBegin..nowEnd]);
                    return ret;
                }
                ret.Add("type", cqCode[nowBegin..nowEnd]);
                do
                {
                    nowBegin = nowEnd + 1;
                    nowEnd = cqCode.IndexOf(',', nowBegin);
                    if (nowEnd == -1)
                        nowEnd = cqCode.Length - 1;
                    var propstr = cqCode[nowBegin..nowEnd];
                    if (propstr.Contains('='))
                    {
                        var splitIndex = propstr.IndexOf('=');
                        var propName = propstr[..splitIndex];
                        var propValue = PropValueDecode(propstr[(splitIndex + 1)..]);
                        ret.Add(propName, propValue);
                    }
                    else
                        ret.Add(propstr, null);
                } while (nowEnd < cqCode.Length - 1);
                return ret;
            }
            else
                return new JsonObject()
                {
                    { "type", "text" },
                    { "data", new JsonObject()
                        {
                            { "text", TextDecode(cqCode) }
                        }
                    }
                };
        }
        var nowBegin = 0;
        var ret = new JsonArray();
        while (nowBegin < cqCode.Length)
        {
            var readingCqCode = cqCode[nowBegin] == '[';
            int nowEnd;
            if (readingCqCode)
            {
                var closeIdx = cqCode.IndexOf(']', nowBegin);
                // 没有匹配的 ']' 时，把剩余部分当作 CQ 码本体然后结束
                if (closeIdx < 0)
                {
                    ret.Add(ConvertPart(cqCode[nowBegin..]));
                    break;
                }
                nowEnd = closeIdx + 1;
            }
            else
            {
                var openIdx = cqCode.IndexOf('[', nowBegin);
                nowEnd = openIdx < 0 ? cqCode.Length : openIdx;
            }
            ret.Add(ConvertPart(cqCode[nowBegin..nowEnd]));
            nowBegin = nowEnd;
        }
        return ret;
    }
}
