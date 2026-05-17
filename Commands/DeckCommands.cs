using CardsTools.Data.Models;

namespace CardsTools.Commands;

public sealed class AddCardCommand(string name, string description, int power) : DeckCommand
{
    public override string Description => $"add card '{name}'";

    public override OperationResult Apply(DeckOfCards deck) =>
        deck.AddCard(name, description, power);
}

public sealed class RemoveCardCommand(string name) : DeckCommand
{
    public override string Description => $"remove card '{name}'";

    public override OperationResult Apply(DeckOfCards deck) =>
        deck.RemoveCard(name);
}

public sealed class ClearDeckCommand : DeckCommand
{
    public override string Description => "clear deck";

    public override OperationResult Apply(DeckOfCards deck)
    {
        deck.Clear();
        return OperationResult.Success();
    }
}

public sealed class SortByIdCommand : DeckCommand
{
    public override string Description => "sort by id";

    public override OperationResult Apply(DeckOfCards deck)
    {
        deck.SortById();
        return OperationResult.Success();
    }
}

public sealed class SortByPowerCommand : DeckCommand
{
    public override string Description => "sort by power";

    public override OperationResult Apply(DeckOfCards deck)
    {
        deck.SortByPower();
        return OperationResult.Success();
    }
}

public sealed class ShuffleCommand : DeckCommand
{
    public override string Description => "shuffle";

    public override OperationResult Apply(DeckOfCards deck)
    {
        deck.Shuffle();
        return OperationResult.Success();
    }
}
