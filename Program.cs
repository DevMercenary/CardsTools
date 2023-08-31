using CardsTools.Data.Managers;
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
            manager.AddCard(new Card() { Id = 0, Name = "Шут" });
            manager.AddCard(new Card() { Id = 1, Name = "Треф" });
            manager.AddCard(new Card() { Id = 2, Name = "Червы" });
            manager.AddCard(new Card() { Id = 3, Name = "Бубны" });
            List<Card> cardCollection = manager.GetCards();
            foreach (var card in cardCollection)
            {
                Console.WriteLine($"{card.Id} - {card.Name}");
            }
        }
        
    }
}