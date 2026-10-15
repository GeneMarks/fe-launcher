using Cabby.Client.Shared.Contracts.Settings;

namespace Cabby.Client.Engine.Processes.Runner
{
    internal sealed record ProcessRunItem(Process Process, ProcessSettings ProcessSettings);
}
