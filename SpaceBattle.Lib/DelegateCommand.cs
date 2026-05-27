namespace SpaceBattle.Lib
{
    public class DelegateCommand : ICommand
    {
        private readonly System.Action _action;
        
        public DelegateCommand(System.Action action)
        {
            _action = action;
        }

        public void Execute()
        {
            _action();
        }
    }
}