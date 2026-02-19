using FELauncher.Shared.Contracts.IO;
using FELauncher.Shared.Contracts.UI;
using FELauncher.Shared.Contracts.UI.Windows;
using FELauncher.UI.Desktop.ViewModels;
using FELauncher.UI.Desktop.Views;

namespace FELauncher.UI.Desktop.Services.Windows
{
    internal sealed class SettingsWindowService(
        IUiDispatcher ui,
        ISettingsStore settingsStore) : IWindowService
    {
        private SettingsWindow? _window;
        private SettingsWindowViewModel? _vm;

        public async Task ShowWindowAsync()
        {
            if (_vm is null)
            {
                _vm = new SettingsWindowViewModel(settingsStore);
                await _vm.InitializeAsync().ConfigureAwait(false);
            }

            await ui.InvokeAsync(() =>
            {
                if (_window is null)
                {
                    _window = new SettingsWindow()
                    {
                        DataContext = _vm
                    };

                    _vm.RequestClose += () => _window.Close();

                    _window.Closed += (_, _) =>
                    {
                        _window = null;
                        _vm = null;
                    };
                }

                if (_window.WindowState == System.Windows.WindowState.Minimized)
                {
                    _window.WindowState = System.Windows.WindowState.Normal;
                }

                _window.Show();
                _window.Activate();
            });
        }
    }
}
