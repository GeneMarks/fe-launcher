using FELauncher.Shared.Contracts.UI.Desktop;

namespace FELauncher.UI.Desktop.Services
{
    internal sealed class SettingsWindowService(WpfService Ui) : ISettingsWindowService
    {
        private SettingsWindow? _window;

        public async Task ShowWindowAsync()
        {
            await Ui.InvokeAsync(() =>
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
