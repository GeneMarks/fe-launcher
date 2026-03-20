using CommunityToolkit.Mvvm.ComponentModel;
using FELauncher.Shared.Contracts.Settings;

namespace FELauncher.UI.Desktop.ViewModels
{
    internal sealed partial class FrontendSectionViewModel : ObservableObject
    {
        [ObservableProperty] private ProcessSettings? _frontendSettings;

        public void LoadFrom(FELauncherSettings settings)
        {
            FrontendSettings = settings.Frontend;
        }

        public void ApplyTo(FELauncherSettings settings)
        {
            settings.Frontend = FrontendSettings ?? new();
        }
    }
}
