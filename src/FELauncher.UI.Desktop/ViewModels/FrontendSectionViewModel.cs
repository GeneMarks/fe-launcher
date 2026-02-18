using CommunityToolkit.Mvvm.ComponentModel;
using FELauncher.Shared.Contracts.Settings;

namespace FELauncher.UI.Desktop.ViewModels
{
    public sealed partial class FrontendSectionViewModel : ObservableObject
    {
        [ObservableProperty] private string? _frontendPath;
        [ObservableProperty] private string? _frontendArgs;
        [ObservableProperty] private int _frontendDelaySeconds;
        [ObservableProperty] private bool _frontendNotifyOnExit;
        [ObservableProperty] private bool _frontendEndSessionOnExit;

        public void LoadFrom(FELauncherSettings settings)
        {
            FrontendPath = settings.Frontend.Path;
            FrontendArgs = settings.Frontend.Arguments;
            FrontendDelaySeconds = settings.Frontend.DelaySeconds;
            FrontendNotifyOnExit = settings.Frontend.NotifyOnExit;
            FrontendEndSessionOnExit = settings.Frontend.EndSessionOnExit;
        }

        public void ApplyTo(FELauncherSettings settings)
        {
            settings.Frontend.Path = FrontendPath ?? "";
            settings.Frontend.Arguments = FrontendArgs;
            settings.Frontend.DelaySeconds = FrontendDelaySeconds;
            settings.Frontend.NotifyOnExit = FrontendNotifyOnExit;
            settings.Frontend.EndSessionOnExit = FrontendEndSessionOnExit;
        }
    }
}
