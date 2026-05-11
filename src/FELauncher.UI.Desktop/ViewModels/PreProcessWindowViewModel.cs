using CommunityToolkit.Mvvm.ComponentModel;
using FELauncher.Shared.Contracts.Settings;
using MvvmDialogs;

namespace FELauncher.UI.Desktop.ViewModels
{
    internal sealed partial class PreProcessWindowViewModel : ObservableObject, IModalDialogViewModel
    {
        private bool? _dialogResult;

        public required ProcessSettings ProcessSettings { get; init; }

        public bool? DialogResult
        {
            get => _dialogResult;
            private set => SetProperty(ref _dialogResult, value);
        }
    }
}
