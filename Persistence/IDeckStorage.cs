using CardsTools.Data.Models;

namespace CardsTools.Persistence;

/// <summary>
/// Persistence boundary for decks. Implementations decide where bytes go
/// (filesystem, embedded resource, in-memory for tests, …).
/// </summary>
public interface IDeckStorage
{
    Task SaveAsync(DeckOfCards deck, CancellationToken ct = default);
    Task<DeckOfCards?> LoadAsync(string path, CancellationToken ct = default);
    IReadOnlyList<string> ListSaved();
}
