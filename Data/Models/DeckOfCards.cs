namespace CardsTools.Data.Models;

/// <summary>
/// A named, finite collection of <see cref="Card"/>s.
///
/// The model is intentionally devoid of UI / logging concerns — those live in
/// the menu and command layers. All mutating methods return an
/// <see cref="OperationResult"/> so callers can react without inspecting
/// internal state.
/// </summary>
public sealed class DeckOfCards
{
    public const int MaxCards = 100;

    private readonly List<Card> _cards;
    private readonly Random _random;

    public string Name { get; private set; }
    public IReadOnlyList<Card> Cards => _cards;
    public bool IsSorted { get; private set; }

    public DeckOfCards(string name) : this(name, [], shuffleSeed: null) { }

    // Test seam: deterministic shuffling.
    public DeckOfCards(string name, IEnumerable<Card> initial, int? shuffleSeed)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        Name = name;
        _cards = [.. initial];
        _random = shuffleSeed.HasValue ? new Random(shuffleSeed.Value) : new Random();
    }

    public void Rename(string newName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(newName);
        Name = newName;
    }

    public OperationResult AddCard(string name, string description, int power)
    {
        if (_cards.Count >= MaxCards)
        {
            return OperationResult.Failure($"Deck is full ({MaxCards} cards).");
        }
        if (_cards.Any(c => string.Equals(c.Name, name, StringComparison.OrdinalIgnoreCase)))
        {
            return OperationResult.Failure($"Card '{name}' already exists.");
        }

        var id = _cards.Count;
        _cards.Add(new Card(id, name, description, power));
        IsSorted = false;
        return OperationResult.Success();
    }

    public OperationResult RemoveCard(string name)
    {
        var idx = _cards.FindIndex(c =>
            string.Equals(c.Name, name, StringComparison.OrdinalIgnoreCase));
        if (idx < 0)
        {
            return OperationResult.Failure($"Card '{name}' not found.");
        }

        _cards.RemoveAt(idx);
        IsSorted = false;
        return OperationResult.Success();
    }

    public void Clear()
    {
        _cards.Clear();
        IsSorted = false;
    }

    public void SortById()
    {
        _cards.Sort((a, b) => a.Id.CompareTo(b.Id));
        IsSorted = true;
    }

    public void SortByPower()
    {
        _cards.Sort((a, b) => b.Power.CompareTo(a.Power));
        IsSorted = true;
    }

    /// <summary>Fisher–Yates shuffle using the deck's <see cref="Random"/>.</summary>
    public void Shuffle()
    {
        for (var i = _cards.Count - 1; i > 0; i--)
        {
            var j = _random.Next(i + 1);
            (_cards[i], _cards[j]) = (_cards[j], _cards[i]);
        }
        IsSorted = false;
    }

    /// <summary>
    /// Replace the contents of this deck with the supplied snapshot. Used by
    /// the Memento / undo-redo layer.
    /// </summary>
    public void RestoreFrom(IEnumerable<Card> snapshot)
    {
        _cards.Clear();
        _cards.AddRange(snapshot);
    }

    /// <summary>Take an immutable snapshot of the current cards.</summary>
    public IReadOnlyList<Card> Snapshot() => [.. _cards];
}

public readonly record struct OperationResult(bool Ok, string? Error)
{
    public static OperationResult Success() => new(true, null);
    public static OperationResult Failure(string message) => new(false, message);
}
