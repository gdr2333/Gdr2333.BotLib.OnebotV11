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

#if DEBUG
using System.Runtime.CompilerServices;

// 仅在 DEBUG 构建下向单元测试项目暴露 internal 成员。
// Release 构建走 NuGet 包发布流程,这里不应当被打开。
[assembly: InternalsVisibleTo("Gdr2333.BotLib.OnebotV11.Tests")]
#endif