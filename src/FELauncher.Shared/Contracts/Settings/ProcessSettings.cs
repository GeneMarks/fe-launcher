namespace FELauncher.Shared.Contracts.Settings
{
    public sealed class ProcessSettings
    {
        public string Path { get; set; } = string.Empty;
        public string? Arguments { get; set; }
        public int DelaySeconds { get; set; }
        public bool NotifyOnExit { get; set; }
        public bool EndSessionOnExit { get; set; }
    }
}
