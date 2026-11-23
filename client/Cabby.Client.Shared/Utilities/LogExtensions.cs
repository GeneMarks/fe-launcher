using System.Text.Json;
using System.Text.Json.Serialization;

namespace Cabby.Client.Shared.Utilities
{
    public static class LogExtensions
    {
        private static readonly JsonSerializerOptions jsonSerializerOptions = new()
        {
            WriteIndented = false,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        public static string ToJsonString<T>(this T obj)
            => JsonSerializer.Serialize(obj, jsonSerializerOptions);
    }
}