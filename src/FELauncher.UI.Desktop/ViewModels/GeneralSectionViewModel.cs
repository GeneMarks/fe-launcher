using CommunityToolkit.Mvvm.ComponentModel;
using FELauncher.Shared.Contracts.Settings;

namespace FELauncher.UI.Desktop.ViewModels
{
    public sealed partial class GeneralSectionViewModel : ObservableObject
    {
        [ObservableProperty] private bool _startWithWindows;
        [ObservableProperty] private bool _autoLaunchSession;
        [ObservableProperty] private bool _disableNotifications;
        [ObservableProperty] private int _endSessionGracePeriod;

        public void LoadFrom(FELauncherSettings settings)
        {
            StartWithWindows = settings.StartWithWindows;
            AutoLaunchSession = settings.AutoLaunchSession;
            DisableNotifications = settings.DisableNotifications;
            EndSessionGracePeriod = settings.EndSessionGracePeriod;
        }

        public void ApplyTo(FELauncherSettings settings)
        {
            settings.StartWithWindows = StartWithWindows;
            settings.AutoLaunchSession = AutoLaunchSession;
            settings.DisableNotifications = DisableNotifications;
            settings.EndSessionGracePeriod = EndSessionGracePeriod;
        }
    }
}
