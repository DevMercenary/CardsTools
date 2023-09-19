using CardsTools.Data.Models;
using CardsTools.Data.Tools;
using Serilog;
using Serilog.Core;
using Serilog.Sinks.SystemConsole.Themes;

namespace CardsTools.Data.Managers
{
    internal class CardManager : Keeper
    {
        private CardManager()
        {
            Logger = new LoggerConfiguration().WriteTo.Console(theme: AnsiConsoleTheme.Literate).CreateLogger();
            AllCardsList = new List<DeskOfCards>();
            ActiveCardsCollection = new DeskOfCards("Empty");
        }
        public static CardManager GetInstance()
        {
            if (_instance == null)
            {
                lock (syncRoot)
                {
                    if (_instance == null)
                        _instance = new CardManager();
                }
            }
            return _instance;
        }
        private static CardManager _instance;
        private static object syncRoot = new Object();
        public Logger Logger;
        public DeskOfCards ActiveCardsCollection { get; set; }
        public List<DeskOfCards> AllCardsList { get; set; }
        public void ChoseActiveCardDesk(string name)
        {
            var cardCollection = AllCardsList.FirstOrDefault(cards => cards.Name == name);
            if (cardCollection != null)
            {
                ActiveCardsCollection.Cards = cardCollection.Cards;
                Logger.Information($"Активная колода {name}.");
            }
            else
            {
                Logger.Error($"Колода с  именем {name} не найдена.");
            }
            
        }

        internal void CreateNewCardsDesk(string? v)
        {
            AllCardsList.Add(new DeskOfCards(v));
        }
    }
}
