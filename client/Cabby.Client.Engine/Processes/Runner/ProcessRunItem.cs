using Cabby.Client.Shared.Contracts.Configuration;

namespace Cabby.Client.Engine.Processes.Runner
{
    internal sealed record ProcessRunItem(Process Process, ProcessSettings ProcessSettings);
}
