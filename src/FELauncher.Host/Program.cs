using FELauncher.Engine;
using FELauncher.Host;
using FELauncher.Host.Bootstrap;
using FELauncher.Host.Exceptions;
using FELauncher.UI.Shell;
using FELauncher.UI.Shell.TaskDialog;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using Serilog.Templates;

class Program
{
    [STAThread]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter")]
    static int Main(string[] args)
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

            if (!hasHandle) return 0;

            return RunFELauncher();
        }
        finally
        {
            if (hasHandle)
            {
                mutex.ReleaseMutex();
            }
        }
    }

    private static int RunFELauncher()
    {
        try
        {
            AppDataBootstrapper.EnsureAppDataInitialized();
        }
        catch (AppDataBootstrapException ex)
        {
            ErrorDialog.ShowFatal(
                "An FE Launcher startup error occurred",
                ex.Message,
                ex.InnerException);
            return 1;
        }

        IConfigurationRoot config;
        try
        {
            config = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile(HostPaths.SettingsFile, optional: false, reloadOnChange: true)
                .Build();
        }
        catch (Exception ex)
        {
            ErrorDialog.ShowFatal(
                "An FE Launcher startup error occurred",
                "Failed to load settings file.", ex);
            return 1;
        }

        try
        {
            var logEventLevel = ParseLogEventLevel(config["FELauncher:LogLevel"]);

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Is(logEventLevel)
                // These two are logging unwanted info entries
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .MinimumLevel.Override("System", LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .WriteTo.File(
                    new ExpressionTemplate(
                        "{@t:yyyy-MM-dd HH:mm:ss.fff} [{@l:u3}]{#if SessionId is not null} [SID:{SessionId}]{#end} {@m}\n{@x}"),
                    path: HostPaths.LogFile,
                    rollOnFileSizeLimit: true,
                    fileSizeLimitBytes: HostConstants.LogFileSizeLimit,
                    retainedFileCountLimit: HostConstants.LogFileCountLimit)
                .CreateLogger();
        }
        catch (Exception ex)
        {
            ErrorDialog.ShowFatal(
                "An FE Launcher startup error occurred",
                "Failed to initialize file logging.", ex);
            return 1;
        }

        try
        {
            var builder = Host.CreateApplicationBuilder();

            builder.Configuration.Sources.Clear();
            builder.Configuration.AddConfiguration(config);

            builder.Logging.ClearProviders();
            builder.Logging.AddSerilog();

            builder.Services.ConfigureEngineSettings(builder.Configuration);
            builder.Services.AddEngineServices();
            builder.Services.AddShellServices();

            using IHost host = builder.Build();

            host.Run();
            return 0;
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "An unexpected fatal error occured.");
            ErrorDialog.ShowFatal(
                "An FE Launcher error occurred",
                "An unexpected fatal error occured.\nCheck logs for more details.", ex);
            return 1;
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }

    private static LogEventLevel ParseLogEventLevel(string? level)
    {
        if (Enum.TryParse(
            level, ignoreCase: true, out LogEventLevel logEventLevel))
        {
            return logEventLevel;
        }

        return LogEventLevel.Information;
    }
}