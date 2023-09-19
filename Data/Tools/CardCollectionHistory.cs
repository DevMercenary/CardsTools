namespace CardsTools.Data.Tools
{
    internal class CardCollectionHistory
    {
        public Stack<CardsMemento> History { get; private set; }

        public CardCollectionHistory()
        {
            History = new();
        }
    }
}
