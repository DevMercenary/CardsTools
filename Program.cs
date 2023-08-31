using CardsTools.Data.Managers;
using CardsTools.Data.Managers.MenuManager;
using CardsTools.Data.Models;
using Serilog.Sinks.SystemConsole.Themes;
using Serilog;

namespace CardsTools
{
    internal class Program
    {
        static CardManager manager = CardManager.GetInstance();
        static void Main(string[] args)
        {

            var mm = new Menu("Главное меню")
            {
                new Menu("Управление колодами")
                {

                    new MenuItem("Создать колоду", (sender, args) => {
                        System.Console.WriteLine("Тут создается колода :)");
                    }),
                    new MenuItem("Перемешать", (sender, args) => {
                        System.Console.WriteLine("Перемашали колоду что жопа тресятеся :)");
                    }),
                    new MenuItem("Выйти", (sender, args) => {
                        var thisItem = ((IMenuItem) sender);
                        thisItem.Root.Stop();
                    })
                }
            };
            mm.Run();
        }
        
    }
}