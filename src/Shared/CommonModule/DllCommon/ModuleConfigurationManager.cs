using System.Text.Json;

namespace CommonModule.DllCommon
{
    public class ModuleConfigurationManager : IModuleConfigurationManager
    {
        private static readonly string ModulesFilename = "modules.json";

        public IEnumerable<ModuleInfo> GetModules()
        {
            string path = Path.Combine(GlobalConfiguration.ContentRootPath, ModulesFilename);
            using StreamReader streamReader = new StreamReader(path);
            string json = streamReader.ReadToEnd();
            return JsonSerializer.Deserialize<List<ModuleInfo>>(json);
        }
    }
}
