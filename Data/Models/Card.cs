namespace CardsTools.Data.Models;

/// <summary>
/// Immutable card. Decks copy cards around (sort, shuffle) but never mutate
/// a card once it has been added — every mutation produces a new record.
/// </summary>
public sealed record Card(int Id, string Name, string Description, int Power)
{
    public override string ToString() =>
        $"#{Id}  {Name}  ({Power})  — {Description}";
}
