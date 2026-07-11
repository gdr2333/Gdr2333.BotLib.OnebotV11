# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project summary

`Gdr2333.BotLib.OnebotV11` is a .NET 10 class library implementing the [OneBot v11](https://github.com/botuniverse/onebot-11) protocol (the standard interface for QQ bot frontends like go-cqhttp / NapCat / Lagrange). The author's stated design goal is **simplicity** — only three top-level namespaces ship user-facing API surface:

| Namespace     | Contents                                                                   |
|---------------|----------------------------------------------------------------------------|
| `Clients`     | `OnebotV11ClientBase` + `WebSocketClient` / `ReverseWebSocketClient`      |
| `Events`      | `OnebotV11EventArgsBase` and all concrete event arg types                  |
| `Messages`    | `Message` + every `*Part` message segment class                            |

Support namespaces `Data` (DTOs for query results), `Utils` (JSON converters, `StaticData`), and `OnebotV11ClientException` are internal-ish glue. There are deliberately no "Sender"/"Action" wrapper classes (a deliberate departure from `EleCho.GoCqHttpSdk`, per the readme).

## Build & run

Standard `dotnet` CLI. From the repo root:

```bash
dotnet restore
dotnet build                              # whole solution
dotnet build -c Release                   # release build (also produces NuGet pkg from main csproj)
dotnet pack Gdr2333.BotLib.OnebotV11/Gdr2333.BotLib.OnebotV11.csproj -c Release
dotnet test                               # no test projects exist — see "Test project" below
```

`Directory.Build.props` sets `LangVersion=latest`, `Nullable=enable`, `ImplicitUsings=enable`, `TreatWarningsAsErrors=true`, `Deterministic=true` for every project. Any new warning breaks the build.

NuGet metadata (`Version`, `Authors`, `PackageLicenseExpression=Apache-2.0`, etc.) is declared inline in the main csproj — bump `Version` there when releasing.

## "Test" project

`Gdr2333.BotLib.OnebotV11.Test` is **not** an xUnit/MSTest project. It's an `<OutputType>Exe</OutputType>` console app (`Program.cs`) that acts as a **manual interactive smoke tester** against a live OneBot v11 endpoint. Without arguments, it prompts for a URL + access token and accepts simple text commands (`sendprivatemessage <uid> <text>`, `sendgroupmessage <gid> <text>`, `getlogininfo`, etc.). To run unattended, drop an `AutoTest.txt` next to the exe with `Y` (reverse WS) or `N` on line 1, the URL on line 2, and optional access token on line 3.

There is no automated test runner — exercise changes by `dotnet build` (which surfaces warnings) + manual run.

## Architecture

### Client layer

`OnebotV11ClientBase` ([OnebotV11ClientBase.cs](Gdr2333.BotLib.OnebotV11/Clients/OnebotV11ClientBase.cs)) is the user-facing facade. It declares every OneBot v11 action as a typed method (`SendPrivateMessageAsync`, `DoGroupKickAsync`, `GetGroupMemberListAsync`, etc.) and exposes abstract `CallApiAsync` / `InvokeApiAsync` overloads that subclasses implement. Two concrete subclasses:

- `WebSocketClient` ([Clients/WebSocketClient.cs](Gdr2333.BotLib.OnebotV11/Clients/WebSocketClient.cs)) — **正向** (forward) WS client: dials a OneBot HTTP+WS server (e.g. NapCat) and reconnects up to `maxRetry` times. `IDisposable`.
- `ReverseWebSocketClient` ([Clients/ReverseWebSocketClient.cs](Gdr2333.BotLib.OnebotV11/Clients/ReverseWebSocketClient.cs)) — **反向** WS server: the bot listens on a URL, and the OneBot implementation connects *into* it. Auto-routes incoming WS connections to `InternalApiClient` (`/api/`), `InternalEventClient` (`/event/`), or `InternalUniverseClient` (everything else, treated as combined API+events). `IDisposable`. API calls fan out to all currently-connected API/universe clients via `Task.WhenAny` and return the first success.

Both expose `OnEventOccurrence` and `OnExceptionOccurrence` events.

### Internal client layer (server-side only)

Each active reverse-WS connection is wrapped by one of three `internal sealed` classes, all defined under `Clients/`:

- `InternalUniverseClient` — handles a combined API+event WS; extends `OnebotV11ClientBase` (so it can reuse the API surface) and forwards through `ApiRequestDispatcher`.
- `InternalApiClient` — API-only WS endpoint.
- `InternalEventClient` — event-only WS endpoint; auto-reconnects via `OnLoopExit` while the socket stays open.

`ApiRequestDispatcher` ([Clients/ApiRequestDispatcher.cs](Gdr2333.BotLib.OnebotV11/Clients/ApiRequestDispatcher.cs)) is the shared state for any WebSocket-backed client: a `ConcurrentDictionary<Guid, Action<OnebotV11ApiResult>>` of pending requests + a `Channel<OnebotV11ApiRequest>` for outbound traffic. The `OnebotV11ApiRequest`/`OnebotV11ApiResult` wire types live in [Clients/Misc.cs](Gdr2333.BotLib.OnebotV11/Clients/Misc.cs). Cancellation hooks are wired via `CancellationTokenRegistration` wrapped in `StrongBox<>` so the registration object can be disposed from both the result callback and the cancellation callback.

### Event dispatch

`OnebotV11EventArgsBase` ([Events/OnebotV11EventArgsBase.cs](Gdr2333.BotLib.OnebotV11/Events/OnebotV11EventArgsBase.cs)) uses a hand-written `OnebotV11EventArgsConverter` + `OnebotV11EventDispatch` ([Events/OnebotV11EventDispatch.cs](Gdr2333.BotLib.OnebotV11/Events/OnebotV11EventDispatch.cs)) dispatch table to route incoming JSON to the right concrete `*EventArgs` type. Sub-dispatchers exist for `notice_type`/`notify.sub_type` (e.g. `poke`, `lucky_king`, `honor`). The shared `JsonSerializerOptions` (with this converter registered) is produced once by `Utils.StaticData.GetOptions()` and reused everywhere — do **not** build new options per call.

### Messages

`Message` ([Messages/Message.cs](Gdr2333.BotLib.OnebotV11/Messages/Message.cs)) is a `List<MessagePartBase>` with a custom `MessageConverter` that accepts either a JSON array (native) **or** a CQ-code string (decoded via `Utils.CqCodeToJsonNode`). The list is serialized straight back as an array.

Polymorphic segment dispatch is registered on `MessagePartBase` ([Messages/MessagePartBase.cs](Gdr2333.BotLib.OnebotV11/Messages/MessagePartBase.cs)) via `[JsonPolymorphic]`/`[JsonDerivedType]` keyed on the `type` discriminator (`text`, `image`, `at`, …). Note the comment in the file: `System.Text.Json` does not allow `JsonDerivedType` to be inherited transitively, so the file re-registers the derived types of `ContactPartBase` (`FriendContactPart`, `GroupContactPart`), `MusicSharePartBase` (`MusicSharePart`, `CustomMusicSharePart`), and `ForwardNodePart` explicitly at the bottom.

`ContactPartAlt` ([Messages/TmpAlt/ContactPartAlt.cs](Gdr2333.BotLib.OnebotV11/Messages/TmpAlt/ContactPartAlt.cs)) is an internal placeholder registered under the `contact` discriminator; it decodes the `type` field (`qq` vs `group`) post-deserialization and returns the correct public subclass via `GetRealPart()`. This is the workaround for `JsonDerivedType` not supporting discriminator strings whose value depends on a *property* of the JSON.

The `Messages/Payload/` folder holds internal-only `*Payload` records (e.g. `TextPartPayload`, `ContactPayload`) — every segment class exposes a `[JsonIgnore]` public property and a private `_*Payload` field, populated/cleared by `OnSerializing`/`OnDeserialized` hooks. The pattern is intentional: it keeps the public surface clean (typed properties only) while satisfying the OneBot v11 wire shape (`{"type":"text","data":{"text":"…"}}`).

### Forward messages — two distinct types

- `ForwardNodePart` (segments marked `type=node` where the content is a single existing message id) — minimal payload `{id}`.
- `CustomForwardNodePart` (segments where the content is a full custom message) — full payload with sender info and a nested `Message`. Created via `CustomForwardNodePart(string senderNickname, long senderId, Message? message, ...)`.

### Number & bool wire quirks

OneBot v11 implementations inconsistently serialize numbers and booleans. The library handles this with:

- `[JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]` on every numeric field/parameter.
- `Utils.OB11JsonBoolConverter` on every bool field — accepts `0/1`, `true/false`, `"yes"/"no"` and variants, normalizes to `bool`.

Use the same conventions when adding new request/response DTOs in `OnebotV11ClientBase` or `Data/`.

## Conventions

- Public API uses XML doc comments in Chinese; new public symbols should follow suit.
- `TreatWarningsAsErrors=true` is enforced via `Directory.Build.props` — `#pragma warning disable` blocks exist for intentional uses of `[Obsolete]` overloads (see `OnebotV11ClientBase.LeaveFromGroupAsync` overloads).
- Library targets `net10.0` only. No multi-targeting.
- All files start with the Apache-2.0 license header — copy it when adding new files.
- Anonymous-message APIs (`DoGroupAnonymousBanAsync`, `AllowAnonymousAsync`, `DisallowAnonmyousAsync`) are marked `[Obsolete(StaticData.AnonymousWarning)]` because new QQ versions dropped anonymous support; the constant lives in `StaticData`.