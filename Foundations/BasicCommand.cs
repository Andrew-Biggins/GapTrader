using System;
using System.Windows.Input;

namespace TradingSharedCore
{
    public sealed class BasicCommand : ICommand
    {
        public event EventHandler CanExecuteChanged { add { } remove { } }

        public BasicCommand(Action a) => _action = a ?? throw new ArgumentNullException(nameof(a));

        public bool CanExecute(object parameter) => true;

        public void Execute(object parameter) => _action();

        private readonly Action _action;
    }
}
