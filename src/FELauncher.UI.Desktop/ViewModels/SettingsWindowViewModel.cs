using FELauncher.Shared;
using FELauncher.Shared.Contracts.IO;
using FELauncher.Shared.Contracts.Settings;
using FELauncher.UI.Desktop.MVVM;
using System.Windows.Input;

namespace FELauncher.UI.Desktop.ViewModels
{
    internal sealed class SettingsWindowViewModel(ISettingsStore settingsStore) : ViewModel
    {
        public event EventHandler? OnRequestClose;

        private FELauncherSettings? _settings = new();

        private bool _isSaving;
        public bool IsSaving
        {
            get => _isSaving;
            private set
            {
                if (SetProperty(ref _isSaving, value))
                {
                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        public string AppVersion { get; } = AppConstants.AppVersion;

        private bool _startWithWindows;
        public bool StartWithWindows { get => _startWithWindows; set => SetProperty(ref _startWithWindows, value); }

        private bool _autoLaunchSession;
        public bool AutoLaunchSession { get => _autoLaunchSession; set => SetProperty(ref _autoLaunchSession, value); }

        private bool _disableNotifications;
        public bool DisableNotifications { get => _disableNotifications; set => SetProperty(ref _disableNotifications, value); }

        private int _endSessionGracePeriod;
        public int EndSessionGracePeriod { get => _endSessionGracePeriod; set => SetProperty(ref _endSessionGracePeriod, value); }

        private string? _frontendPath;
        public string? FrontendPath { get => _frontendPath; set => SetProperty(ref _frontendPath, value); }

        private string? _frontendArgs;
        public string? FrontendArgs { get => _frontendArgs; set => SetProperty(ref _frontendArgs, value); }

        private int _frontendDelaySeconds;
        public int FrontendDelaySeconds { get => _frontendDelaySeconds; set => SetProperty(ref _frontendDelaySeconds, value); }

        private bool _frontendNotifyOnExit;
        public bool FrontendNotifyOnExit { get => _frontendNotifyOnExit; set => SetProperty(ref _frontendNotifyOnExit, value); }

        private bool _frontendEndSessionOnExit;
        public bool FrontendEndSessionOnExit { get => _frontendEndSessionOnExit; set => SetProperty(ref _frontendEndSessionOnExit, value); }

        public async Task LoadSettingsAsync()
        {
            _settings = await settingsStore.GetSettingsAsync();

            StartWithWindows = _settings?.StartWithWindows ?? false;
            AutoLaunchSession = _settings?.AutoLaunchSession ?? false;
            DisableNotifications = _settings?.DisableNotifications ?? false;
            EndSessionGracePeriod = _settings?.EndSessionGracePeriod ?? 0;
            FrontendPath = _settings?.Frontend.Path ?? "";
            FrontendArgs = _settings?.Frontend.Arguments ?? "";
            FrontendDelaySeconds = _settings?.Frontend.DelaySeconds ?? 0;
            FrontendNotifyOnExit = _settings?.Frontend.NotifyOnExit ?? false;
            FrontendEndSessionOnExit = _settings?.Frontend.EndSessionOnExit ?? false;
        }

        public void ApplySettings()
        {
            _settings ??= new FELauncherSettings();

            _settings.StartWithWindows = StartWithWindows;
            _settings.AutoLaunchSession = AutoLaunchSession;
            _settings.DisableNotifications = DisableNotifications;
            _settings.EndSessionGracePeriod = EndSessionGracePeriod;
            _settings.Frontend.Path = FrontendPath ?? "";
            _settings.Frontend.Arguments = FrontendArgs;
            _settings.Frontend.DelaySeconds = FrontendDelaySeconds;
            _settings.Frontend.NotifyOnExit = FrontendNotifyOnExit;
            _settings.Frontend.EndSessionOnExit = FrontendEndSessionOnExit;
        }

        private ICommand? _okCommand;
        public ICommand OkCommand
            => _okCommand ??= new AsyncRelayCommand(OkAsync);

        private async Task OkAsync()
        {
            IsSaving = true;
            ApplySettings();
            await settingsStore.SaveSettingsAsync(_settings!);
            IsSaving = false;
            RequestClose();
        }

        private ICommand? _cancelCommand;
        public ICommand CancelCommand
            => _cancelCommand ??= new RelayCommand(
                (_) => !IsSaving,
                (_) => RequestClose());

        private void RequestClose() => OnRequestClose?.Invoke(this, EventArgs.Empty);
    }
}
