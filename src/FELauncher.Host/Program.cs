using FELauncher.Engine.IO;
using FELauncher.Engine.Processes;
using FELauncher.Engine.Settings;
using FELauncher.Host.Bootstrap;
using FELauncher.Host.Tray;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;


class Program
{
    private const string MutexName = @"Global\5fa78146-d6da-4f0a-954d-7aff5ebc3106";

    [STAThread]
    static void Main(string[] args)
    {
        using var mutex = new Mutex(false, MutexName);
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
        const string SettingsFile = "felauncher.json";
        FELauncherSettingsBootstrapper.EnsureSettingsFileExists(SettingsFile);

        var builder = Host.CreateApplicationBuilder();

        builder.Configuration.Sources.Clear();
        builder.Configuration
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile(SettingsFile, optional: false, reloadOnChange: true);

        builder.Services.Configure<FrontendSettings>(builder.Configuration.GetSection(key: "Frontend"));

        builder.Services.AddSingleton<ITrayController, TrayController>();
        builder.Services.AddHostedService<TrayService>();
        builder.Services.AddSingleton<IPathResolver, PathResolver>();
        builder.Services.AddSingleton<IProcessManager, ProcessManager>();

        using IHost host = builder.Build();

        host.Run();
    }
}