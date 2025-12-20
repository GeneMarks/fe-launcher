namespace FELauncher.Host
{
    internal static class HostConstants
    {
        public const string MutexName      = @"Global\925bac46-79de-4db8-8b09-0ba589ce99ee";
        public const long LogFileSizeLimit = (long)1e+8; // 100mb
        public const int LogFileCountLimit = 10;
    }
}
