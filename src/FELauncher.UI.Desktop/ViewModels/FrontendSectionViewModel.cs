using CommunityToolkit.Mvvm.ComponentModel;
using FELauncher.Shared.Contracts.Settings;
using MvvmDialogs;

namespace FELauncher.UI.Desktop.ViewModels
{
    internal sealed partial class FrontendSectionViewModel : ObservableObject
    {
        private readonly IDialogService _dialogService;

        [ObservableProperty]
        private ProcessSettingsUserControlViewModel? _frontendSettingsViewModel;

        public FrontendSectionViewModel(IDialogService dialogService)
        {
            _dialogService = dialogService;
        }

        public void LoadFrom(FELauncherSettings settings)
        {
            var frontendSettings = settings.Frontend ?? new ProcessSettings();
            FrontendSettingsViewModel = new ProcessSettingsUserControlViewModel(_dialogService, frontendSettings);
        }

        public void ApplyTo(FELauncherSettings settings)
        {
            settings.Frontend = FrontendSettingsViewModel?.ProcessSettings ?? new ProcessSettings();
        }
    }
}
