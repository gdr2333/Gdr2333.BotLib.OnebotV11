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

using Gdr2333.BotLib.OnebotV11.Events;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Gdr2333.BotLib.OnebotV11.Utils;

internal static class StaticData
{
    public const bool CqUseProxyDefault = true;
    public const bool CqUseCacheDefault = true;

    public const string AnonymousWarning = "新版QQ不再支持匿名消息，且微信等平台也不支持。目前应该没有实现继续使用该字段。";
    public const string BadEnumValueMessage = "无法接受的枚举类型！";

    private static readonly JsonSerializerOptions _options = BuildOptions();

    public static JsonSerializerOptions GetOptions() => _options;

    private static JsonSerializerOptions BuildOptions()
    {
        var options = new JsonSerializerOptions
        {
            // 默认忽略可空字段写 null，避免 ImagePart / RecordPart / SharePart 等
            // 的 data 段出现 "{type:null}" / "{url:null}" / "{name:null}" 这种
            // 严格 OneBot 实现可能拒收的"半 null"JSON。
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        };
        options.Converters.Add(new OnebotV11EventArgsConverter());
        return options;
    }
}
