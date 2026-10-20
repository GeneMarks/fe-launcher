using Cabby.Client.Engine.Configuration;
using Cabby.Client.Shared.Contracts.Settings;

namespace Cabby.Client.Tests.Engine
{
    public sealed class JsonSettingsStoreTests : IDisposable
    {
        private readonly string _tempDirectory = Path.Combine(
            Path.GetTempPath(),
            "CabbyClient.Tests",
            Guid.NewGuid().ToString("N"));

        public JsonSettingsStoreTests()
        {
            Directory.CreateDirectory(_tempDirectory);
        }

        private string GetTempFilePath()
            => Path.Combine(_tempDirectory, $"{Guid.NewGuid():N}.json");

        [Fact]
        public async Task SaveSettingsAsync_CreatesFile()
        {
            var path = GetTempFilePath();
            var store = new JsonSettingsStore(path);
            var settings = new CabbySettings();

            await store.SaveSettingsAsync(settings);

            Assert.True(File.Exists(path));
            Assert.NotEmpty(await File.ReadAllTextAsync(path));
        }

        [Fact]
        public async Task GetSettingsAsync_ReturnsDeserializedSettings()
        {
            var path = GetTempFilePath();
            var store = new JsonSettingsStore(path);
            var settings = new CabbySettings();
            await store.SaveSettingsAsync(settings);

            var result = await store.GetSettingsAsync();

            Assert.NotNull(result);
        }

        [Fact]
        public void SaveSettingsStatic_CreatesFile()
        {
            var path = GetTempFilePath();
            var settings = new CabbySettings();

            JsonSettingsStore.SaveSettings(settings, path);

            Assert.True(File.Exists(path));
            Assert.NotEmpty(File.ReadAllText(path));
        }

        [Fact]
        public async Task SaveSettingsAsync_OverwritesExistingFile()
        {
            var testJson = "{ \"old\": true }";
            var path = GetTempFilePath();
            var store = new JsonSettingsStore(path);
            var settings = new CabbySettings();
            await File.WriteAllTextAsync(path, testJson);

            await store.SaveSettingsAsync(settings);
            var result = await File.ReadAllTextAsync(path);

            Assert.DoesNotContain(testJson, result);
        }

        public void Dispose()
        {
            if (Directory.Exists(_tempDirectory))
            {
                Directory.Delete(_tempDirectory, true);
            }
        }
    }
}
