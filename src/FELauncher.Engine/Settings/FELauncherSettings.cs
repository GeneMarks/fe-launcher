namespace FELauncher.Engine.Settings
{
    public sealed class FELauncherSettings
    {
        public string? LogLevel { get; set; }
        public int? EndSessionGracePeriod { get; set; }
        public FrontendSettings Frontend { get; set; } = new();
        public List<PreProcessSettings> PreProcesses { get; set; } = [];
    }
}