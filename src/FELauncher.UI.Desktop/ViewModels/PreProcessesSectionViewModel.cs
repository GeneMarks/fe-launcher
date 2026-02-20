using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FELauncher.Shared.Contracts.Settings;
using FELauncher.UI.Desktop.Views;
using System.Collections.ObjectModel;

namespace FELauncher.UI.Desktop.ViewModels
{
    internal sealed partial class PreProcessesSectionViewModel : ObservableObject
    {
        public ObservableCollection<ProcessSettings> PreProcesses { get; } = [];

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(EditCommand), nameof(DeleteCommand),
            nameof(MoveDownCommand), nameof(MoveUpCommand))]
        private ProcessSettings? _selectedPreProcess;

        [RelayCommand]
        private void Add()
        {
            var p = new ProcessSettings();
            PreProcesses.Add(p);
            CreatePreProcessWindow(p);
        }

        private bool CanEdit => SelectedPreProcess is not null;
        [RelayCommand(CanExecute = nameof(CanEdit))]
        private void Edit()
            => CreatePreProcessWindow(SelectedPreProcess!);

        private bool CanDelete => SelectedPreProcess is not null;
        [RelayCommand(CanExecute = nameof(CanDelete))]
        private void Delete()
        {
            PreProcesses.Remove(SelectedPreProcess!);
        }

        private bool CanMove => PreProcesses.Count > 1 && SelectedPreProcess is not null;

        [RelayCommand(CanExecute = nameof(CanMove))]
        private void MoveDown()
        {
            int index = PreProcesses.IndexOf(SelectedPreProcess!);

            if (index == PreProcesses.Count - 1)
            {
                PreProcesses.Move(index, 0);
            }
            else
            {
                PreProcesses.Move(index, index + 1);
            }
        }

        [RelayCommand(CanExecute = nameof(CanMove))]
        private void MoveUp()
        {
            int index = PreProcesses.IndexOf(SelectedPreProcess!);

            if (index == 0)
            {
                PreProcesses.Move(index, PreProcesses.Count - 1);
            }
            else
            {
                PreProcesses.Move(index, index - 1);
            }
        }

        private void CreatePreProcessWindow(ProcessSettings processSettings)
        {
            var vm = new PreProcessWindowViewModel
            {
                ProcessSettings = processSettings
            };

            var window = new PreProcessWindow
            {
                DataContext = vm
            };

            window.ShowDialog();
        }

        public void LoadFrom(FELauncherSettings settings)
        {
            PreProcesses.Clear();

            foreach (var preProcess in settings.PreProcesses)
            {
                PreProcesses.Add(new ProcessSettings(preProcess));
            }
        }

        public void ApplyTo(FELauncherSettings settings)
        {
            settings.PreProcesses = [.. PreProcesses];
        }
    }
}
