using Cabby.Client.Shared;
using Cabby.Client.Shared.Contracts.IO;
using Cabby.Client.Shared.Contracts.Settings;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Cabby.Client.Engine.IO
{
    public sealed class JsonSettingsStore : ISettingsStore
    {
        private readonly string _settingsFile;
        private static readonly JsonSerializerOptions jsonOptions = new()
        {
            WriteIndented = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        public JsonSettingsStore(string? settingsFile = null)
        {
            if (string.IsNullOrEmpty(settingsFile))
            {
                _settingsFile = AppPaths.SettingsFile;
            }
            else
            {
                _settingsFile = settingsFile;
            }
        }

        public async Task<CabbySettings?> GetSettingsAsync()
        {
            using var stream = new FileStream(_settingsFile!, FileMode.Open, FileAccess.Read);
            var settings = await JsonSerializer.DeserializeAsync<CabbySettings>(stream);

            return settings;
        }

        public async Task SaveSettingsAsync(CabbySettings settings)
        {
            var tmp = Path.GetTempFileName();
            await File.WriteAllTextAsync(tmp, Serialize(settings));
            File.Move(tmp, _settingsFile!, overwrite: true);
        }

        public static void SaveSettings(CabbySettings settings, string path)
        {
            var tmp = Path.GetTempFileName();
            File.WriteAllText(tmp, Serialize(settings));
            File.Move(tmp, path, overwrite: true);
        }

        private static string Serialize(CabbySettings settings)
            => JsonSerializer.Serialize(settings, jsonOptions);
    }
}
