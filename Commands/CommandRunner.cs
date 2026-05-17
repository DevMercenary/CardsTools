using CardsTools.Data.Managers;
using CardsTools.Data.Models;
using CardsTools.Data.Tools;
using Microsoft.Extensions.Logging;

namespace CardsTools.Commands;

/// <summary>
/// Glue between <see cref="CardManager"/>, <see cref="IDeckCommand"/> and
/// <see cref="DeckHistory"/>: take a memento *before* applying the command,
/// then run it. Failure rolls the memento back.
/// </summary>
public sealed partial class CommandRunner(CardManager manager, ILogger<CommandRunner> logger)
{
    public OperationResult Run(IDeckCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);

        var deck = manager.ActiveDeck;
        var history = manager.ActiveHistory;
        if (deck is null || history is null)
        {
            return OperationResult.Failure("No active deck.");
        }

        history.Save(deck);
        var result = command.Apply(deck);

        if (!result.Ok)
        {
            // Roll back the memento we just pushed — the command did nothing useful.
            history.Undo(deck);
            LogFailed(command.Description, result.Error ?? "unknown");
        }
        else
        {
            LogApplied(command.Description);
        }
        return result;
    }

    public bool Undo()
    {
        var deck = manager.ActiveDeck;
        var history = manager.ActiveHistory;
        return deck is not null && history is not null && history.Undo(deck);
    }

    public bool Redo()
    {
        var deck = manager.ActiveDeck;
        var history = manager.ActiveHistory;
        return deck is not null && history is not null && history.Redo(deck);
    }

    [LoggerMessage(EventId = 4001, Level = LogLevel.Information, Message = "applied: {Description}")]
    private partial void LogApplied(string description);

    [LoggerMessage(EventId = 4002, Level = LogLevel.Warning, Message = "failed: {Description} ({Error})")]
    private partial void LogFailed(string description, string error);
}
