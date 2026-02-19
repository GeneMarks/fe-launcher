using CommunityToolkit.Mvvm.ComponentModel;

namespace FELauncher.UI.Desktop.ViewModels.Navigation
{
    internal sealed class NavItem
    {
        public string Title { get; init; } = "";
        public ObservableObject Content { get; init; } = default!;
    }
}
