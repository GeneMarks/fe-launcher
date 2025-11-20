namespace FELauncher.Host.Tray
{
    public interface ITrayController
    {
        void LaunchFrontend();
        void OpenSettings();
        void Exit();
    }
}
