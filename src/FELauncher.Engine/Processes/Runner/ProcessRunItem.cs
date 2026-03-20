using FELauncher.Shared.Contracts.Settings;

namespace FELauncher.Engine.Processes.Runner
{
    internal sealed record ProcessRunItem(Process Process, ProcessSettings ProcessSettings);
}
