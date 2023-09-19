using CardsTools.Data.Models;

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
            AllCardsList = new List<DeskOfCards?>();
        }
        public static CardManager GetInstance()
        {
            // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
            if (_instance == null)
            {
                lock (_syncRoot)
                {
                    if (_instance == null)
                        _instance = new CardManager();
                }
            }
            return _instance;
        }
        private static CardManager? _instance;
        private static object _syncRoot = new Object();
        public Logger Logger;
        public DeskOfCards? ActiveCardsCollection { get; set; }
        public List<DeskOfCards?> AllCardsList { get; set; }
        public void ChoseActiveCardDesk(string name)
        {
            var cardCollection = AllCardsList.FirstOrDefault(cards => cards?.Name == name);
            if (cardCollection != null)
            {
                ActiveCardsCollection = cardCollection;
                Logger.Information($"Активная колода {name}.");
            }
            else
            {
                Logger.Error($"Колода с  именем {name} не найдена.");
            }

        }
        public void Import(string path)
        {
            var deskImport = OpenDeskCardToStorage(path) ?? throw new InvalidOperationException();
            var cardCollection = AllCardsList.FirstOrDefault(cards => cards?.Name == deskImport.Name);
            if (cardCollection == null)
            {
                AllCardsList.Add(deskImport);
                Logger.Information($"Колода {deskImport.Name} успешно ипортирована.");
            }
            else
            {
                Logger.Warning($"Колода с  именем {deskImport.Name} уже существует.");
                AllCardsList.Add(deskImport);
                deskImport.Name += $"_Backup_{DateTime.Now.ToLongTimeString()}";
                Logger.Information($"Имя колоды изменено {deskImport.Name} успешно ипортирована.");
            }
        }
        internal void CreateNewCardsDesk(string name)
        {
            var cardCollection = AllCardsList.FirstOrDefault(cards => cards?.Name == name);
            if (cardCollection == null)
            {
                AllCardsList.Add(new DeskOfCards(name));
                Logger.Information($"Колода {name} успешно создана.");
            }
            else
            {
                Logger.Error($"Колода с  именем {name} уже существует.");
            }

        }

        internal bool RenameDeskCard(string newName)
        {
            var cardCollection = AllCardsList.FirstOrDefault(cards => cards?.Name == newName);
            if (cardCollection == null)
            {
                ActiveCardsCollection?.Rename(newName);
                Logger.Information($"Колода {newName} успешно переименована.");
                return true;
            }
            else
            {
                Logger.Error($"Колода с  именем {newName} уже существует.");
                return false;
            }
        }
        internal void RemoveDeskCard()
        {
            AllCardsList.Remove(ActiveCardsCollection);
            ActiveCardsCollection = null;
        }


    }
}
