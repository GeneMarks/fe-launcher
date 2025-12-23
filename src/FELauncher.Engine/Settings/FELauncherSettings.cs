namespace FELauncher.Engine.Settings
{
    public sealed class FELauncherSettings
    {
        public string? LogLevel { get; set; }
        public int? EndSessionGracePeriod { get; set; }
        public ProcessSettings Frontend { get; set; } = new();
        public List<ProcessSettings> PreProcesses { get; set; } = [];
    }
}