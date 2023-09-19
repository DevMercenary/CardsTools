using Serilog;
using Serilog.Core;
using Serilog.Sinks.SystemConsole.Themes;

namespace CardsTools.Data.Models
{
    public class DeskOfCards
    {
        delegate void SortedHandler();

        private event SortedHandler OnSortedDeskNotify;
        private Logger _logger;
        public DeskOfCards(string name)
        {
            Name = name;
            _logger = new LoggerConfiguration().WriteTo.Console(theme: AnsiConsoleTheme.Literate).CreateLogger();
            _random = new Random();
            IsSorted = false;
            OnSortedDeskNotify += () =>
            {
                IsSorted = true;
            };
        }

        public List<Card> Cards = new();
        public string Name { get; set; }
        public bool IsSorted { get; set; }
        private Random _random;
        public void Display()
        {
            if (Cards.Count != 0)
            {
                _logger.Information($"Колода: {Name}");
                _logger.Information($"Состояние колоды:  {IsSorted}");
                _logger.Information($"Карты: {Cards.Count} шт.");
                foreach (Card card in Cards)
                {
                    _logger.Information($"ID: {card.Id} Название: {card.Name} Сила  карты: {card.CardPower} Описание: {card.Description}");
                }
            }
            else
            {
                _logger.Information("В колоде нет карт.");
            }

        }
        public void CardDeskSortById()
        {
            if (Cards.Count == 0)
            {
                _logger.Information("В колоде нет карт.");
                return;
            }
            var orderByDescending = Cards.OrderBy(p => p.Id);
            Cards = orderByDescending.ToList();
            OnSortedDeskNotify.Invoke();
            _logger.Information("Колода отсортирована по порядку.");
        }
        public void CardDeskSortByPower()
        {
            if (Cards.Count == 0)
            {
                _logger.Information("В колоде нет карт.");
                return;
            }
            var orderByDescending = Cards.OrderByDescending(p => p.CardPower);
            Cards = orderByDescending.ToList();
            OnSortedDeskNotify.Invoke();
            _logger.Information("Колода отсортирована по силе.");
        }
        public void CardDeskRandomSort()
        {

            if (Cards.Count == 0)
            {
                _logger.Information("В колоде нет карт.");
                return;
            }
            int x = Cards.Count;
            while (x > 1)
            {
                x--;
                int r = _random.Next(x + 1);
                (Cards[r], Cards[x]) = (Cards[x], Cards[r]);
            }
            OnSortedDeskNotify.Invoke();
            _logger.Information("Колода отсортирована случайно.");
        }

        public void ClearCard()
        {
            Cards.Clear();
        }
        public void AddCardToDesk(string name, string description, int power)
        {
            if (IsDeskCardsFull(Cards))
            {
                _logger.Error("Активная колода содержит 100 карт, удалите карту для добавления новой.");
            }
            else
            {
                int id = Cards.Count;
                Cards.Add(new Card() { Name = name, Id = id, CardPower = power, Description = description });
                _logger.Information($"Карта {name} успешно добавлена её индентификатор {id}.");
            }
        }

        public void RemoveCardToDesk(string name)
        {
            var card = Cards.FirstOrDefault(card => card.Name == name);
            if (card != null)
            {
                Cards.Remove(card);
                _logger.Information($"Карта {name} успешно удалена.");
            }
            else
            {
                _logger.Error($"Карта с  именем {name} не найдена.");
            }
        }
        public void Rename(string newName)
        {
            Name = newName;
        }
        private bool IsDeskCardsFull(List<Card> cards)
        {
            return cards.Count == 100;
        }
    }
}
