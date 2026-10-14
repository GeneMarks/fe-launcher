using CommunityToolkit.Mvvm.ComponentModel;
using MvvmDialogs;

namespace FELauncher.UI.Desktop.ViewModels
{
    internal sealed partial class PreProcessWindowViewModel : ObservableObject, IModalDialogViewModel
    {
        private bool? _dialogResult;

        public ProcessSettingsUserControlViewModel ProcessSettingsViewModel { get; }

        public PreProcessWindowViewModel(ProcessSettingsUserControlViewModel processSettingsViewModel)
        {
            ProcessSettingsViewModel = processSettingsViewModel;
        }

        public bool? DialogResult
        {
            get => _dialogResult;
            private set => SetProperty(ref _dialogResult, value);
        }
    }
}
