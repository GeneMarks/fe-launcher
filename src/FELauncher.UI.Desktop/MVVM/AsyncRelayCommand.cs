using System.Windows.Input;

namespace FELauncher.UI.Desktop.MVVM
{
    internal sealed class AsyncRelayCommand : ICommand
    {
        private readonly Func<Task> _executeAsync;
        private bool _isExecuting;

        public event EventHandler? CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public AsyncRelayCommand(Func<Task> executeAsync)
        {
            _executeAsync = executeAsync;
        }

        public bool CanExecute(object? _)
            => !_isExecuting;

        public async void Execute(object? _)
        {
            if (_isExecuting) return;

            try
            {
                _isExecuting = true;
                CommandManager.InvalidateRequerySuggested();

                await _executeAsync();
            }
            finally
            {
                _isExecuting = false;
                CommandManager.InvalidateRequerySuggested();
            }
        }
    }
}
