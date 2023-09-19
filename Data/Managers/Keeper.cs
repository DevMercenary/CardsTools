using CardsTools.Data.Tools;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CardsTools.Data.Models;

namespace CardsTools.Data.Managers
{
    internal abstract class Keeper
    {
        public Keeper()
        {
            _history = new CardCollectionHistory();
        }



        private CardCollectionHistory _history;
        private CardsMemento SaveState(List<Card> cards)
        {
            return new CardsMemento(cards);
        }
        private void RestoreState(CardsMemento cardsMemento, List<Card> cards)
        {
            cards = cardsMemento.Cards;
        }

        public void SaveActiveCardsCollection(List<Card> cards)
        {
            _history.History.Push(SaveState(cards));
        }

        public void RestoreActiveCardsCollection(List<Card> cards)
        {
            cards = _history.History.Pop().Cards;
        }
    }
}
