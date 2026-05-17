using CardsTools.Data.Managers;
using CardsTools.Data.Models;
using CardsTools.Persistence;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace CardsTools.Tests;

public sealed class CardManagerTests
{
    [Fact]
    public void CreateDeck_rejects_duplicate_names()
    {
        var manager = BuildManager();
        manager.CreateDeck("Tarot").Ok.Should().BeTrue();
        var dup = manager.CreateDeck("tarot");
        dup.Ok.Should().BeFalse();
    }

    [Fact]
    public void CreateDeck_rejects_blank_names()
    {
        var manager = BuildManager();
        var blank = manager.CreateDeck("   ");
        blank.Ok.Should().BeFalse();
    }

    [Fact]
    public void Rename_moves_history_under_new_name()
    {
        var manager = BuildManager();
        manager.CreateDeck("old");
        manager.Activate("old");
        var historyBefore = manager.ActiveHistory;
        manager.Rename("new").Ok.Should().BeTrue();

        manager.ActiveDeck!.Name.Should().Be("new");
        manager.ActiveHistory.Should().BeSameAs(historyBefore);
    }

    [Fact]
    public async Task Import_renames_on_conflict()
    {
        var manager = BuildManager();
        manager.CreateDeck("shared");

        var fakeStorage = new SingleFileStorage(new DeckOfCards("shared"));
        var another = new CardManager(fakeStorage, NullLogger<CardManager>.Instance);
        another.CreateDeck("shared"); // already taken

        var result = await another.ImportAsync("anything");
        result.Ok.Should().BeTrue();
        another.Decks.Should().HaveCount(2);
        another.Decks.Skip(1).First().Name.Should().StartWith("shared_Backup_");
    }

    private static CardManager BuildManager() =>
        new(new NullStorage(), NullLogger<CardManager>.Instance);

    private sealed class NullStorage : IDeckStorage
    {
        public Task SaveAsync(DeckOfCards deck, CancellationToken ct = default) => Task.CompletedTask;
        public Task<DeckOfCards?> LoadAsync(string path, CancellationToken ct = default) =>
            Task.FromResult<DeckOfCards?>(null);
        public IReadOnlyList<string> ListSaved() => [];
    }

    private sealed class SingleFileStorage(DeckOfCards deck) : IDeckStorage
    {
        public Task SaveAsync(DeckOfCards deck, CancellationToken ct = default) => Task.CompletedTask;
        public Task<DeckOfCards?> LoadAsync(string path, CancellationToken ct = default) =>
            Task.FromResult<DeckOfCards?>(new DeckOfCards(deck.Name));
        public IReadOnlyList<string> ListSaved() => [deck.Name];
    }
}
