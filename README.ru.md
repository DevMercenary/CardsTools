<div align="center">

# CardsTools

**Консольное приложение для работы с игральными и гадальными картами**

[![CI](https://github.com/DevMercenary/CardsTools/actions/workflows/ci.yml/badge.svg)](https://github.com/DevMercenary/CardsTools/actions/workflows/ci.yml)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)
[![.NET](https://img.shields.io/badge/.NET-10.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)

[English version](README.md)

</div>

---

Небольшое интерактивное консольное приложение, позволяющее создавать, редактировать и
импортировать/экспортировать колоды карт. Изначально песочница для отработки ООП,
позже выросло в портфолио-проект, показывающий ручной фреймворк меню,
паттерн Memento (под будущий undo/redo) и логирование через Serilog.

## Что умеет

- Создавать, переименовывать и удалять колоды карт.
- Добавлять, удалять, выводить и сортировать карты в колоде (по id, по силе, случайно).
- Сохранять колоды на диск в JSON + автоматический бэкап c timestamp.
- Импортировать ранее сохранённые колоды (с авто-переименованием при конфликте).
- Управлять всем через многоуровневое меню с клавиатурной навигацией.

## Архитектура и паттерны

Кода немного, но в нём специально использованы заметные ООП-приёмы:

| Паттерн                  | Где он живёт                                                            |
|--------------------------|-------------------------------------------------------------------------|
| **Singleton**            | `CardManager` (заменим на DI в ближайшем рефакторинге)                  |
| **Composite-меню**       | Иерархия `IMenu`/`IMenuItem` в `Data/Managers/MenuManager/`             |
| **Observer**             | Событие `DeskOfCards.OnSortedDeskNotify`                                |
| **Memento (заготовка)**  | `CardsMemento` + `CardCollectionHistory` — undo/redo в roadmap          |
| **Strategy (сортировка)**| Sort-by-id / sort-by-power / random-shuffle на уровне колоды            |
| **Pattern-matching**     | `Binding.ProcessAnswer` через switch-expression                         |

## Технологический стек

| Слой        | Библиотека                                                                               |
|-------------|------------------------------------------------------------------------------------------|
| Runtime     | .NET 10                                                                                  |
| Логирование | [Serilog](https://serilog.net/) + Compact JSON formatter + ANSI Console sink             |
| JSON        | [Newtonsoft.Json](https://www.newtonsoft.com/json) (порт на System.Text.Json — в roadmap)|

## Быстрый старт

```bash
dotnet restore
dotnet run
```

Появится главное меню — `↑`/`↓` навигация, `Enter` выбор, `Esc` назад,
`Ctrl`+`C` выход. Колоды сохраняются в папке `DeskCardSave/`.

## Структура проекта

```
CardsTools/
├── Program.cs                       Точка входа — сборка дерева главного меню
├── Data/
│   ├── Managers/
│   │   ├── CardManager.cs           Singleton; владеет всеми колодами, импорт/сохранение
│   │   ├── Keeper.cs                База для JSON-персистентности
│   │   └── MenuManager/             Ручной фреймворк консольного меню
│   │       ├── IMenu.cs, IMenuItem.cs
│   │       ├── Menu.cs, MenuItem.cs
│   │       ├── Binding.cs           Клавиатура → MenuPoint (switch expr.)
│   │       ├── MenuPoint.cs
│   │       └── ArgumentActionExecutionEvent.cs
│   ├── Models/
│   │   ├── Card.cs                  Сущность карты
│   │   └── DeskOfCards.cs           Колода = упорядоченный Card[] + сортировка/лимиты
│   └── Tools/
│       ├── FileHelper.cs            Пути хранилища + папка бэкапов
│       ├── ValidationCard.cs        Маленькие валидаторы ввода
│       ├── CardsMemento.cs          Memento (заготовка для undo/redo)
│       └── CardCollectionHistory.cs История memento (заготовка)
├── Directory.Build.props
├── CardsTools.csproj
└── CardsTools.sln
```

## Разработка

```bash
dotnet format --verify-no-changes        # проверка стиля
dotnet build --configuration Release     # сборка
dotnet test                              # тесты (в roadmap)
```

## Roadmap

Что превратит этот песочный проект в полноценный «настоящий C#»-образец:

- Замена ручного Singleton на **Microsoft.Extensions.Hosting** + DI.
- Замена **Newtonsoft.Json** на **System.Text.Json** (source-generated context, AOT-friendly).
- Введение **fluent `MenuBuilder` DSL** — меню декларативно, а не через ручные колбэки.
- Каждое пользовательское действие — отдельный **Command object** для реального
  Undo/Redo через готовую заготовку Memento.
- Перевод `Card` в **sealed record**.
- **xUnit-проект** тестов на инварианты колоды, историю и menu builder.

## Лицензия

[MIT](LICENSE).
