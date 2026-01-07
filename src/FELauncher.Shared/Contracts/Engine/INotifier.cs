namespace FELauncher.Shared.Contracts.Engine
{
    public interface INotifier
    {
        /// <summary>
        /// Creates and displays a Windows 10 toast notification to the user.
        /// </summary>
        void Notify(string title, string body);
    }
}
