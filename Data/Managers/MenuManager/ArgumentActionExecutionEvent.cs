namespace CardsTools.Data.Managers.MenuManager
{
    public class ArgumentActionExecutionEvent : EventArgs
    {
        public ArgumentActionExecutionEvent(ConsoleKeyInfo input, params object[] args)
        {
            Input = input;
            Args = args;
        }
        public ConsoleKeyInfo Input { get; }
        public object[] Args { get; }
    }
}
