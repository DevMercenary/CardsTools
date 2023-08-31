using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using CardsTools.Data.Models;
using Serilog;
using Serilog.Core;
using Serilog.Sinks.SystemConsole.Themes;

namespace CardsTools.Data.Managers
{
    internal class CardManager
    {
        private static CardManager instance;
        private static object syncRoot = new Object();
        private List<Card> _cards;
        private Logger _logger;

        private CardManager()
        {
            _logger = new LoggerConfiguration().WriteTo.Console(theme: AnsiConsoleTheme.Literate).CreateLogger();
            _cards = new List<Card>();
        }
        public List<Card> GetCards()
        {
            return _cards;
        }
        public void AddCard(Card card)
        {
            _cards.Add(card);
        }
        public static CardManager GetInstance()
        {
            if (instance == null)
            {
                lock (syncRoot)
                {
                    if (instance == null)
                        instance = new CardManager();
                }
            }
            return instance;
        }
    }
}
