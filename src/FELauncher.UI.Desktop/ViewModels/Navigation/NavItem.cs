using CommunityToolkit.Mvvm.ComponentModel;

namespace FELauncher.UI.Desktop.ViewModels.Navigation
{
    public sealed class NavItem
    {
        public string Title { get; init; } = "";
        public ObservableObject Content { get; init; } = default!;
    }
}
