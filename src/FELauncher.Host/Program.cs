using FELauncher.Engine.Processes;
using FELauncher.Host.Tray;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;


class Program
{
    [STAThread]
    static void Main(string[] args)
    {
        HostApplicationBuilder builder = Host.CreateApplicationBuilder();

        builder.Services.AddSingleton<IProcessManager, ProcessManager>();
        builder.Services.AddSingleton<NotifyIconManager>();

        using IHost host = builder.Build();

        host.Services.GetRequiredService<NotifyIconManager>();

        host.Run();
    }
}