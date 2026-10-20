namespace Cabby.Client.Shared.Contracts.Configuration
{
    public interface ISettingsStore
    {
        Task<CabbySettings?> GetSettingsAsync();
        Task SaveSettingsAsync(CabbySettings settings);
    }
}
