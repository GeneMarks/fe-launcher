namespace FELauncher.Shared.Contracts.Settings
{
    public sealed class ProcessSettings
    {
        public string Path { get; set; } = string.Empty;
        public string? Arguments { get; set; }
        public int DelaySeconds { get; set; }
        public bool NotifyOnExit { get; set; }
        public bool EndSessionOnExit { get; set; }

        public ProcessSettings() { }

        public ProcessSettings(ProcessSettings ps)
        {
            Path = ps.Path;
            Arguments = ps.Arguments;
            DelaySeconds = ps.DelaySeconds;
            NotifyOnExit = ps.NotifyOnExit;
            EndSessionOnExit = ps.EndSessionOnExit;
        }
    }
}
