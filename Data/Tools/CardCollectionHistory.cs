using CardsTools.Data.Models;

namespace CardsTools.Data.Tools;

/// <summary>
/// Two-stack Memento history for a single <see cref="DeckOfCards"/>. The
/// <see cref="Save"/> method is called before every destructive operation,
/// <see cref="Undo"/> rolls back, <see cref="Redo"/> replays a previously
/// undone state.
/// </summary>
public sealed class DeckHistory
{
    private readonly Stack<CardsMemento> _undo = new();
    private readonly Stack<CardsMemento> _redo = new();

    public bool CanUndo => _undo.Count > 0;
    public bool CanRedo => _redo.Count > 0;

    /// <summary>Save the current state of <paramref name="deck"/> as a memento.</summary>
    public void Save(DeckOfCards deck)
    {
        ArgumentNullException.ThrowIfNull(deck);
        _undo.Push(new CardsMemento(deck.Snapshot()));
        _redo.Clear();
    }

    /// <summary>
    /// Restore the deck to the previous saved state. Returns false if there
    /// is nothing to undo.
    /// </summary>
    public bool Undo(DeckOfCards deck)
    {
        ArgumentNullException.ThrowIfNull(deck);
        if (!CanUndo)
        {
            return false;
        }
        var snapshot = _undo.Pop();
        _redo.Push(new CardsMemento(deck.Snapshot()));
        deck.RestoreFrom(snapshot.Cards);
        return true;
    }

    /// <summary>Replay a previously undone state.</summary>
    public bool Redo(DeckOfCards deck)
    {
        ArgumentNullException.ThrowIfNull(deck);
        if (!CanRedo)
        {
            return false;
        }
        var snapshot = _redo.Pop();
        _undo.Push(new CardsMemento(deck.Snapshot()));
        deck.RestoreFrom(snapshot.Cards);
        return true;
    }

    public void Clear()
    {
        _undo.Clear();
        _redo.Clear();
    }
}

