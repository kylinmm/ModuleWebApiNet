namespace CommonModule.DllCommon
{
    public static class GlobalConfiguration
    {
        public static IList<ModuleInfo> Modules { get; set; } = new List<ModuleInfo>();


        public static string WebRootPath { get; set; }

        public static string ContentRootPath { get; set; }

        public static IList<string> AngularModules { get; } = new List<string>();


        public static void RegisterAngularModule(string angularModuleName)
        {
            AngularModules.Add(angularModuleName);
        }
    }
}
