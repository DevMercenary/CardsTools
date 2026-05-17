using System.Text.Json.Serialization;
using CardsTools.Data.Models;

namespace CardsTools.Persistence;

/// <summary>
/// Source-generated System.Text.Json context. Lets the persistence layer
/// serialise without reflection — AOT-friendly and faster than reflection.
/// </summary>
[JsonSourceGenerationOptions(
    WriteIndented = true,
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull)]
[JsonSerializable(typeof(DeckSnapshot))]
[JsonSerializable(typeof(Card))]
internal sealed partial class CardsJsonContext : JsonSerializerContext
{
}

/// <summary>
/// Wire-format for a persisted deck. Keeps the on-disk representation
/// independent of the in-memory <see cref="DeckOfCards"/> shape so the type
/// can evolve without breaking saved files.
/// </summary>
public sealed record DeckSnapshot(string Name, IReadOnlyList<Card> Cards);
