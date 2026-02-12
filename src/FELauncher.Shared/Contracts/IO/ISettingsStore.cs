using FELauncher.Shared.Contracts.Settings;

namespace FELauncher.Shared.Contracts.IO
{
    public interface ISettingsStore
    {
        Task<FELauncherSettings?> GetSettingsAsync();
        Task SaveSettingsAsync(FELauncherSettings settings);
    }
}
