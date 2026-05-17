using CardsTools.Data.Models;

namespace CardsTools.Data.Tools;

/// <summary>
/// Immutable snapshot of a deck's contents. Created by the deck before any
/// destructive operation so a <see cref="DeckHistory"/> can roll it back.
/// </summary>
public sealed record CardsMemento(IReadOnlyList<Card> Cards);
