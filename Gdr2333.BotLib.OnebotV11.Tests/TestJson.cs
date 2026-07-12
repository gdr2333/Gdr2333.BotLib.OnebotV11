/*
   Copyright 2026 All contributors of Gdr2333.BotLib

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

using Gdr2333.BotLib.OnebotV11.Utils;

// 整个项目复用一份 JsonSerializerOptions,与生产代码保持一致:
// StaticData.GetOptions() 已经注册了 OnebotV11EventArgsConverter,
// 业务代码不应该自己 new 一个新的 options (CLAUDE.md 已强调)。
internal static class TestJson
{
    public static System.Text.Json.JsonSerializerOptions Options { get; } = StaticData.GetOptions();
}