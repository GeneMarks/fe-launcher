using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FELauncher.Shared;
using FELauncher.Shared.Contracts.IO;
using FELauncher.Shared.Contracts.Settings;
using FELauncher.UI.Desktop.ViewModels.Navigation;
using System.Collections.ObjectModel;

namespace FELauncher.UI.Desktop.ViewModels
{
    public sealed partial class SettingsWindowViewModel : ObservableObject
    {
        public event Action? RequestClose;

        private readonly ISettingsStore _settingsStore;

        public string AppVersion { get; } = AppConstants.AppVersion;
        public ObservableCollection<NavItem> NavItems { get; } = [];
        public GeneralSectionViewModel GeneralSection { get; } = new();
        public FrontendSectionViewModel FrontendSection { get; } = new();
        public PreProcessesSectionViewModel PreProcessesSection { get; } = new();

        [ObservableProperty]
        private NavItem? _selectedNavItem;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(OkCommand))]
        [NotifyCanExecuteChangedFor(nameof(CancelCommand))]
        private bool _isSaving;

        public SettingsWindowViewModel(ISettingsStore settingsStore)
        {
            _settingsStore = settingsStore;

            NavItems.Add(new NavItem { Title = "General", Content = GeneralSection });
            NavItems.Add(new NavItem { Title = "Frontend", Content = FrontendSection });
            NavItems.Add(new NavItem { Title = "Pre-processes", Content = PreProcessesSection });

            SelectedNavItem = NavItems[0];
        }

        private bool CanOk => !IsSaving;
        [RelayCommand(CanExecute = nameof(CanOk))]
        private async Task OkAsync()
        {
            if (IsSaving) return;

            IsSaving = true;
            try
            {
                var settings = BuildSettingsToSave();
                await _settingsStore.SaveSettingsAsync(settings);
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
            var settings = await _settingsStore.GetSettingsAsync() ?? new FELauncherSettings();

            GeneralSection.LoadFrom(settings);
            FrontendSection.LoadFrom(settings);
            PreProcessesSection.LoadFrom(settings);
        }

        private FELauncherSettings BuildSettingsToSave()
        {
            var settings = new FELauncherSettings();

            GeneralSection.ApplyTo(settings);
            FrontendSection.ApplyTo(settings);
            PreProcessesSection.ApplyTo(settings);

            return settings;
        }
    }
}
