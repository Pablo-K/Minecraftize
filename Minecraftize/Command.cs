using System;
using System.Windows.Input;

namespace Minecraftize
{
    internal class Command : ICommand
    {
        private readonly Action<object?> _execute;
        private readonly Func<object?, bool>? _canExecute;
        private ICommand? firstSite;

        public event EventHandler? CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public Command(Action<object?> execute, Func<object?, bool>? canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public Command(ICommand? firstSite)
        {
            this.firstSite = firstSite;
        }

        public bool CanExecute(object? parameter)
        {
            if (_canExecute is null) return true;
            return _canExecute(parameter);
        }

        public void Execute(object? parameter)
        {
            _execute?.Invoke(parameter);
        }

    }
}
