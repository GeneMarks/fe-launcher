using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FELauncher.Shared.Contracts.Settings;
using MvvmDialogs;
using MvvmDialogs.FrameworkDialogs.OpenFile;

namespace FELauncher.UI.Desktop.ViewModels
{
    internal sealed partial class ProcessSettingsUserControlViewModel : ObservableObject
    {
        private readonly IDialogService _dialogService;

        public ProcessSettings ProcessSettings { get; }

        public ProcessSettingsUserControlViewModel(IDialogService dialogService, ProcessSettings processSettings)
        {
            _dialogService = dialogService;
            ProcessSettings = processSettings;
        }

        public string Path
        {
            get => ProcessSettings.Path;
            set => SetProperty(
                ProcessSettings.Path,
                value,
                ProcessSettings,
                (model, value) => ProcessSettings.Path = value);
        }

        public string? Arguments
        {
            get => ProcessSettings.Arguments;
            set => SetProperty(
                ProcessSettings.Arguments,
                value,
                ProcessSettings,
                (model, value) => ProcessSettings.Arguments = value);
        }

        public int DelaySeconds
        {
            get => ProcessSettings.DelaySeconds;
            set => SetProperty(
                ProcessSettings.DelaySeconds,
                value,
                ProcessSettings,
                (model, value) => ProcessSettings.DelaySeconds = value);
        }

        public bool NotifyOnExit
        {
            get => ProcessSettings.NotifyOnExit;
            set => SetProperty(
                ProcessSettings.NotifyOnExit,
                value,
                ProcessSettings,
                (model, value) => ProcessSettings.NotifyOnExit = value);
        }

        public bool EndSessionOnExit
        {
            get => ProcessSettings.EndSessionOnExit;
            set => SetProperty(
                ProcessSettings.EndSessionOnExit,
                value,
                ProcessSettings,
                (model, value) => ProcessSettings.EndSessionOnExit = value);
        }

        [RelayCommand]
        private void ShowOpenFileDialog()
        {
            var dialogSettings = new OpenFileDialogSettings
            {
                CheckFileExists = true,
                Multiselect = false,
                ReadOnlyChecked = false,
                ShowReadOnly = false
            };

            bool? success = _dialogService.ShowOpenFileDialog(this, dialogSettings);

            if (success == true)
            {
                Path = dialogSettings.FileName;
            }
        }
    }
}
