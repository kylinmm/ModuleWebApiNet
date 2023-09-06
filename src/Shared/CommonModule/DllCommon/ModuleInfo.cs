using System.Reflection;
using System.Text.Json.Serialization;

namespace CommonModule.DllCommon
{
    public class ModuleInfo
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("isBundledWithHost")]
        public bool IsBundledWithHost { get; set; }

        [JsonPropertyName("lastVersion")]
        public string LastVersion { get; set; }

        [JsonPropertyName("version")]
        public string Version { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonIgnore]
        public Assembly Assembly { get; set; }
    }
}
