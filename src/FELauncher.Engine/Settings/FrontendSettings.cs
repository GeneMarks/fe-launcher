namespace FELauncher.Engine.Settings
{
    public sealed class FrontendSettings
    {
        public string Path { get; set; } = String.Empty;
        public string? Arguments { get; set; }
        public int DelaySeconds { get; set; }
    }
}
