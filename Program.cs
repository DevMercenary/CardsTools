using CardsTools.Data.Managers;
using CardsTools.Data.Managers.MenuManager;
using CardsTools.Data.Tools;

namespace CardsTools
{
    internal class Program
    {
        static CardManager? _manager = CardManager.GetInstance();
        static Menu _mainMenu = new Menu("Главное меню");
        static Menu _cardDeskMenu = new Menu("Меню колоды");
        static Menu _cardDeskLoads = new Menu("Сохраненые колоды");
        static Menu _concretDeskCardMenu = new Menu("Операции с колодой: ");

        static void Main()
        {
            _mainMenu.AddMenu("Колоды карт");
            IMenu cardsMenu = ((IMenu)_mainMenu[0]);
            cardsMenu.AddItem("Все колоды", PrintMenuToDeskCard);
            _mainMenu.AddItem(new MenuItem("Выйти", (sender, _) =>
            {
                var item = (MenuItem)sender!;
                item.Root?.Stop();
            }));
            cardsMenu.AddItem("Создать колоду", ActionCreateCardsPack);
            cardsMenu.AddItem("Импорт колоды", (_, _) =>
            {
                _mainMenu.Stop();
                Console.Clear();
                _cardDeskLoads.Clear();
                _cardDeskLoads.AddItem("Назад", (senderChild, _) =>
                {
                    var item = (MenuItem)senderChild!;
                    item.Root?.Stop();
                    Console.Clear();
                    _mainMenu.Run();
                });
                foreach (var filename in FileHelper.ReadFiles())
                {
                    _cardDeskLoads.AddItem(filename, (_, _) =>
                    {
                        _manager?.Import(filename);
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
            if (_manager?.AllCardsList != null)
                foreach (var card in _manager.AllCardsList)
                {
                    _cardDeskMenu.AddItem(card!.Name, (_, _) =>
                    {
                        _manager.ChoseActiveCardDesk(card.Name);
                        Console.WriteLine("Обработка колоды");
                        Thread.Sleep(500);
                        _cardDeskMenu.Stop();
                        Console.Clear();
                        _concretDeskCardMenu.Clear();
                        _concretDeskCardMenu.Header = card.Name;
                        _concretDeskCardMenu.AddItem("О колоде", (_, _) => { card.Display(); });
                        _concretDeskCardMenu.AddItem("Добавить карту", (_, _) =>
                        {
                            Console.Write("Введите название карты: ");
                            var cardName = Console.ReadLine();
                            if (ValidationCard.IsStringNull(cardName!))
                            {
                                _manager.Logger.Error("Название карты, не может быть пустым. Отмена создание карты.");
                                return;
                            }

                            Console.Write("Введите описание карты: ");
                            var cardDiscription = Console.ReadLine();
                            if (ValidationCard.IsStringNull(cardDiscription!))
                            {
                                _manager.Logger.Error("Описание карты, не может быть пустым. Отмена создание карты.");
                                return;
                            }

                            Console.Write("Введите значение силы карты: ");
                            string cardPower = Console.ReadLine()!;
                            if (!ValidationCard.IsNumeric(cardPower))
                            {
                                _manager.Logger.Error(
                                    "Сила карты имеет численое занчение, указанное арабскими цифрами. Отмена создание карты.");
                                return;
                            }

                            int cardIntengerPower = Convert.ToInt32(cardPower);
                            _manager.ActiveCardsCollection!.AddCardToDesk(cardName!, cardDiscription!, cardIntengerPower);
                        });
                        _concretDeskCardMenu.AddItem("Удалить карту", (_, _) =>
                        {
                            Console.Write("Введите название карты: ");
                            _manager.ActiveCardsCollection!.RemoveCardToDesk(Console.ReadLine()!);
                        });
                        _concretDeskCardMenu.AddItem("Очистить колоду",
                            (_, _) => { _manager.ActiveCardsCollection!.ClearCard(); });
                        _concretDeskCardMenu.AddItem("Сортировать колоду по порядку",
                            (_, _) => { _manager.ActiveCardsCollection!.CardDeskSortById(); });
                        _concretDeskCardMenu.AddItem("Сортировать колоду по силе карт",
                            (_, _) => { _manager.ActiveCardsCollection!.CardDeskSortByPower(); });
                        _concretDeskCardMenu.AddItem("Сортировать колоду случайно",
                            (_, _) => { _manager.ActiveCardsCollection!.CardDeskRandomSort(); });
                        _concretDeskCardMenu.AddItem("Переименовать колоду", (sender1, _) =>
                        {
                            Console.Write("Введите название колоды: ");
                            var deskName = Console.ReadLine();
                            if (ValidationCard.IsStringNull(deskName!))
                            {
                                _manager.Logger.Error("Название колоды, не может быть пустым. Отмена создание карты.");
                                return;
                            }

                            if (_manager.RenameDeskCard(deskName!))
                            {
                                var item = (MenuItem)sender1!;
                                item.Root!.Stop();
                                Console.Clear();
                                _mainMenu.Run();
                            }
                        });
                        _concretDeskCardMenu.AddItem("Удалить колоду", (sender1, _) =>
                        {
                            Console.Write("Нажмите Y для удаления, любую другую для отмены: ");
                            ConsoleKeyInfo inputKey = Console.ReadKey();
                            switch (inputKey.Key)
                            {
                                case ConsoleKey.Y:
                                    var item = (MenuItem)sender1!;
                                    item.Root!.Stop();
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
                        _concretDeskCardMenu.AddItem("Экспорт колоды (.json)",
                            (_, _) => _manager.SaveDeskCardToStorage(_manager.ActiveCardsCollection));
                        _concretDeskCardMenu.AddItem("Назад", (senderChild, _) =>
                        {
                            var item = (MenuItem)senderChild!;
                            item.Root!.Stop();
                            Console.Clear();
                            _cardDeskMenu.Run();
                        });
                        _concretDeskCardMenu.Run();
                    });
                }

            _cardDeskMenu.AddItem("Назад", (senderChild, _) =>
            {
                var item = (MenuItem)senderChild!;
                item.Root!.Stop();
                Console.Clear();
                _mainMenu.Run();
            });
            _cardDeskMenu.Run();
        }
        private static void ActionCreateCardsPack(object? sender, ArgumentActionExecutionEvent e)
        {
            Console.Write("Введите название колоды: ");
            _manager!.CreateNewCardsDesk(Console.ReadLine()!);
        }
    }
}