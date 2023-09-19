namespace CardsTools.Data.Managers.MenuManager
{
    public class MenuItem : IMenuItem
    {
        public MenuItem(string header = "<insert header>", EventHandler<ArgumentActionExecutionEvent>? actionToExecute = null)
        {
            Header = header;
            if (actionToExecute != null)
                ActionToExecute += actionToExecute;
        }
        public IMenu? RootMenu
        {
            get
            {
                IMenu? result;
                for (result = Root; result?.Root != null; result = result.Root) { }
                return result;
            }
        }
        public IMenu? Root { get; set; }
        public string Header { get; set; }
        public object? MenuItemObject { get; set; }
        public event EventHandler<ArgumentActionExecutionEvent>? ActionToExecute;
        public event EventHandler<ArgumentActionExecutionEvent>? ArgumentActionExecution;
        public event EventHandler<ArgumentActionExecutionEvent>? ArgumentActionExecuted;

        public void ActionExecute(ConsoleKeyInfo userInput, params object[] args)
        {
            var actionArgs = new ArgumentActionExecutionEvent(userInput, args);
            ArgumentActionExecuted?.Invoke(this, actionArgs);
            ActionToExecute?.Invoke(this, actionArgs);
            ArgumentActionExecution?.Invoke(this, actionArgs);
        }

        public bool IsValid(IEnumerable<IMenuItem?> menuToCheck) => true;

        public string GetParent()
        {
            List<string> directories = new();
            for (IMenuItem? current = this; current != null; current = current.Root)
            {
                directories.Add(current.Header);
            }

            directories.Reverse();
            return string.Join(@"\", directories) + ".item";
        }
    }
}
