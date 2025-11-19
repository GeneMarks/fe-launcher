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

        HostApplicationBuilder builder = Host.CreateApplicationBuilder();

        builder.Configuration.Sources.Clear();
        builder.Configuration
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile(settingsFile, optional: false, reloadOnChange: true);

        builder.Services.Configure<FrontendSettings>(builder.Configuration.GetSection(key: "Frontend"));
        
        builder.Services.AddSingleton<IPathResolver, PathResolver>();
        builder.Services.AddSingleton<IProcessManager, ProcessManager>();
        builder.Services.AddSingleton<NotifyIconManager>();

        using IHost host = builder.Build();

        host.Services.GetRequiredService<NotifyIconManager>();

        host.Run();
    }
}