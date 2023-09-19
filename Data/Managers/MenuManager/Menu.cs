using System.Collections;
using System.Drawing;

namespace CardsTools.Data.Managers.MenuManager
{
    public class Menu : IMenu
    {
        public IMenuItem this[Index index]
        {
            get => SubItems[index] ?? throw new InvalidOperationException();
            set => SubItems[index] = value;
        }

        public IMenuItem this[int index]
        {
            get => SubItems[index] ?? throw new InvalidOperationException();
            set => SubItems[index] = value;
        }

        public IMenuItem this[object itemObj]
        {
            get => SubItems.First(item => item?.MenuItemObject == itemObj) ?? throw new InvalidOperationException();
            set
            {
                var index = SubItems.FindIndex(item => item?.MenuItemObject == itemObj);
                SubItems[index] = value;
            }
        }

        public List<object> Args { get; } = new();

        public bool Running
        {
            get
            {
                if (_isRoot)
                    return _running;
                else if (Root != null)
                    return Root.Running;
                else
                    return _running;
            }
        }

        private bool _isRoot;
        public void Run()
        {
            _running = true;
            _isRoot = true;
            PositionZero = new Point(Console.CursorLeft, Console.CursorTop + 1);
            ShowMenu(Items.Count);
            _isRoot = false;
        }

        public void Stop()
        {
            _running = false;

            if (!_isRoot && Root != null)
            {
                Root.Stop();
            }
        }

        public IMenu? RootMenu
        {
            get
            {
                IMenu? result;
                for (result = this; result.Root != null; result = result.Root) { }
                return result;
            }
        }
        public IMenu? Root { get; set; }
        public string Header { get; set; }
        public object? MenuItemObject { get; set; }

        protected static Point DefaultConsoleLocation;
        protected int ActionPerformedEndRow = -1;
        private bool _running;
        protected static Point PositionZero = new(Console.CursorLeft, Console.CursorTop + 1);

        public bool RemoveItem(IMenuItem? item) => SubItems.Remove(item);

        public void Clear() => SubItems.Clear();

        public int SelectedIndex { get; private set; } = -1;

        public IMenuItem? SelectedItem => SelectedIndex < 0 || SelectedIndex >= SubItems.Count ? SubItems[SelectedIndex] : null;
        public event EventHandler<ArgumentActionExecutionEvent>? ArgumentActionExecution;
        public event EventHandler<ArgumentActionExecutionEvent>? ArgumentActionExecuted;

        public Menu(string header = "<insert header>",
                    params IMenuItem?[] menuItems)
        {
            Header = header;

            foreach (var menuItem in menuItems)
            {
                AddItem(menuItem);
            }
        }
        public virtual void ActionExecute(ConsoleKeyInfo input, params object[] args)
        {
            var actionArgs = new ArgumentActionExecutionEvent(input, args);
            ArgumentActionExecution?.Invoke(this, actionArgs);
            ShowMenu((int)args[0]);
            ArgumentActionExecuted?.Invoke(this, actionArgs);
        }
        public virtual bool IsValid(IEnumerable<IMenuItem?> menuToCheck) => true;
        public string GetParent() => string.Join(@"\", GetParentsNames());
        private IEnumerable<string> GetParentsNames()
        {
            List<string> directories = new();

            for (IMenuItem? current = this; current != null; current = current.Root)
            {
                directories.Add(current.Header);
            }

            directories.Reverse();
            return directories;
        }
        protected readonly List<IMenuItem?> SubItems = new();
        public IReadOnlyList<IMenuItem?> Items => SubItems.AsReadOnly();
        public virtual (MenuPoint, int) ShowMenu(int previousLines = -1)
        {
            if (!Running) return (MenuPoint.Quit, -1);

            (MenuPoint, int) @return = default;

            do
            {
                ResetPosition();
                PrintFirstLines();
                PrintItems(new Point(Console.CursorLeft, Console.CursorTop), @return.Item2 + previousLines);
                var answer = Console.ReadKey(true);
                @return = TryPerformAction(answer);
            } while (@return.Item1 != MenuPoint.Quit && @return.Item1 != MenuPoint.Back);
            ClearPerformedLines();
            return @return;
        }
        public virtual int ExecuteAction(int numItem, ConsoleKeyInfo userInput, params object[] args)
        {
            ClearPerformedLines();
            Console.CursorVisible = true;
            var subItem = SubItems[numItem];
            subItem?.ActionExecute(userInput, Items.Count, args);
            Console.CursorVisible = false;
            ActionPerformedEndRow = Console.CursorTop;
            if (subItem is IMenu m) return m.Items.Count;
            return -1;

        }
        private void ClearPerformedLines()
        {
            if (ActionPerformedEndRow == -1) return;
            var curr = Console.CursorTop;
            for (var i = 0; i < ActionPerformedEndRow - curr + 1; i++)
            {
                ClearCurrentConsoleLine();
                Console.WriteLine();
            }
            Console.CursorTop = curr;
        }
        public bool AddMenu(string title)
        {
            var menu = new Menu(title) { MenuItemObject = title };
            return AddItem(menu);
        }
        public bool AddItem(IMenuItem? item)
        {
            if (item == null || !item.IsValid(SubItems))
            {
                return false;
            }

            item.Root = this;
            SubItems.Add(item);
            return true;
        }

        public bool AddItem(string header = "<insert header>", EventHandler<ArgumentActionExecutionEvent>? actionToExecute = default, object? tag = default)
        {
            var menuItem = new MenuItem(header, actionToExecute) { MenuItemObject = null };
            return AddItem(menuItem);
        }

        protected void PrintFirstLines()
        {
            ResetPosition();
            Console.ForegroundColor = ConsoleColor.Gray;
            CustomWriteLine(Binding.FirstLine);
            CustomWriteLine();
            PrintPrettyDir();
            CustomWriteLine();
            DefaultConsoleLocation = new Point(Console.CursorLeft, Console.CursorTop);
        }

        private void ResetPosition()
        {
            Console.CursorTop = PositionZero.Y;
            Console.CursorLeft = PositionZero.X;
        }

        private void CustomWriteLine(object? s = null)
        {
            ClearCurrentConsoleLine();
            Console.WriteLine(s);
        }

        private (MenuPoint, int) TryPerformAction(ConsoleKeyInfo answer)
        {
            var @return = Binding.ProcessAnswer(answer);
            var @int = -1;
            switch (@return)
            {
                case MenuPoint.Enter when SelectedIndex != -1:
                    @int = ExecuteAction(SelectedIndex, answer, Args);
                    break;
                case MenuPoint.Quit:
                    Root?.Stop();
                    break;
                case MenuPoint.Up:
                    int max = Items.Count == 0 ? 0 : Items.Count - 1, min = 0;
                    if (SelectedIndex <= min)
                        SelectedIndex = max;
                    else
                        SelectedIndex--;
                    break;
                case MenuPoint.Down:
                    (max, min) = (Items.Count == 0 ? 0 : Items.Count - 1, 0);
                    if (SelectedIndex >= max)
                        SelectedIndex = min;
                    else
                        SelectedIndex++;
                    break;
                case MenuPoint.Back:
                case MenuPoint.Unknown:
                default:
                    break;
            }

            return (!Running ? MenuPoint.Quit : @return, @int);
        }
        protected void PrintPrettyDir()
        {
            Console.ForegroundColor = ConsoleColor.Red;
            CustomWriteLine("Текущая позиция: ");
            Console.ForegroundColor = ConsoleColor.White;
            var parents = GetParentsNames();
            ClearCurrentConsoleLine();
            foreach (var parent in parents)
            {
                if (parent == RootMenu?.Header)
                {
                    Console.BackgroundColor = ConsoleColor.Green;
                    Console.Write(parent);
                }
                else
                {
                    Console.BackgroundColor = ConsoleColor.Blue;
                    Console.Write(parent);
                }

                Console.ResetColor();
                Console.Write(@"\");
            }

            Console.Write(">");
            Console.WriteLine();
        }
        protected void PrintItems(Point consolePosition, int previousLines)
        {
            Console.CursorVisible = false;
            Console.SetCursorPosition(0, DefaultConsoleLocation.Y);
            var max = previousLines > SubItems.Count ? previousLines : SubItems.Count;
            const string format = "> ";
            for (var i = 0; i < max; i++)
            {
                ClearCurrentConsoleLine();
                if (i == SelectedIndex)
                {
                    Console.BackgroundColor = ConsoleColor.White;
                    Console.ForegroundColor = ConsoleColor.Black;
                    Console.Write(format);
                }
                if (i < SubItems.Count)
                {
                    Console.Write($"(#{i + 1}) - {SubItems[i]?.Header} ");
                    if (i != SelectedIndex)
                        Console.Write($"{string.Join("", Enumerable.Repeat(" ", format.Length))}");
                    Console.WriteLine();
                }
                else
                {
                    Console.WriteLine();
                }

                Console.ResetColor();
            }
        }
        public static void ClearCurrentConsoleLine()
        {
            var currentLineCursor = Console.CursorTop;
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.Write(new string(' ', Console.WindowWidth));
            Console.SetCursorPosition(0, currentLineCursor);
        }

        /// <inheritdoc />
        public IEnumerator<IMenuItem> GetEnumerator() => SubItems.GetEnumerator();

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public void Add(IMenuItem menuItem) => AddItem(menuItem);
    }
}
