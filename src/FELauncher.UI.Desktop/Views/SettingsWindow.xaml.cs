using FELauncher.UI.Desktop.ViewModels;
using System.Windows;

namespace FELauncher.UI.Desktop.Views
{
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InitializeComponent();
        }

        private void SettingsWindow_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
        {
            if (DataContext is SettingsWindowViewModel vm && vm.IsSaving)
            {
                e.Cancel = true;
            }
        }

        private void SettingsWindow_MouseDown(object? sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ChangedButton == System.Windows.Input.MouseButton.Left)
            {
                DragMove();
            }
        }
    }
}
