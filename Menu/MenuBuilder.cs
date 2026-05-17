using CardsTools.Data.Managers.MenuManager;

namespace CardsTools.Menu;

/// <summary>
/// Fluent builder for the existing <see cref="IMenu"/> framework. Lets the
/// program describe its menu tree declaratively instead of attaching event
/// handlers in nested callbacks the way the original Program.cs did.
/// </summary>
public static class MenuBuilder
{
    public static MenuNode Root(string header) =>
        new(new Data.Managers.MenuManager.Menu(header));

    public sealed class MenuNode
    {
        private readonly Data.Managers.MenuManager.Menu _menu;

        internal MenuNode(Data.Managers.MenuManager.Menu menu) => _menu = menu;

        /// <summary>Add a leaf menu item that runs <paramref name="action"/> when chosen.</summary>
        public MenuNode Item(string header, Action action)
        {
            ArgumentNullException.ThrowIfNull(action);
            _menu.AddItem(header, (_, _) => action());
            return this;
        }

        /// <summary>Add a leaf menu item that runs an async action synchronously.</summary>
        public MenuNode Item(string header, Func<Task> action)
        {
            ArgumentNullException.ThrowIfNull(action);
            _menu.AddItem(header, (_, _) => action().GetAwaiter().GetResult());
            return this;
        }

        /// <summary>Add a sub-menu populated by <paramref name="configure"/>.</summary>
        public MenuNode Submenu(string header, Action<MenuNode> configure)
        {
            ArgumentNullException.ThrowIfNull(configure);
            var child = new Data.Managers.MenuManager.Menu(header) { MenuItemObject = header };
            configure(new MenuNode(child));
            _menu.AddItem(child);
            return this;
        }

        /// <summary>
        /// Add an item that stops the current (sub-)menu. Use it to render
        /// "Back" / "Quit" entries without writing the same boilerplate twice.
        /// </summary>
        public MenuNode Back(string header = "Back")
        {
            _menu.AddItem(header, (sender, _) =>
            {
                var item = (MenuItem)sender!;
                item.Root?.Stop();
            });
            return this;
        }

        /// <summary>Build and run the menu loop.</summary>
        public void Run() => _menu.Run();

        public Data.Managers.MenuManager.Menu Build() => _menu;
    }
}
