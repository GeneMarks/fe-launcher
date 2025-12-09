namespace FELauncher.Engine.Settings
{
    public sealed class PreProcessSettings
    {
        public string Path { get; set; } = String.Empty;
        public string? Arguments { get; set; }
        public int DelaySeconds { get; set; }

        public bool NotifyOnExit { get; set; }
        public bool EndSessionOnExit { get; set; }
    }
}
