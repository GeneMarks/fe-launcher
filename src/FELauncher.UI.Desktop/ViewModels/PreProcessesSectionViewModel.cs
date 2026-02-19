using CommunityToolkit.Mvvm.ComponentModel;
using FELauncher.Shared.Contracts.Settings;
using System.Collections.ObjectModel;

namespace FELauncher.UI.Desktop.ViewModels
{
    internal sealed partial class PreProcessesSectionViewModel : ObservableObject
    {
        public ObservableCollection<ProcessSettings> PreProcesses { get; } = [];

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
            settings.PreProcesses = PreProcesses.ToList();
        }
    }
}
