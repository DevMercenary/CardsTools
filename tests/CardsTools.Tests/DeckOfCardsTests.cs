using CardsTools.Data.Models;
using FluentAssertions;
using Xunit;

namespace CardsTools.Tests;

public sealed class DeckOfCardsTests
{
    [Fact]
    public void AddCard_assigns_sequential_ids_and_unsorts()
    {
        var deck = new DeckOfCards("d");
        deck.SortById(); // make IsSorted true to verify Add resets it

        deck.AddCard("Ace", "high", 14).Ok.Should().BeTrue();
        deck.AddCard("Two", "low", 2).Ok.Should().BeTrue();

        deck.Cards.Select(c => c.Id).Should().Equal(0, 1);
        deck.IsSorted.Should().BeFalse();
    }

    [Fact]
    public void AddCard_rejects_duplicate_names_case_insensitively()
    {
        var deck = new DeckOfCards("d");
        deck.AddCard("Ace", "x", 1);

        var dup = deck.AddCard("ACE", "y", 2);
        dup.Ok.Should().BeFalse();
        dup.Error.Should().Contain("already exists");
        deck.Cards.Should().HaveCount(1);
    }

    [Fact]
    public void RemoveCard_returns_failure_when_missing()
    {
        var deck = new DeckOfCards("d");
        var result = deck.RemoveCard("ghost");
        result.Ok.Should().BeFalse();
    }

    [Fact]
    public void SortByPower_orders_descending()
    {
        var deck = new DeckOfCards("d");
        deck.AddCard("A", "", 5);
        deck.AddCard("B", "", 20);
        deck.AddCard("C", "", 1);

        deck.SortByPower();

        deck.Cards.Select(c => c.Power).Should().Equal(20, 5, 1);
        deck.IsSorted.Should().BeTrue();
    }

    [Fact]
    public void Shuffle_with_seed_is_deterministic()
    {
        var deck1 = new DeckOfCards(
            "d",
            Enumerable.Range(0, 8).Select(i => new Card(i, $"C{i}", "", i)),
            shuffleSeed: 42);
        var deck2 = new DeckOfCards(
            "d",
            Enumerable.Range(0, 8).Select(i => new Card(i, $"C{i}", "", i)),
            shuffleSeed: 42);

        deck1.Shuffle();
        deck2.Shuffle();

        deck1.Cards.Should().Equal(deck2.Cards);
    }

    [Fact]
    public void Cannot_exceed_max_cards()
    {
        var deck = new DeckOfCards("d");
        for (var i = 0; i < DeckOfCards.MaxCards; i++)
        {
            deck.AddCard($"card{i}", "", i).Ok.Should().BeTrue();
        }

        var overflow = deck.AddCard("overflow", "", 999);
        overflow.Ok.Should().BeFalse();
        overflow.Error.Should().Contain("full");
    }

    [Fact]
    public void Snapshot_returns_independent_copy()
    {
        var deck = new DeckOfCards("d");
        deck.AddCard("A", "", 1);

        var snapshot = deck.Snapshot();
        deck.AddCard("B", "", 2);

        snapshot.Should().HaveCount(1);
        deck.Cards.Should().HaveCount(2);
    }
}
