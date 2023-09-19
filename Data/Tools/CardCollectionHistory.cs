using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
