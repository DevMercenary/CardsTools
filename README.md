<div align="center">

# CardsTools

**A console deck manager — DI Host, Command pattern, real Memento undo/redo**

[![CI](https://github.com/DevMercenary/CardsTools/actions/workflows/ci.yml/badge.svg)](https://github.com/DevMercenary/CardsTools/actions/workflows/ci.yml)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)
[![.NET](https://img.shields.io/badge/.NET-10.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)

[Русская версия](README.ru.md)

</div>

---

A small interactive console application that lets you build, edit, persist and replay
decks of playing- or divination-cards. The project started as a sandbox for OOP and
grew into a portfolio piece showing how the same domain can be expressed cleanly with
DI, Command pattern, Memento and source-generated APIs.

## What is showcased

- **Generic Host + DI** — `Host.CreateApplicationBuilder` with everything resolved
  through the container; no `Singleton` patterns anywhere.
- **`IHostedService`** drives the console loop with cooperative shutdown on Ctrl-C.
- **Declarative menu DSL** — `MenuBuilder.Root(...)` fluently composes the menu tree
  on top of the existing `IMenu`/`IMenuItem` framework.
- **Command pattern** — every destructive operation (`AddCardCommand`,
  `RemoveCardCommand`, `SortByPowerCommand`, …) implements `IDeckCommand` and goes
  through a `CommandRunner` that snapshots a memento before applying.
- **Real Undo/Redo via Memento** — `DeckHistory` (two-stack) lets users walk forward
  and back through their edits; failed commands roll their memento back automatically.
- **System.Text.Json source-generated** persistence — no reflection at runtime,
  AOT-friendly serialisation via `[JsonSerializable]` + `JsonSerializerContext`.
- **`[LoggerMessage]` source generator** for allocation-free structured logs.
- **Sealed records** (`Card`, `CardsMemento`, `DeckSnapshot`) for invariant-preserving
  immutable data.
- **xUnit + FluentAssertions** test suite (21 tests covering deck invariants,
  history, command runner, manager and menu builder).

## Project layout

```
CardsTools/
├── Program.cs                       Host.CreateApplicationBuilder composition root
├── Hosting/
│   └── ConsoleAppService.cs         BackgroundService driving the console UI
├── Menu/
│   └── MenuBuilder.cs               Fluent DSL on top of the menu framework
├── Commands/
│   ├── IDeckCommand.cs              Command pattern interface + base class
│   ├── DeckCommands.cs              AddCard / RemoveCard / Clear / Sort / Shuffle
│   └── CommandRunner.cs             Saves memento → applies command → rolls back on failure
├── Persistence/
│   ├── IDeckStorage.cs              Save / Load / ListSaved abstraction
│   ├── JsonDeckStorage.cs           Filesystem impl + timestamped backups
│   └── JsonContext.cs               System.Text.Json source-generated context
├── Data/
│   ├── Models/
│   │   ├── Card.cs                  sealed record Card(Id, Name, Description, Power)
│   │   └── DeckOfCards.cs           Domain model + OperationResult struct
│   ├── Managers/
│   │   ├── CardManager.cs           DI-managed; owns decks + per-deck histories
│   │   └── MenuManager/             Hand-rolled console menu framework
│   │       ├── IMenu.cs, IMenuItem.cs, Menu.cs, MenuItem.cs
│   │       ├── Binding.cs           Keyboard → MenuPoint switch expression
│   │       └── MenuPoint.cs, ArgumentActionExecutionEvent.cs
│   └── Tools/
│       ├── CardsMemento.cs          Immutable snapshot record
│       ├── CardCollectionHistory.cs DeckHistory: two-stack Undo/Redo
│       └── ValidationCard.cs        Tiny input helpers
└── tests/
    └── CardsTools.Tests/            xUnit + FluentAssertions
```

## Quick start

```bash
dotnet restore
dotnet run
```

Use `↑`/`↓` to navigate, `Enter` to select, `Esc` to go back, `Ctrl`+`C` to quit.
Decks are persisted under `DeckCardSave/` with timestamped backups under
`DeckCardSave/Backup/<deck>/`.

## Development

```bash
dotnet format --verify-no-changes        # check style
dotnet build --configuration Release     # build
dotnet test                              # 21 xUnit tests
```

## Tech stack

| Layer        | Library                                                                                  |
|--------------|------------------------------------------------------------------------------------------|
| Runtime      | .NET 10                                                                                  |
| Hosting / DI | [`Microsoft.Extensions.Hosting`](https://learn.microsoft.com/dotnet/core/extensions/generic-host) |
| Logging      | [Serilog](https://serilog.net/) + `[LoggerMessage]` generators                           |
| JSON         | `System.Text.Json` (source-generated context, AOT-friendly)                              |
| Tests        | [xUnit](https://xunit.net/) + [FluentAssertions](https://fluentassertions.com/)          |

## License

[MIT](LICENSE).
