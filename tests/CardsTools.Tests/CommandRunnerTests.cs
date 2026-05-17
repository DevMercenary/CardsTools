using CardsTools.Commands;
using CardsTools.Data.Managers;
using CardsTools.Data.Models;
using CardsTools.Persistence;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace CardsTools.Tests;

public sealed class CommandRunnerTests
{
    [Fact]
    public void Add_then_undo_restores_empty_deck()
    {
        var (runner, manager) = BuildRunner();
        manager.CreateDeck("d");
        manager.Activate("d");

        runner.Run(new AddCardCommand("A", "", 1)).Ok.Should().BeTrue();
        manager.ActiveDeck!.Cards.Should().HaveCount(1);

        runner.Undo().Should().BeTrue();
        manager.ActiveDeck.Cards.Should().BeEmpty();
    }

    [Fact]
    public void Add_then_undo_then_redo_re_adds_card()
    {
        var (runner, manager) = BuildRunner();
        manager.CreateDeck("d");
        manager.Activate("d");

        runner.Run(new AddCardCommand("A", "", 1));
        runner.Undo();
        runner.Redo().Should().BeTrue();

        manager.ActiveDeck!.Cards.Should().HaveCount(1);
        manager.ActiveDeck.Cards[0].Name.Should().Be("A");
    }

    [Fact]
    public void Failed_command_does_not_consume_undo_slot()
    {
        var (runner, manager) = BuildRunner();
        manager.CreateDeck("d");
        manager.Activate("d");

        runner.Run(new AddCardCommand("A", "", 1));
        var dup = runner.Run(new AddCardCommand("A", "", 1)); // duplicate

        dup.Ok.Should().BeFalse();
        // Undo should restore the empty deck — confirming the failed command
        // rolled its memento back.
        runner.Undo().Should().BeTrue();
        manager.ActiveDeck!.Cards.Should().BeEmpty();
    }

    [Fact]
    public void Run_without_active_deck_fails_gracefully()
    {
        var (runner, _) = BuildRunner();
        var result = runner.Run(new ClearDeckCommand());
        result.Ok.Should().BeFalse();
    }

    private static (CommandRunner runner, CardManager manager) BuildRunner()
    {
        var storage = new InMemoryDeckStorage();
        var manager = new CardManager(storage, NullLogger<CardManager>.Instance);
        var runner = new CommandRunner(manager, NullLogger<CommandRunner>.Instance);
        return (runner, manager);
    }

    private sealed class InMemoryDeckStorage : IDeckStorage
    {
        public Task SaveAsync(DeckOfCards deck, CancellationToken ct = default) => Task.CompletedTask;
        public Task<DeckOfCards?> LoadAsync(string path, CancellationToken ct = default) =>
            Task.FromResult<DeckOfCards?>(null);
        public IReadOnlyList<string> ListSaved() => [];
    }
}
