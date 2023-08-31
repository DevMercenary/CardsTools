using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardsTools.Data.Managers.MenuManager
{
    public interface IMenuItem
    {
        // 
        IMenu? RootMenu { get; }
        IMenu? Root { get; set; }
        string Header { get; set; }
        object? MenuItemObject { get; set; }
        event EventHandler<ArgumentActionExecutionEvent> ArgumentActionExecution;
        event EventHandler<ArgumentActionExecutionEvent> ArgumentActionExecuted;
        void ActionExecute(ConsoleKeyInfo input, params object[] args);
        bool IsValid(IEnumerable<IMenuItem?> itemCheck);
        string GetParent();
    }
}
