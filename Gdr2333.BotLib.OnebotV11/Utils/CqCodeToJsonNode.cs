using System.Text.Json.Nodes;

namespace Gdr2333.BotLib.OnebotV11.Utils;

internal static class CqCodeToJsonNode
{
    public static JsonNode Convert(string cqCode)
    {
        static string PropValueDecode(string value) => value
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
                nowEnd = cqCode.Length - 1;
            ret.Add("type", cqCode[nowBegin..nowEnd]);
            nowBegin = nowEnd + 1;
            nowEnd = cqCode.IndexOf(',', nowBegin);
            if (nowEnd == -1)
                nowEnd = cqCode.Length - 1;
            while (nowEnd != -1 && nowEnd < cqCode.Length - 1)
            {
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
                nowBegin = nowEnd + 1;
                nowEnd = cqCode.IndexOf(',', nowBegin);
                if (nowEnd == -1)
                    nowEnd = cqCode.Length - 1;
            }
            return ret;
        }
        else
            return new JsonObject()
            {
                { "type", "text" },
                { "data", new JsonObject()
                    {
                        { "text", PropValueDecode(cqCode) }
                    }
                }
            };
    }
}
