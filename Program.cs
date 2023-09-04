using CardsTools.Data.Managers;
using CardsTools.Data.Managers.MenuManager;
using CardsTools.Data.Models;
using Serilog.Sinks.SystemConsole.Themes;
using Serilog;
using System.Reflection;

namespace CardsTools
{
    internal class Program
    {
        static CardManager manager = CardManager.GetInstance();
        static Menu mainMenu  = new Menu("Главное меню");
        static Menu cardsPackMenu  = new Menu("Все колоды");
        static void Main(string[] args)
        {
            manager.AddCard(new Card(){Id = 0, Name = "Шут"});
            manager.AddCard(new Card(){Id = 2, Name = "Черви" });
            manager.AddCard(new Card(){Id = 7, Name = "Трефы" });
            mainMenu.AddMenu("Колоды карт");
            IMenu cardsMenu = ((IMenu) mainMenu[0]);
            cardsMenu.AddItem("Создать колоду", ActionCreateCardsPack);
            cardsMenu.AddItem("Все колоды", ActionPrintAllCardsPack);
            mainMenu.AddItem(new MenuItem("Выйти", (sender, @event) =>
            {
                var item = (MenuItem) sender;
                item.Root.Stop();
            }));
            mainMenu.Run();
        }

        private static void ActionPrintAllCardsPack(object? sender, ArgumentActionExecutionEvent e)
        {
            mainMenu.Stop();
            Console.Clear();
            cardsPackMenu.Clear();
            foreach (var card in manager.GetCards())
            {
                cardsPackMenu.AddItem(card.Name, (o, @event) => { });
            }

            cardsPackMenu.AddItem("Назад", (senderChild, @event) =>
            {
                var item = (MenuItem) senderChild;
                item.Root.Stop();
                Console.Clear();
                mainMenu.Run();
            });
            cardsPackMenu.Run();
        }

        private static void ActionCreateCardsPack(object? sender, ArgumentActionExecutionEvent e)
        {

        }
    }
}