using CommunityToolkit.Mvvm.ComponentModel;
using FELauncher.Shared.Contracts.Settings;

namespace FELauncher.UI.Desktop.ViewModels
{
    internal sealed partial class PreProcessWindowViewModel : ObservableObject
    {
        public required ProcessSettings ProcessSettings { get; init; }
    }
}
