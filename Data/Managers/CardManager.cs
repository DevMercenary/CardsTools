using CardsTools.Data.Models;
using CardsTools.Data.Tools;
using CardsTools.Persistence;
using Microsoft.Extensions.Logging;

namespace CardsTools.Data.Managers;

/// <summary>
/// Owns the in-memory collection of decks, the currently active deck, and the
/// per-deck undo/redo history. Created once by the DI container — replaces
/// the old hand-rolled Singleton.
/// </summary>
public sealed partial class CardManager(IDeckStorage storage, ILogger<CardManager> logger)
{
    private readonly List<DeckOfCards> _decks = [];
    private readonly Dictionary<string, DeckHistory> _history = [];

    public IReadOnlyList<DeckOfCards> Decks => _decks;
    public DeckOfCards? ActiveDeck { get; private set; }

    public DeckHistory? ActiveHistory =>
        ActiveDeck is null ? null : _history[ActiveDeck.Name];

    public OperationResult CreateDeck(string name)
    {
        if (ValidationCard.IsBlank(name))
        {
            return OperationResult.Failure("Deck name cannot be empty.");
        }
        if (_decks.Any(d => string.Equals(d.Name, name, StringComparison.OrdinalIgnoreCase)))
        {
            return OperationResult.Failure($"Deck '{name}' already exists.");
        }

        var deck = new DeckOfCards(name);
        _decks.Add(deck);
        _history[deck.Name] = new DeckHistory();
        LogCreated(name);
        return OperationResult.Success();
    }

    public OperationResult Activate(string name)
    {
        var deck = _decks.FirstOrDefault(d =>
            string.Equals(d.Name, name, StringComparison.OrdinalIgnoreCase));
        if (deck is null)
        {
            return OperationResult.Failure($"Deck '{name}' not found.");
        }
        ActiveDeck = deck;
        return OperationResult.Success();
    }

    public OperationResult Rename(string newName)
    {
        if (ActiveDeck is null)
        {
            return OperationResult.Failure("No active deck.");
        }
        if (ValidationCard.IsBlank(newName))
        {
            return OperationResult.Failure("Name cannot be empty.");
        }
        if (_decks.Any(d => string.Equals(d.Name, newName, StringComparison.OrdinalIgnoreCase)))
        {
            return OperationResult.Failure($"Deck '{newName}' already exists.");
        }

        var oldName = ActiveDeck.Name;
        ActiveDeck.Rename(newName);
        _history[newName] = _history[oldName];
        _history.Remove(oldName);
        return OperationResult.Success();
    }

    public OperationResult RemoveActive()
    {
        if (ActiveDeck is null)
        {
            return OperationResult.Failure("No active deck.");
        }
        _history.Remove(ActiveDeck.Name);
        _decks.Remove(ActiveDeck);
        ActiveDeck = null;
        return OperationResult.Success();
    }

    public async Task<OperationResult> ImportAsync(string path, CancellationToken ct = default)
    {
        var deck = await storage.LoadAsync(path, ct);
        if (deck is null)
        {
            return OperationResult.Failure($"Could not load '{path}'.");
        }

        var name = deck.Name;
        if (_decks.Any(d => string.Equals(d.Name, name, StringComparison.OrdinalIgnoreCase)))
        {
            // Auto-rename on conflict so the import always succeeds.
            var suffix = DateTime.UtcNow.ToString("yyyyMMddTHHmmss");
            name = $"{deck.Name}_Backup_{suffix}";
            deck.Rename(name);
            LogRenamedOnImport(deck.Name);
        }

        _decks.Add(deck);
        _history[deck.Name] = new DeckHistory();
        LogImported(deck.Name);
        return OperationResult.Success();
    }

    public Task SaveActiveAsync(CancellationToken ct = default)
    {
        if (ActiveDeck is null)
        {
            return Task.CompletedTask;
        }
        return storage.SaveAsync(ActiveDeck, ct);
    }

    [LoggerMessage(EventId = 3001, Level = LogLevel.Information, Message = "Created deck {Name}")]
    private partial void LogCreated(string name);

    [LoggerMessage(EventId = 3002, Level = LogLevel.Information, Message = "Imported deck {Name}")]
    private partial void LogImported(string name);

    [LoggerMessage(EventId = 3003, Level = LogLevel.Warning, Message = "Deck name conflict on import, renamed to {Name}")]
    private partial void LogRenamedOnImport(string name);
}
