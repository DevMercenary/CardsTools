<div align="center">

# CardsTools

**Консольный менеджер колод — DI Host, Command pattern, реальный Memento undo/redo**

[![CI](https://github.com/DevMercenary/CardsTools/actions/workflows/ci.yml/badge.svg)](https://github.com/DevMercenary/CardsTools/actions/workflows/ci.yml)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)
[![.NET](https://img.shields.io/badge/.NET-10.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)

[English version](README.md)

</div>

---

Маленькое интерактивное консольное приложение для создания, редактирования, сохранения и
проигрывания колод игральных/гадальных карт. Изначально песочница для ООП, выросла
в портфолио-проект, показывающий как ту же предметную область можно выразить чисто
через DI, Command pattern, Memento и source-generated API.

## Что показано

- **Generic Host + DI** — `Host.CreateApplicationBuilder` c полным разрешением через
  контейнер; никаких ручных Singleton.
- **`IHostedService`** ведёт консольный цикл с кооперативным завершением по Ctrl-C.
- **Декларативный DSL для меню** — `MenuBuilder.Root(...)` собирает дерево меню
  fluent-стилем поверх существующего фреймворка `IMenu`/`IMenuItem`.
- **Command pattern** — каждая деструктивная операция (`AddCardCommand`,
  `RemoveCardCommand`, `SortByPowerCommand`, …) реализует `IDeckCommand` и проходит
  через `CommandRunner`, который снимает memento до применения.
- **Реальный Undo/Redo через Memento** — `DeckHistory` (две стопки) позволяет ходить
  туда-сюда по правкам пользователя; неуспешные команды откатывают свой memento сами.
- **Сериализация через `System.Text.Json` source-generator** — никакой рефлексии в
  runtime, AOT-friendly через `[JsonSerializable]` + `JsonSerializerContext`.
- **`[LoggerMessage]` source generator** для бессаловских структурированных логов.
- **Sealed records** (`Card`, `CardsMemento`, `DeckSnapshot`) — иммутабельность с
  инвариантами.
- **xUnit + FluentAssertions** тесты (21 шт.: инварианты колоды, история,
  командный раннер, менеджер и menu builder).

## Структура проекта

```
CardsTools/
├── Program.cs                       Composition root c Host.CreateApplicationBuilder
├── Hosting/
│   └── ConsoleAppService.cs         BackgroundService — UI цикл
├── Menu/
│   └── MenuBuilder.cs               Fluent DSL поверх меню-фреймворка
├── Commands/
│   ├── IDeckCommand.cs              Интерфейс + базовый класс паттерна
│   ├── DeckCommands.cs              AddCard / RemoveCard / Clear / Sort / Shuffle
│   └── CommandRunner.cs             Сохранить memento → применить → откатить при ошибке
├── Persistence/
│   ├── IDeckStorage.cs              Save / Load / ListSaved абстракция
│   ├── JsonDeckStorage.cs           Реализация на ФС + timestamp-бэкапы
│   └── JsonContext.cs               System.Text.Json source-generated context
├── Data/
│   ├── Models/
│   │   ├── Card.cs                  sealed record Card(Id, Name, Description, Power)
│   │   └── DeckOfCards.cs           Доменная модель + OperationResult struct
│   ├── Managers/
│   │   ├── CardManager.cs           DI-managed; владеет колодами + per-deck историями
│   │   └── MenuManager/             Ручной фреймворк консольного меню
│   │       ├── IMenu.cs, IMenuItem.cs, Menu.cs, MenuItem.cs
│   │       ├── Binding.cs           Клавиатура → MenuPoint switch-expression
│   │       └── MenuPoint.cs, ArgumentActionExecutionEvent.cs
│   └── Tools/
│       ├── CardsMemento.cs          Иммутабельный snapshot-record
│       ├── CardCollectionHistory.cs DeckHistory: двустопочный Undo/Redo
│       └── ValidationCard.cs        Маленькие input-валидаторы
└── tests/
    └── CardsTools.Tests/            xUnit + FluentAssertions
```

## Быстрый старт

```bash
dotnet restore
dotnet run
```

`↑`/`↓` — навигация, `Enter` — выбор, `Esc` — назад, `Ctrl`+`C` — выход. Колоды
сохраняются в `DeckCardSave/` c timestamp-бэкапами в `DeckCardSave/Backup/<deck>/`.

## Разработка

```bash
dotnet format --verify-no-changes        # проверка стиля
dotnet build --configuration Release     # сборка
dotnet test                              # 21 xUnit-тест
```

## Технологический стек

| Слой         | Библиотека                                                                                |
|--------------|-------------------------------------------------------------------------------------------|
| Runtime      | .NET 10                                                                                   |
| Hosting / DI | [`Microsoft.Extensions.Hosting`](https://learn.microsoft.com/dotnet/core/extensions/generic-host) |
| Логирование  | [Serilog](https://serilog.net/) + `[LoggerMessage]` source-generators                     |
| JSON         | `System.Text.Json` (source-generated context, AOT-friendly)                               |
| Тесты        | [xUnit](https://xunit.net/) + [FluentAssertions](https://fluentassertions.com/)           |

## Лицензия

[MIT](LICENSE).
