# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## 项目概述

一个 C# 实现的 OneBot v11 协议 QQ 机器人库,目标框架 `net10.0`,通过 `GeneratePackageOnBuild` 打包为 NuGet 包(`Gdr2333.BotLib.OnebotV11`),协议遵循 Apache-2.0。本库的设计主旨是**尽量简单**,核心功能只围绕三个命名空间:`Clients`、`Events`、`Messages`。作者刻意没有为每个 OneBot action 单独封装 Sender 类(详见 [Readme.md](Readme.md) 链接到的 EleCho.GoCqHttpSdk 反例)。

## 构建与运行

```bash
# 构建整个解决方案
dotnet build Gdr2333.BotLib.sln

# 打包 NuGet 包(csproj 中已设置 GeneratePackageOnBuild,显式 pack 也可)
dotnet pack Gdr2333.BotLib.OnebotV11/Gdr2333.BotLib.OnebotV11.csproj

# 本项目没有单元测试。Gdr2333.BotLib.OnebotV11.Test 是 OutputType=Exe 的
# 手动冒烟测试 / 交互式 REPL:从控制台(或工作目录下的 AutoTest.txt)读取
# URL 和 access token,然后在键盘上调用所有 API 方法:
dotnet run --project Gdr2333.BotLib.OnebotV11.Test
# AutoTest.txt 格式(每行一项):
#   第 1 行: Y/N(是否反向 WebSocket)
#   第 2 行: 目标 URL
#   第 3 行: (可选)access token
```

版本号在 [Gdr2333.BotLib.OnebotV11/Gdr2333.BotLib.OnebotV11.csproj](Gdr2333.BotLib.OnebotV11/Gdr2333.BotLib.OnebotV11.csproj#L10) 的 `<Version>1.1.0</Version>` 处修改。

## 高层架构

```
Gdr2333.BotLib.OnebotV11/   ← 发布的库(根命名空间 Gdr2333.BotLib.OnebotV11)
├── Clients/                ← 公开的客户端抽象 + 内部 WebSocket 实现
├── Events/                 ← OneBot 事件参数继承体系
├── Messages/               ← 消息 + 消息段(segment)类型
│   ├── Payload/            ← 内部 DTO,对应 JSON 线缆格式
│   └── TmpAlt/             ← 仅在多态反序列化过程中使用的临时类
├── Data/                   ← API 返回的结果/信息 DTO
├── Utils/                  ← JsonConverter 实现 + StaticData
└── OnebotV11ClientException.cs

Gdr2333.BotLib.OnebotV11.Test/   ← 交互式冒烟测试 REPL(不是单元测试项目)
```

### 公开的客户端接口

[`OnebotV11ClientBase`](Gdr2333.BotLib.OnebotV11/Gdr2333.BotLib.OnebotV11/Clients/OnebotV11ClientBase.cs) 是抽象基类。它将每个 OneBot API 调用都声明为 `public` 方法,最终都委托到四个抽象的 `CallApiAsync` / `InvokeApiAsync` 重载之一。具体的客户端类有:

- [`WebSocketClient`](Gdr2333.BotLib.OnebotV11/Gdr2333.BotLib.OnebotV11/Clients/WebSocketClient.cs) — **正向**(主动连出)WebSocket。调用方必须先 `await LinkAsync()`,后续的 `Call/InvokeApiAsync` 调用在未连接时将抛出 `InvalidOperationException`。连接失败时最多重试 `maxRetry` 次(默认 5)。
- [`ReverseWebSocketClient`](Gdr2333.BotLib.OnebotV11/Gdr2333.BotLib.OnebotV11/Clients/ReverseWebSocketClient.cs) — **反向**(被动接受)HttpListener。根据基础 URL 注册三个 URL 前缀(`/`、`/api/`、`/event/`)。根据路径末段把每个连接路由到三种 `Internal…Client` 中的一种:
  - `/api` → `InternalApiClient`(只走 API 通道)
  - `/event` → `InternalEventClient`(只走事件通道)
  - 其他 → `InternalUniverseClient`(合一通道,OneBot v11 规范的默认形式)
  每种类型用独立的 `ReaderWriterLockSlim` 保护的列表维护。`CallApiAsync` 把请求扇出到所有已连接的 API+Universe 客户端,返回第一个 `Task.WhenAny` 成功完成的结果。
- `OnebotV11ClientBase` 的构造函数不会被直接调用——没有"全合一"的反向 WS 客户端。要在一个进程里同时接受三种连接,直接用 `ReverseWebSocketClient`(它内部处理路由)。

事件通过抽象的 `OnEventOccurrence` / `OnExceptionOccurrence` 事件传递。`OnEventOccurrence` 始终携带 `OnebotV11EventArgsBase` 的子类;根据类型 switch(或用相关事件参数实现的 `IGroupEventArgs` / `IUserEventArgs` 标记接口)取回群/用户 ID,就可以用 [`OnebotV11ClientBase.SendMessageAsync(OnebotV11EventArgsBase, Message, …)`](Gdr2333.BotLib.OnebotV11/Gdr2333.BotLib.OnebotV11/Clients/OnebotV11ClientBase.cs#L355) 回复。

### 线缆协议(JSON over WebSocket)

API 请求:`{ "action": "<名称>", "params": {…}, "echo": "<guid>" }`。
API 响应:`{ "status", "retcode", "data"?, "message"?, "wording"?, "echo" }`(见 [`OnebotV11ApiResult`](Gdr2333.BotLib.OnebotV11/Gdr2333.BotLib.OnebotV11/Clients/Misc.cs))。
事件没有 `echo` 字段——`InternalUniverseClient.ReceiveLoop` 通过尝试读取 `echo` 来区分两者。`retcode` 为 `0` 表示成功且带 data,`1` 表示"成功但无 data";其他情况抛 `OnebotV11ClientException`。

每次外发的 API 调用都会得到一个 `Guid` echo,在 `ConcurrentDictionary<Guid, Action<OnebotV11ApiResult>>` 中匹配等待中的回调。发送和接收分别在两个 `Channel` 驱动的循环中运行,所以调用方是非阻塞的。注意:代码在入队时使用了 `Channel.Writer.WriteAsync(...).AsTask().Wait()` 同步等待——如果修改排队策略请留意线程阻塞的影响。

### 消息

[`Message`](Gdr2333.BotLib.OnebotV11/Gdr2333.BotLib.OnebotV11/Messages/Message.cs) 是 `List<MessagePartBase>`,也可以从一个普通 `string` 构造(包装为单个 `TextPart`)。多态 JSON 反序列化由 `type` 鉴别字段驱动,鉴别字段通过 [`MessagePartBase`](Gdr2333.BotLib.OnebotV11/Gdr2333.BotLib.OnebotV11/Messages/MessagePartBase.cs) 上的 `[JsonDerivedType]` 特性注册。新增消息段类型:添加继承 `MessagePartBase` 的类,并添加一行 `[JsonDerivedType(typeof(YourPart), "type-string")]`。鉴别字段的取值必须和 OneBot 规范完全一致(例如 `"text"`、`"image"`、`"at"`)。

每个消息段都有公开的强类型属性(`Text`、`UserId` 等),加一个私有的 `_data` 字段(类型为 `Messages/Payload/*`),用来保存线缆格式的数据。基类实现了 `IJsonOnSerializing` / `IJsonOnDeserialized`——消息段重写 `OnSerializing`(属性推到 `_data`)和 `OnDeserialized`(把 `_data` 拉回属性,然后置空)。新增消息段时请遵循这个模式。

`Message` 既可以从 JSON 数组(标准)反序列化,也可以从 CQ 码字符串(例如 `"[CQ:at,qq=123] hello"`)反序列化;后者先经过 [`CqCodeToJsonNode`](Gdr2333.BotLib.OnebotV11/Gdr2333.BotLib.OnebotV11/Utils/CqCodeToJsonNode.cs) 转换再走反序列化。`contact` 段类型有特殊处理:它会先反序列化到内部的 [`ContactPartAlt`](Gdr2333.BotLib.OnebotV11/Gdr2333.BotLib.OnebotV11/Messages/TmpAlt/ContactPartAlt.cs),然后由 `MessageConverter` 根据 `type` 负载字段替换为 `FriendContactPart` 或 `GroupContactPart`。整个 `TmpAlt` 目录是只用于反序列化的暂存区。

### 事件

[`OnebotV11EventArgsBase`](Gdr2333.BotLib.OnebotV11/Gdr2333.BotLib.OnebotV11/Events/OnebotV11EventArgsBase.cs) 及其 `OnebotV11EventArgsConverter` 根据 `post_type` → `message_type` / `notice_type` / `sub_type` / `meta_event_type` / `request_type` 的链路选择具体的事件参数类。新增事件类型意味着:声明一个继承合适基类(`MessageReceivedEventArgsBase`、`NoticeEventArgsBase`、`MetaEventArgsBase`、`RequestEventArgsBase` 或直接继承 `OnebotV11EventArgsBase`)的类,把它加到文件顶部的 `[JsonDerivedType]` 列表里,并在转换器那个巨大的 `switch` 表达式里加上分发分支。

`MessageReceivedEventArgsBase` 携带实际的 `Message`;`IGroupEventArgs` 和 `IUserEventArgs` 标记接口暴露相关的 ID,用于 `SendMessageAsync(eventArgs, msg)` 的路由。某些属性被标了 `[Obsolete]`,因为新版 QQ/OneBot 实现已弃用——参见 [`StaticData.AnonymousWarning`](Gdr2333.BotLib.OnebotV11/Gdr2333.BotLib.OnebotV11/Utils/StaticData.cs)。触及这些过时路径时请保留 `[Obsolete]` 特性;本项目刻意保持比当前实现支持的 API 表面更宽。

### `Utils/` 里处理的 JSON 怪癖

- [`OB11JsonBoolConverter`](Gdr2333.BotLib.OnebotV11/Gdr2333.BotLib.OnebotV11/Utils/OB11JsonBoolConverter.cs) — 接受 `true`/`false`、`0`/`1`、`"yes"`/`"no"`,以及可解析为以上任何一种的字符串。所有 bool 类型的负载字段都应该用上它。
- [`UnixTimeToDateTimeConverter`](Gdr2333.BotLib.OnebotV11/Gdr2333.BotLib.OnebotV11/Utils/UnixTimeToDateTimeConverter.cs) — `DateTime` ↔ Unix 秒,同时接受数字和字符串两种 JSON token。
- `MillisecondToTimeSpanConverter` / `SecondToTimeSpanConverter` / `DayToTimeSpanConverter` — 同理,单位不同。
- `AutoDecisionUserInfoConverter` — 根据是否带有 `group_id` 字段,选择把入站消息的 `sender` 反序列化为 `UserInfo` 或 `GroupMemberInfo`。
- `StaticData.GetOptions()` 返回共享的 `JsonSerializerOptions`,里面注册了多态事件参数转换器;序列化往返时**复用**它(不要 `new` 另一个 options)。

`[JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]` 在所有数值字段上都被广泛使用——OneBot 实现可能把 ID 写成 JSON number 或 JSON string,有时还混用。新增字段时请保留这个习惯。

## 约定

- 全项目启用 `nullable`;除非有充分理由,不要抑制可空性警告。
- 所有 `.cs` 文件都以 Apache 2.0 版权头开头(`Copyright 2025 All contributors of Gdr2333.BotLib`)。
- 文档注释使用中文,请保持这种风格。
- 公开 API 在每个类型和成员上都有 `<summary>` XML 文档(csproj 中设置了 `GenerateDocumentationFile`)。
- `[Obsolete]` 用于标注已废弃的 OneBot 特性(匿名消息、`set_group_anonymous`、`set_group_special_title` 的 `seconds` 参数等)——保留以保持向后兼容,但要标注原因。**使用**过时符号时,用 `#pragma warning disable CS0618` 包住,而不是移除 `[Obsolete]` 特性。
- 内部需要跨程序集访问的类型放在 `Utils/` 或 `Messages/TmpAlt/` 下,用 `internal` 可见性;没有充分理由不要提升为 public。
- 项目刻意**不**为每个 API 单独封装 `Sender` / `Action` 层;调用方直接用 `client.SendGroupMessageAsync(...)` 这类方法。不要新增包装层。
- 仓库内不存在 `EditorConfig`、`.cursor/rules/`、`.cursorrules` 或 `.github/copilot-instructions.md`。
