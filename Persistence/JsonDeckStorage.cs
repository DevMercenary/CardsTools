using System.Text.Json;
using CardsTools.Data.Models;
using Microsoft.Extensions.Logging;

namespace CardsTools.Persistence;

public sealed partial class JsonDeckStorage(ILogger<JsonDeckStorage> logger) : IDeckStorage
{
    private const string StorageDir = "DeckCardSave";
    private const string BackupDir = "Backup";

    public async Task SaveAsync(DeckOfCards deck, CancellationToken ct = default)
    {
        var dir = EnsureDir(GetStoragePath());
        var primary = Path.Combine(dir, $"{deck.Name}.json");
        var snapshot = new DeckSnapshot(deck.Name, deck.Snapshot());

        await using (var stream = File.Create(primary))
        {
            await JsonSerializer.SerializeAsync(stream, snapshot, CardsJsonContext.Default.DeckSnapshot, ct);
        }

        // Timestamped backup so accidents can be recovered.
        var backupDir = EnsureDir(Path.Combine(dir, BackupDir, deck.Name));
        var backupFile = Path.Combine(backupDir, $"{deck.Name}_{DateTime.UtcNow:yyyyMMddTHHmmss}.json");
        await using (var stream = File.Create(backupFile))
        {
            await JsonSerializer.SerializeAsync(stream, snapshot, CardsJsonContext.Default.DeckSnapshot, ct);
        }

        LogSaved(deck.Name, primary);
    }

    public async Task<DeckOfCards?> LoadAsync(string path, CancellationToken ct = default)
    {
        if (!File.Exists(path))
        {
            return null;
        }
        await using var stream = File.OpenRead(path);
        var snapshot = await JsonSerializer.DeserializeAsync(stream, CardsJsonContext.Default.DeckSnapshot, ct);
        if (snapshot is null)
        {
            return null;
        }
        var deck = new DeckOfCards(snapshot.Name);
        deck.RestoreFrom(snapshot.Cards);
        LogLoaded(snapshot.Name, path);
        return deck;
    }

    public IReadOnlyList<string> ListSaved()
    {
        var dir = EnsureDir(GetStoragePath());
        return [.. Directory
            .GetFiles(dir, "*.json", SearchOption.TopDirectoryOnly)
            .OrderBy(p => p)];
    }

    private static string GetStoragePath() =>
        Path.Combine(Directory.GetCurrentDirectory(), StorageDir);

    private static string EnsureDir(string path)
    {
        Directory.CreateDirectory(path);
        return path;
    }

    [LoggerMessage(EventId = 2001, Level = LogLevel.Information, Message = "Saved deck {Name} to {Path}")]
    private partial void LogSaved(string name, string path);

    [LoggerMessage(EventId = 2002, Level = LogLevel.Information, Message = "Loaded deck {Name} from {Path}")]
    private partial void LogLoaded(string name, string path);
}
