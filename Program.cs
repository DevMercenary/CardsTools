using System.ComponentModel.Design;
using CardsTools.Data.Managers;
using CardsTools.Data.Managers.MenuManager;
using CardsTools.Data.Models;
using Serilog.Sinks.SystemConsole.Themes;
using Serilog;
using System.Reflection;
using CardsTools.Data.Tools;

namespace CardsTools
{
    internal class Program
    {
        static CardManager _manager = CardManager.GetInstance();
        static Menu _mainMenu = new Menu("Главное меню");
        static Menu _cardsPackMenu = new Menu("Все колоды");
        static Menu _cardDeskMenu = new Menu("Меню колоды");
        static Menu _cardDeskLoads = new Menu("Сохраненые колоды");
        static Menu _concretDeskCardMenu = new Menu("Операции с колодой: ");

        static void Main(string[] args)
        {
            _mainMenu.AddMenu("Колоды карт");
            IMenu cardsMenu = ((IMenu)_mainMenu[0]);
            cardsMenu.AddItem("Все колоды", PrintMenuToDeskCard);
            _mainMenu.AddItem(new MenuItem("Выйти", (sender, @event) =>
            {
                var item = (MenuItem)sender;
                item.Root.Stop();
            }));
            cardsMenu.AddItem("Создать колоду", ActionCreateCardsPack);
            cardsMenu.AddItem("Импорт колоды", (sender, @event) =>
            {
                _mainMenu.Stop();
                Console.Clear();
                _cardDeskLoads.Clear();
                _cardDeskLoads.AddItem("Назад", (senderChild, @event) =>
                {
                    var item = (MenuItem)senderChild;
                    item.Root.Stop();
                    Console.Clear();
                    _mainMenu.Run();
                });
                foreach (var filename in FileHelper.ReadFiles())
                {
                    _cardDeskLoads.AddItem(filename, (o, executionEvent) =>
                    {
                        _manager.Import(filename);
                    });
                }
                _cardDeskLoads.Run();
            });
            _mainMenu.Run();
        }

        public static void PrintMenuToDeskCard(object? sender, ArgumentActionExecutionEvent e)
        {
            _mainMenu.Stop();
            Console.Clear();
            _cardDeskMenu.Clear();
            foreach (var card in _manager.AllCardsList)
            {
                _cardDeskMenu.AddItem(card.Name, (o, @event) =>
                {
                    _manager.ChoseActiveCardDesk(card.Name);
                    Console.WriteLine("Обработка колоды");
                    Thread.Sleep(500);
                    _cardDeskMenu.Stop();
                    Console.Clear();
                    _concretDeskCardMenu.Clear();
                    _concretDeskCardMenu.Header = card.Name;
                    _concretDeskCardMenu.AddItem("О колоде", (sender1, executionEvent) =>
                    {
                        card.Display();
                    });
                    _concretDeskCardMenu.AddItem("Добавить карту", (sender1, executionEvent) =>
                    {
                        Console.Write("Введите название карты: ");
                        var cardName = Console.ReadLine();
                        if (ValidationCard.IsStringNull(cardName))
                        {
                            _manager.Logger.Error("Название карты, не может быть пустым. Отмена создание карты.");
                            return;
                        }
                        Console.Write("Введите описание карты: ");
                        var cardDiscription  = Console.ReadLine();
                        if (ValidationCard.IsStringNull(cardDiscription))
                        {
                            _manager.Logger.Error("Описание карты, не может быть пустым. Отмена создание карты.");
                            return;
                        }
                        Console.Write("Введите значение силы карты: ");
                        string cardPower = Console.ReadLine();
                        if (!ValidationCard.IsNumeric(cardPower))
                        {
                            _manager.Logger.Error("Сила карты имеет численое занчение, указанное арабскими цифрами. Отмена создание карты.");
                            return;
                        }
                        int cardIntengerPower = Convert.ToInt32(cardPower);
                        _manager.ActiveCardsCollection.AddCardToDesk(cardName, cardDiscription, cardIntengerPower);

                    });
                    _concretDeskCardMenu.AddItem("Удалить карту", (sender1, executionEvent) =>
                    {
                        Console.Write("Введите название карты: ");
                        _manager.ActiveCardsCollection.RemoveCardToDesk(Console.ReadLine());
                    });
                    _concretDeskCardMenu.AddItem("Очистить колоду", (sender1, executionEvent) =>
                    {
                        _manager.ActiveCardsCollection.ClearCard();
                    });
                    _concretDeskCardMenu.AddItem("Сортировать колоду по порядку", (sender1, executionEvent) =>
                    {
                        _manager.ActiveCardsCollection.CardDeskSortById();
                    });
                    _concretDeskCardMenu.AddItem("Сортировать колоду по силе карт", (sender1, executionEvent) =>
                    {
                        _manager.ActiveCardsCollection.CardDeskSortByPower();
                    });
                    _concretDeskCardMenu.AddItem("Сортировать колоду случайно", (sender1, executionEvent) =>
                    {
                        _manager.ActiveCardsCollection.CardDeskRandomSort();

                    });
                    _concretDeskCardMenu.AddItem("Переименовать колоду", (sender1, executionEvent) =>
                    {
                        Console.Write("Введите название колоды: ");
                        var deskName = Console.ReadLine();
                        if (ValidationCard.IsStringNull(deskName))
                        {
                            _manager.Logger.Error("Название колоды, не может быть пустым. Отмена создание карты.");
                            return;
                        }
                        if(_manager.RenameDeskCard(deskName))
                        {
                            var item = (MenuItem)sender1;
                            item.Root.Stop();
                            Console.Clear();
                            _mainMenu.Run();
                        }

                    });
                    _concretDeskCardMenu.AddItem("Удалить колоду", (sender1, executionEvent) =>
                    {
                        Console.Write("Нажмите Y для удаления, любую другую для отмены: ");
                        ConsoleKeyInfo inputKey = Console.ReadKey();
                        switch (inputKey.Key)
                        {
                            case ConsoleKey.Y:
                                var item = (MenuItem) sender1;
                                item.Root.Stop();
                                Console.Clear();
                                _cardDeskMenu.Clear();
                                _manager.RemoveDeskCard();
                                _mainMenu.Run();
                                break;
                            default:
                                _manager.Logger.Information("Отмена удаления колоды.");
                                break;
                        }
                    });
                    _concretDeskCardMenu.AddItem("Экспорт колоды (.json)", (sender1, executionEvent) => _manager.SaveDeskCardToStorage(_manager.ActiveCardsCollection));
                    _concretDeskCardMenu.AddItem("Назад", (senderChild, @event) =>
                    {
                        var item = (MenuItem)senderChild;
                        item.Root.Stop();
                        Console.Clear();
                        _cardDeskMenu.Run();
                    });
                    _concretDeskCardMenu.Run();

                });
            }
            _cardDeskMenu.AddItem("Назад", (senderChild, @event) =>
            {
                var item = (MenuItem)senderChild;
                item.Root.Stop();
                Console.Clear();
                _mainMenu.Run();
            });
            _cardDeskMenu.Run();
        }

        private static void ActionPrintAllCardsPack(object? sender, ArgumentActionExecutionEvent e)
        {
            _mainMenu.Stop();
            Console.Clear();
            _cardsPackMenu.Clear();
            foreach (var card in _manager.AllCardsList)
            {
                _cardsPackMenu.AddItem(card.Name, (o, @event) =>
                {
                    Console.WriteLine($"В колоде {card.Name} - {card.Cards.Count} карт.");
                    card.Display();
                });
            }

            _cardsPackMenu.AddItem("Назад", (senderChild, @event) =>
            {
                var item = (MenuItem)senderChild;
                item.Root.Stop();
                Console.Clear();
                _mainMenu.Run();
            });
            _cardsPackMenu.Run();
        }

        private static void ActionCreateCardsPack(object? sender, ArgumentActionExecutionEvent e)
        {
            Console.Write("Введите название колоды: ");
            _manager.CreateNewCardsDesk(Console.ReadLine());
        }
    }
}