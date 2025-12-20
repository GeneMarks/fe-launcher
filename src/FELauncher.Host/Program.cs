using FELauncher.Engine;
using FELauncher.Host;
using FELauncher.Host.Bootstrap;
using FELauncher.Host.Tray;
using FELauncher.UI.Shell;
using FELauncher.UI.Shell.TaskDialog;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

class Program
{
    [STAThread]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter")]
    static void Main(string[] args)
    {
        using var mutex = new Mutex(false, HostConstants.MutexName);
        bool hasHandle = false;

        try
        {
            try
            {
                hasHandle = mutex.WaitOne(0, false);
            }
            catch (AbandonedMutexException)
            {
                hasHandle = true;
            }

            if (!hasHandle) return;

            RunFELauncher();
        }
        finally
        {
            if (hasHandle)
            {
                mutex.ReleaseMutex();
            }
        }
    }

    private static void RunFELauncher()
    {
        FELauncherSettingsBootstrapper.EnsureSettingsFileExists(HostConstants.SettingsFile);

        var builder = Host.CreateApplicationBuilder();

        builder.Configuration.Sources.Clear();
        builder.Configuration
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile(HostConstants.SettingsFile, optional: false, reloadOnChange: true);

        builder.Services.ConfigureEngineSettings(builder.Configuration);
        builder.Services.AddEngineServices();
            builder.Services.AddShellServices();

        using IHost host = builder.Build();

        host.Run();
    }
}