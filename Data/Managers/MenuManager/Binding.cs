namespace CardsTools.Data.Managers.MenuManager
{
    internal static class Binding
    {
        public const string FirstLine =
            "Чтобы перейти в предыдущее меню \"ESC\", Что бы выйти из приложения \"Ctrl + C\" ";

        public const ConsoleKey Quit = ConsoleKey.C;
        public const ConsoleKey Back = ConsoleKey.Escape;
        public const ConsoleKey Enter = ConsoleKey.Enter;
        public const ConsoleKey Down = ConsoleKey.DownArrow;
        public const ConsoleKey Up = ConsoleKey.UpArrow;
        public static MenuPoint ProcessAnswer(ConsoleKeyInfo answer)
        {
            return answer.Key switch
            {
                Back => MenuPoint.Back,
                Quit when answer.Modifiers == ConsoleModifiers.Control => MenuPoint.Quit,
                Enter => MenuPoint.Enter,
                Down => MenuPoint.Down,
                Up => MenuPoint.Up,
                _ => MenuPoint.Unknown
            };
        }
    }
}
