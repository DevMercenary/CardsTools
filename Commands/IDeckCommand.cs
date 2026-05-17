using CardsTools.Data.Models;
using CardsTools.Data.Tools;

namespace CardsTools.Commands;

/// <summary>
/// A single user-driven mutation against a <see cref="DeckOfCards"/>. Every
/// destructive operation goes through one of these so the
/// <see cref="DeckHistory"/> can roll it back as a single undo step.
/// </summary>
public interface IDeckCommand
{
    string Description { get; }
    OperationResult Apply(DeckOfCards deck);
}

/// <summary>Convenience base that records the memento before <see cref="Apply"/>.</summary>
public abstract class DeckCommand : IDeckCommand
{
    public abstract string Description { get; }

    /// <summary>
    /// Apply the command. The runner is responsible for saving the memento
    /// onto the history *before* this is called.
    /// </summary>
    public abstract OperationResult Apply(DeckOfCards deck);
}
