using CommonModule.DllCommon;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.Loader;

namespace Host.ServiceExtensions
{
    public static class DllLoadServiceExt
    {
        private static readonly IModuleConfigurationManager _modulesConfig = new ModuleConfigurationManager();

        public static IServiceCollection AddModules(this IServiceCollection services)
        {
            foreach (ModuleInfo module in _modulesConfig.GetModules())
            {
                if (!module.IsBundledWithHost)
                {
                    TryLoadModuleAssembly(module.Id, module);
                    if ((object)module.Assembly == null)
                    {
                        throw new Exception("Cannot find main assembly for module " + module.Id);
                    }
                }
                else
                {
                    module.Assembly = Assembly.Load(new AssemblyName(module.Id));
                }

                GlobalConfiguration.Modules.Add(module);
            }

            return services;
        }

        public static IServiceCollection AddInitModules(this IServiceCollection services)
        {
            foreach (ModuleInfo module in GlobalConfiguration.Modules)
            {
                Type type = module.Assembly.GetTypes().FirstOrDefault((Type t) => typeof(IModuleInitializer).IsAssignableFrom(t));
                if (type != null && type != typeof(IModuleInitializer))
                {
                    IModuleInitializer moduleInitializer = (IModuleInitializer)Activator.CreateInstance(type);
                    services.AddSingleton(typeof(IModuleInitializer), moduleInitializer);
                    moduleInitializer.ConfigureServices(services);
                }
            }

            return services;
        }

        private static void TryLoadModuleAssembly(string moduleFolderPath, ModuleInfo module)
        {
            string path = Path.Combine(moduleFolderPath, "bin");
            DirectoryInfo directoryInfo = new DirectoryInfo(path);
            if (!Directory.Exists(path))
            {
                return;
            }

            FileSystemInfo[] fileSystemInfos = directoryInfo.GetFileSystemInfos("*.dll", SearchOption.AllDirectories);
            foreach (FileSystemInfo fileSystemInfo in fileSystemInfos)
            {
                Assembly assembly;
                try
                {
                    assembly = AssemblyLoadContext.Default.LoadFromAssemblyPath(fileSystemInfo.FullName);
                }
                catch (FileLoadException)
                {
                    assembly = Assembly.Load(new AssemblyName(Path.GetFileNameWithoutExtension(fileSystemInfo.Name)));
                    if ((object)assembly == null)
                    {
                        throw;
                    }

                    string fileVersion = FileVersionInfo.GetVersionInfo(assembly.Location).FileVersion;
                    string fileVersion2 = FileVersionInfo.GetVersionInfo(fileSystemInfo.FullName).FileVersion;
                    if (fileVersion2 != fileVersion)
                    {
                        throw new Exception("Cannot load " + fileSystemInfo.FullName + " " + fileVersion2 + " because " + assembly.Location + " " + fileVersion + " has been loaded");
                    }
                }

                if (Path.GetFileNameWithoutExtension(assembly.ManifestModule.Name) == module.Id)
                {
                    module.Assembly = assembly;
                }
            }
        }

        public static IServiceCollection AddCustomizedMvc(this IServiceCollection services)
        {
            IMvcBuilder mvcBuilder = services.AddMvc();
            foreach (ModuleInfo item in GlobalConfiguration.Modules.Where((ModuleInfo x) => !x.IsBundledWithHost))
            {
                AddApplicationPart(mvcBuilder, item.Assembly);
            }

            return services;
        }

        private static void AddApplicationPart(IMvcBuilder mvcBuilder, Assembly assembly)
        {
            ApplicationPartFactory applicationPartFactory = ApplicationPartFactory.GetApplicationPartFactory(assembly);
            foreach (ApplicationPart applicationPart in applicationPartFactory.GetApplicationParts(assembly))
            {
                mvcBuilder.PartManager.ApplicationParts.Add(applicationPart);
            }

            IReadOnlyList<Assembly> relatedAssemblies = RelatedAssemblyAttribute.GetRelatedAssemblies(assembly, throwOnError: false);
            foreach (Assembly item in relatedAssemblies)
            {
                applicationPartFactory = ApplicationPartFactory.GetApplicationPartFactory(item);
                foreach (ApplicationPart applicationPart2 in applicationPartFactory.GetApplicationParts(item))
                {
                    mvcBuilder.PartManager.ApplicationParts.Add(applicationPart2);
                }
            }
        }
    }
}
