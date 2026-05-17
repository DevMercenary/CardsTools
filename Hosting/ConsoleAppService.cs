using CardsTools.Commands;
using CardsTools.Data.Managers;
using CardsTools.Data.Tools;
using CardsTools.Menu;
using CardsTools.Persistence;
using Microsoft.Extensions.Hosting;

namespace CardsTools.Hosting;

/// <summary>
/// Hosted service that drives the console UI. The framework's lifetime token
/// is used to exit cleanly on Ctrl-C.
/// </summary>
public sealed class ConsoleAppService(
    CardManager manager,
    CommandRunner runner,
    IDeckStorage storage,
    IHostApplicationLifetime lifetime) : BackgroundService
{
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        BuildMenu().Run();
        lifetime.StopApplication();
        return Task.CompletedTask;
    }

    private MenuBuilder.MenuNode BuildMenu() =>
        MenuBuilder.Root("Главное меню / Main Menu")
            .Submenu("Колоды / Decks", decks => decks
                .Item("List", ListDecks)
                .Item("Create", CreateDeck)
                .Item("Import", ImportFromDisk)
                .Back("Назад / Back"))
            .Back("Выйти / Quit");

    private void ListDecks()
    {
        if (manager.Decks.Count == 0)
        {
            Console.WriteLine("No decks yet. Create one first.");
            Pause();
            return;
        }

        var node = MenuBuilder.Root("Decks");
        foreach (var deck in manager.Decks)
        {
            var captured = deck.Name;
            node.Item(captured, () =>
            {
                manager.Activate(captured);
                ActiveDeckMenu().Run();
            });
        }
        node.Back("Назад / Back").Run();
    }

    private void CreateDeck()
    {
        Console.Write("Deck name: ");
        var name = Console.ReadLine();
        if (ValidationCard.IsBlank(name))
        {
            Console.WriteLine("Name cannot be empty.");
            Pause();
            return;
        }
        var result = manager.CreateDeck(name!);
        Console.WriteLine(result.Ok ? $"Created '{name}'." : result.Error);
        Pause();
    }

    private void ImportFromDisk()
    {
        var saved = storage.ListSaved();
        if (saved.Count == 0)
        {
            Console.WriteLine("No saved decks on disk.");
            Pause();
            return;
        }

        var node = MenuBuilder.Root("Import");
        foreach (var path in saved)
        {
            var captured = path;
            node.Item(Path.GetFileName(captured), () =>
            {
                var result = manager.ImportAsync(captured).GetAwaiter().GetResult();
                Console.WriteLine(result.Ok ? "Imported." : result.Error);
                Pause();
            });
        }
        node.Back("Назад / Back").Run();
    }

    private MenuBuilder.MenuNode ActiveDeckMenu() =>
        MenuBuilder.Root($"Deck: {manager.ActiveDeck?.Name}")
            .Item("About", DescribeActive)
            .Item("Add card", AddCard)
            .Item("Remove card", RemoveCard)
            .Item("Clear", () => RunCmd(new ClearDeckCommand()))
            .Item("Sort by id", () => RunCmd(new SortByIdCommand()))
            .Item("Sort by power", () => RunCmd(new SortByPowerCommand()))
            .Item("Shuffle", () => RunCmd(new ShuffleCommand()))
            .Item("Undo", () => { Console.WriteLine(runner.Undo() ? "undone" : "Nothing to undo."); Pause(); })
            .Item("Redo", () => { Console.WriteLine(runner.Redo() ? "redone" : "Nothing to redo."); Pause(); })
            .Item("Rename deck", Rename)
            .Item("Delete deck", DeleteActive)
            .Item("Export (json)", () => { manager.SaveActiveAsync().GetAwaiter().GetResult(); Console.WriteLine("Saved."); Pause(); })
            .Back("Back");

    private void RunCmd(IDeckCommand command)
    {
        var result = runner.Run(command);
        if (!result.Ok)
        {
            Console.WriteLine(result.Error);
            Pause();
        }
    }

    private void AddCard()
    {
        Console.Write("Name: ");
        var name = Console.ReadLine();
        Console.Write("Description: ");
        var description = Console.ReadLine();
        Console.Write("Power (integer): ");
        var powerStr = Console.ReadLine();

        if (ValidationCard.IsBlank(name) || ValidationCard.IsBlank(description) ||
            !ValidationCard.TryParseInt(powerStr, out var power))
        {
            Console.WriteLine("Invalid input.");
            Pause();
            return;
        }
        RunCmd(new AddCardCommand(name!, description!, power));
    }

    private void RemoveCard()
    {
        Console.Write("Card name: ");
        var name = Console.ReadLine();
        if (ValidationCard.IsBlank(name)) return;
        RunCmd(new RemoveCardCommand(name!));
    }

    private void Rename()
    {
        Console.Write("New name: ");
        var name = Console.ReadLine();
        if (ValidationCard.IsBlank(name)) return;
        var result = manager.Rename(name!);
        Console.WriteLine(result.Ok ? "Renamed." : result.Error);
        Pause();
    }

    private void DeleteActive()
    {
        Console.Write("Press Y to confirm: ");
        var key = Console.ReadKey().Key;
        Console.WriteLine();
        if (key == ConsoleKey.Y)
        {
            manager.RemoveActive();
            Console.WriteLine("Deleted.");
            Pause();
        }
    }

    private void DescribeActive()
    {
        var deck = manager.ActiveDeck;
        if (deck is null) return;
        Console.WriteLine($"Deck: {deck.Name}   sorted={deck.IsSorted}   cards={deck.Cards.Count}");
        foreach (var card in deck.Cards)
        {
            Console.WriteLine(card);
        }
        Pause();
    }

    private static void Pause()
    {
        Console.WriteLine("Press any key…");
        Console.ReadKey(intercept: true);
    }
}
