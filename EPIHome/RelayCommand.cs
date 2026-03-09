using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace EPIHome
{
    public class RelayCommand<T> : ICommand
    {
        private readonly Action<T> _excute;
        private readonly Predicate<T> _canExcute;
        public RelayCommand(Action<T> excute, Predicate<T> canExcute = null)
        {
            this._excute = excute;
            this._canExcute = canExcute;
        }
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
        public bool CanExecute(object parameter)
        {
            return _canExcute?.Invoke((T)parameter) ?? true;
        }
        public void Execute(object parameter)
        {
            _excute?.Invoke((T)parameter);
        }
    }
    public sealed class AsyncRelayCommand : ICommand
    {
        private readonly Func<Task> _execute;
        private readonly Func<bool> _canExecute;
        private bool _isRunning;
        public event EventHandler CanExecuteChanged;
        public AsyncRelayCommand(Func<Task> execute, Func<bool> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }
        public bool CanExecute(object parameter)
        {
            return !_isRunning && (_canExecute == null || _canExecute());
        }
        public async void Execute(object parameter)
        {
            if (!CanExecute(parameter)) return;
            try
            {
                _isRunning = true;
                RaiseCanExecuteChanged();
                await _execute();
            }
            finally
            {
                _isRunning = false;
                RaiseCanExecuteChanged();
            }
        }
        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
