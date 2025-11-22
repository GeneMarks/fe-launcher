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
    [STAThread]
    static void Main(string[] args)
    {
        const string settingsFile = "felauncher.json";
        FELauncherSettingsBootstrapper.EnsureSettingsFileExists(settingsFile);

        var builder = Host.CreateApplicationBuilder();

        builder.Configuration.Sources.Clear();
        builder.Configuration
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile(settingsFile, optional: false, reloadOnChange: true);

        builder.Services.Configure<FrontendSettings>(builder.Configuration.GetSection(key: "Frontend"));

        builder.Services.AddSingleton<ITrayController, TrayController>();
        builder.Services.AddSingleton<TrayIcon>();
        builder.Services.AddSingleton<TrayMenu>();
        builder.Services.AddSingleton<IPathResolver, PathResolver>();
        builder.Services.AddSingleton<IProcessManager, ProcessManager>();

        using IHost host = builder.Build();

        var trayIcon = host.Services.GetRequiredService<TrayIcon>();
        trayIcon.AddNotifyIcon();

        host.Run();
    }
}