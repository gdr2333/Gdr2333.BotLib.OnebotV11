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

namespace Gdr2333.BotLib.OnebotV11;

/// <summary>
/// 客户端内部异常
/// </summary>
public class OnebotV11ClientException : ApplicationException
{
    /// <summary>
    /// 新建一个客户端内部异常
    /// </summary>
    public OnebotV11ClientException()
    {
    }

    /// <summary>
    /// 新建一个具有指定信息的客户端内部异常
    /// </summary>
    /// <param name="message">异常信息</param>
    public OnebotV11ClientException(string? message) : base()
    {
    }

    /// <summary>
    /// 新建一个具有指定信息和内部异常的客户端内部异常
    /// </summary>
    /// <param name="message">异常信息</param>
    /// <param name="innerException">内部异常</param>
    public OnebotV11ClientException(string? message, Exception? innerException) : base()
    {
    }
}
