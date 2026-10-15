namespace Cabby.Client.Shared.Contracts.Settings
{
    public sealed class CabbySettings
    {
        public string? LogLevel { get; set; }
        public bool StartWithWindows { get; set; }
        public bool AutoLaunchSession { get; set; }
        public bool DisableNotifications { get; set; }
        public int EndSessionGracePeriod { get; set; }
        public ProcessSettings Frontend { get; set; } = new();
        public List<ProcessSettings> PreProcesses { get; set; } = [];
    }
}