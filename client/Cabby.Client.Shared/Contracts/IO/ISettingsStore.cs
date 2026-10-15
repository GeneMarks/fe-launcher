using Cabby.Client.Shared.Contracts.Settings;

namespace Cabby.Client.Shared.Contracts.IO
{
    public interface ISettingsStore
    {
        Task<CabbySettings?> GetSettingsAsync();
        Task SaveSettingsAsync(CabbySettings settings);
    }
}
