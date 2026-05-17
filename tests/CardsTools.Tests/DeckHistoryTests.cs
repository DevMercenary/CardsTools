using CardsTools.Data.Models;
using CardsTools.Data.Tools;
using FluentAssertions;
using Xunit;

namespace CardsTools.Tests;

public sealed class DeckHistoryTests
{
    [Fact]
    public void Undo_restores_previous_state()
    {
        var deck = new DeckOfCards("d");
        deck.AddCard("A", "", 1);
        var history = new DeckHistory();

        history.Save(deck);
        deck.AddCard("B", "", 2);

        history.Undo(deck).Should().BeTrue();
        deck.Cards.Select(c => c.Name).Should().Equal("A");
    }

    [Fact]
    public void Redo_replays_undone_state()
    {
        var deck = new DeckOfCards("d");
        deck.AddCard("A", "", 1);
        var history = new DeckHistory();

        history.Save(deck);
        deck.AddCard("B", "", 2);
        history.Undo(deck);

        history.Redo(deck).Should().BeTrue();
        deck.Cards.Select(c => c.Name).Should().Equal("A", "B");
    }

    [Fact]
    public void Save_clears_redo_stack()
    {
        var deck = new DeckOfCards("d");
        var history = new DeckHistory();

        history.Save(deck);
        deck.AddCard("A", "", 1);
        history.Undo(deck);
        history.CanRedo.Should().BeTrue();

        history.Save(deck);
        history.CanRedo.Should().BeFalse();
    }

    [Fact]
    public void Undo_on_empty_history_returns_false()
    {
        var deck = new DeckOfCards("d");
        var history = new DeckHistory();
        history.Undo(deck).Should().BeFalse();
    }
}
