using CardsTools.Data.Tools;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CardsTools.Data.Models;
using Newtonsoft.Json;

namespace CardsTools.Data.Managers
{
    internal abstract class Keeper
    {
        public Keeper()
        {
            _history = new CardCollectionHistory();
        }

        public virtual void SaveDeskCardToStorage(DeskOfCards desk)
        {
            File.WriteAllText(Path.Combine(FileHelper.GetStoragePath(),$"{desk.Name}.json"), JsonConvert.SerializeObject(desk));
            File.WriteAllText(Path.Combine(FileHelper.GetStorageBackupPath(desk.Name),$"{desk.Name}_{DateTime.Now.ToFileTime()}.json"), JsonConvert.SerializeObject(desk));
        }
        public virtual DeskOfCards? OpenDeskCardToStorage(string path)
        {
            return JsonConvert.DeserializeObject<DeskOfCards>(File.ReadAllText(path));
        }
        private CardCollectionHistory _history;
        private CardsMemento SaveState(List<Card> cards)
        {
            return new CardsMemento(cards);
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
