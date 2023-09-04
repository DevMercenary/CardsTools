using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardsTools.Data.Managers.MenuManager
{
    public interface IMenu : IMenuItem, IEnumerable<IMenuItem>
    {
        IMenuItem this[int index] { get; set; }
        IMenuItem this[Index index] { get; set; }
        IMenuItem this[object itemObject] { get; set; }
        IReadOnlyList<IMenuItem?> Items { get; }
        (MenuPoint, int) ShowMenu(int previousLines = -1);
        int ExecuteAction(int numItem, ConsoleKeyInfo input, params object[] args);
        bool AddMenu(string title);
        bool AddItem(IMenuItem? item);
        bool AddItem(string header, EventHandler<ArgumentActionExecutionEvent> actionToExecute = default, object? tag = default);
        bool RemoveItem(IMenuItem? item);
        void Clear();
        int SelectedIndex { get; }
        IMenuItem? SelectedItem { get; }
        List<object> Args { get; }
        bool Running { get; }
        void Run();
        void Stop();
    }
}
