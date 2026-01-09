using FELauncher.Shared.Contracts.UI.Desktop;
using FELauncher.Shared.Contracts.UI.Desktop.Windows;

namespace FELauncher.UI.Desktop.Services.Windows
{
    internal sealed class SettingsWindowService(IUiDispatcher ui) : ISettingsWindowService
    {
        private SettingsWindow? _window;

        public async Task ShowWindowAsync()
        {
            await ui.InvokeAsync(() =>
            {
                if (_window is null)
                {
                    _window = new SettingsWindow();
                    _window.Closed += (_, _) => _window = null;
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
