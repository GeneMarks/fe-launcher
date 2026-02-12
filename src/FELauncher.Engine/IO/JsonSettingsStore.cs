using FELauncher.Shared;
using FELauncher.Shared.Contracts.IO;
using FELauncher.Shared.Contracts.Settings;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FELauncher.Engine.IO
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

        public async Task<FELauncherSettings?> GetSettingsAsync()
        {
            using var stream = new FileStream(_settingsFile!, FileMode.Open, FileAccess.Read);
            var settings = await JsonSerializer.DeserializeAsync<FELauncherSettings>(stream);

            return settings;
        }

        public async Task SaveSettingsAsync(FELauncherSettings settings)
        {
            var tmp = Path.GetTempFileName();
            await File.WriteAllTextAsync(tmp, Serialize(settings));
            File.Move(tmp, _settingsFile!, overwrite: true);
        }

        public static void SaveSettings(FELauncherSettings settings, string path)
        {
            var tmp = Path.GetTempFileName();
            File.WriteAllText(tmp, Serialize(settings));
            File.Move(tmp, path, overwrite: true);
        }

        private static string Serialize(FELauncherSettings settings)
            => JsonSerializer.Serialize(settings, jsonOptions);
    }
}
