using CardsTools.Data.Models;

namespace CardsTools.Data.Tools
{
    internal class CardsMemento
    {
        public List<Card> Cards { get; private set; }

        public CardsMemento(List<Card> cards)
        {
            Cards = cards;
        }
    }
}
