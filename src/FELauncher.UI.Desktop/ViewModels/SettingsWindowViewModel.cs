using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FELauncher.Shared;
using FELauncher.Shared.Contracts.IO;
using FELauncher.Shared.Contracts.Settings;

namespace FELauncher.UI.Desktop.ViewModels
{
    internal sealed partial class SettingsWindowViewModel(ISettingsStore settingsStore) : ObservableObject
    {
        public event Action? RequestClose;

        private FELauncherSettings? _settings = new();

        public string AppVersion { get; } = AppConstants.AppVersion;

        [ObservableProperty] private bool _startWithWindows;
        [ObservableProperty] private bool _autoLaunchSession;
        [ObservableProperty] private bool _disableNotifications;
        [ObservableProperty] private int _endSessionGracePeriod;

        [ObservableProperty] private string? _frontendPath;
        [ObservableProperty] private string? _frontendArgs;
        [ObservableProperty] private int _frontendDelaySeconds;
        [ObservableProperty] private bool _frontendNotifyOnExit;
        [ObservableProperty] private bool _frontendEndSessionOnExit;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(OkCommand))]
        [NotifyCanExecuteChangedFor(nameof(CancelCommand))]
        private bool _isSaving;

        private bool CanOk => !IsSaving;
        [RelayCommand(CanExecute = nameof(CanOk))]
        private async Task OkAsync()
        {
            if (IsSaving) return;

            IsSaving = true;
            try
            {
                ApplyToSettings();
                await settingsStore.SaveSettingsAsync(_settings!);
            }
            finally
            {
                IsSaving = false;
                RequestClose?.Invoke();
            }
        }

        private bool CanCancel => !IsSaving;
        [RelayCommand(CanExecute = nameof(CanCancel))]
        private void Cancel() => RequestClose?.Invoke();

        public async Task InitializeAsync()
        {
            _settings = await settingsStore.GetSettingsAsync();

            StartWithWindows         = _settings?.StartWithWindows ?? false;
            AutoLaunchSession        = _settings?.AutoLaunchSession ?? false;
            DisableNotifications     = _settings?.DisableNotifications ?? false;
            EndSessionGracePeriod    = _settings?.EndSessionGracePeriod ?? 0;

            FrontendPath             = _settings?.Frontend.Path ?? "";
            FrontendArgs             = _settings?.Frontend.Arguments ?? "";
            FrontendDelaySeconds     = _settings?.Frontend.DelaySeconds ?? 0;
            FrontendNotifyOnExit     = _settings?.Frontend.NotifyOnExit ?? false;
            FrontendEndSessionOnExit = _settings?.Frontend.EndSessionOnExit ?? false;
        }

        private void ApplyToSettings()
        {
            _settings ??= new FELauncherSettings();
            _settings.StartWithWindows      = StartWithWindows;
            _settings.AutoLaunchSession     = AutoLaunchSession;
            _settings.DisableNotifications  = DisableNotifications;
            _settings.EndSessionGracePeriod = EndSessionGracePeriod;

            _settings.Frontend ??= new ProcessSettings();
            _settings.Frontend.Path             = FrontendPath ?? "";
            _settings.Frontend.Arguments        = FrontendArgs;
            _settings.Frontend.DelaySeconds     = FrontendDelaySeconds;
            _settings.Frontend.NotifyOnExit     = FrontendNotifyOnExit;
            _settings.Frontend.EndSessionOnExit = FrontendEndSessionOnExit;
        }
    }
}
