某个小孩做的OnebotV11协议实现库。

本库的主旨是尽量*简单*，所以整个库的核心功能只聚焦在以下几个命名空间（省略库名前缀）：

|命名空间|内容|
|---|---|
|Clients|客户端实现|
|Events|事件内容|
|Messages|消息内容|

*我才懒得去弄什么[Sender](https://github.com/OrgEleCho/EleCho.GoCqHttpSdk/blob/master/src/EleCho.GoCqHttpSdk/Action/Sender/CqWsActionSender.cs)和[Action](https://github.com/OrgEleCho/EleCho.GoCqHttpSdk/blob/master/src/EleCho.GoCqHttpSdk/Action/CqGetCookiesAction.cs)。*

功能实现状态：

|||
|---|---|
|OnebotV11规定的所有消息段|完成|
|OnebotV11规定的所有操作|完成|
|正向Websocket连接到Universeal客户端|完成|
|反向Websocket链接|完成|
|正向HTTP链接|不在计划内|
|反向HTTP链接|不在计划内|
|NapcatQQ的扩展消息段|已规划|
|NapcatQQ的扩展API|已规划|