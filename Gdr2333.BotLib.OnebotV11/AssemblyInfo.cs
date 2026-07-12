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

using System.Runtime.CompilerServices;

// 向单元测试项目暴露 internal 成员,Debug / Release 构建都生效。
// InternalsVisibleTo 是"主项目对指定 friend assembly 名的可见性授权",
// 不会泄露给普通 NuGet consumer(consumer 程序集名不匹配)——所以 Release NuGet
// 包里的 internal 字段/类仍然对最终用户隐藏,只是测试项目在两种 build 下都能
// 访问到 internal 以做白盒测试。
[assembly: InternalsVisibleTo("Gdr2333.BotLib.OnebotV11.Tests")]