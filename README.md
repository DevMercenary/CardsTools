<div align="center">

# CardsTools

**A console application for managing playing- and divination-card decks**

[![CI](https://github.com/DevMercenary/CardsTools/actions/workflows/ci.yml/badge.svg)](https://github.com/DevMercenary/CardsTools/actions/workflows/ci.yml)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)
[![.NET](https://img.shields.io/badge/.NET-10.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)

[Русская версия](README.ru.md)

</div>

---

A small, interactive console application that lets you create, edit and import/export
decks of cards. The project was originally a sandbox for practising object-oriented
design and grew into a portfolio piece showcasing a hand-rolled console menu framework,
the Memento pattern for undo/redo, and Serilog-based logging.

## What it does

- Create, rename and delete decks of cards.
- Add, remove, list and sort cards inside a deck (by id, by power, randomly).
- Persist decks to disk as JSON, plus automatic timestamped backups.
- Import previously exported decks (with auto-renaming on conflict).
- Drive everything from a keyboard-navigable nested menu.

## Architecture & patterns

The codebase is small enough to read top-to-bottom yet it intentionally uses several
OOP techniques worth pointing out:

| Pattern              | Where it lives                                                        |
|----------------------|-----------------------------------------------------------------------|
| **Singleton**        | `CardManager` (will be replaced by DI in upcoming refactor)           |
| **Composite menu**   | `IMenu`/`IMenuItem` hierarchy in `Data/Managers/MenuManager/`         |
| **Observer**         | `DeskOfCards.OnSortedDeskNotify` event tracking sort state            |
| **Memento (scaffolded)** | `CardsMemento` + `CardCollectionHistory` — full undo/redo in roadmap |
| **Strategy** (sort)  | Sort-by-id / sort-by-power / random-shuffle methods on the deck       |
| **Custom enums + switch expr.** | `MenuPoint` resolved through pattern-matching `Binding`    |

## Tech stack

| Layer       | Library                                                                                  |
|-------------|------------------------------------------------------------------------------------------|
| Runtime     | .NET 10                                                                                  |
| Logging     | [Serilog](https://serilog.net/) + Compact JSON formatter + ANSI Console sink             |
| JSON        | [Newtonsoft.Json](https://www.newtonsoft.com/json) (System.Text.Json port in roadmap)    |

## Quick start

```bash
dotnet restore
dotnet run
```

You will see the main menu — use `↑`/`↓` to navigate, `Enter` to select, `Esc` to go
back, `Ctrl`+`C` to quit. Decks are persisted under `DeskCardSave/`.

## Project layout

```
CardsTools/
├── Program.cs                       Entry point — wires the main menu tree
├── Data/
│   ├── Managers/
│   │   ├── CardManager.cs           Singleton; owns all decks, import/save flow
│   │   ├── Keeper.cs                JSON persistence base class
│   │   └── MenuManager/             Hand-rolled console menu framework
│   │       ├── IMenu.cs, IMenuItem.cs
│   │       ├── Menu.cs, MenuItem.cs
│   │       ├── Binding.cs           Keyboard → MenuPoint mapping (switch expr.)
│   │       ├── MenuPoint.cs
│   │       └── ArgumentActionExecutionEvent.cs
│   ├── Models/
│   │   ├── Card.cs                  Single card entity
│   │   └── DeskOfCards.cs           Deck = ordered Card[] + sort/random/limits
│   └── Tools/
│       ├── FileHelper.cs            Storage path resolution + backup folder
│       ├── ValidationCard.cs        Tiny input validators
│       ├── CardsMemento.cs          Memento (scaffolding for undo/redo)
│       └── CardCollectionHistory.cs Memento history (scaffolding)
├── Directory.Build.props
├── CardsTools.csproj
└── CardsTools.sln
```

## Development

```bash
dotnet format --verify-no-changes        # check style
dotnet build --configuration Release     # build
dotnet test                              # run tests (added in roadmap)
```

## Roadmap

Planned upgrades that turn this from a sandbox into a "show me real C#" sample:

- Replace the manual Singleton with **Microsoft.Extensions.Hosting** + DI.
- Replace **Newtonsoft.Json** with **System.Text.Json** (source-generated context, AOT-friendly).
- Introduce a **fluent `MenuBuilder` DSL** so menu trees are declared, not assembled in callbacks.
- Promote each user action into a **Command object** to power real Undo/Redo via the existing Memento scaffolding.
- Switch `Card` to a **sealed record**.
- Add an **xUnit test project** covering deck invariants, history and the menu builder.

## License

[MIT](LICENSE).
