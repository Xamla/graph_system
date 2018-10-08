using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xamla.Graph.MethodModule;
using Xamla.Types;

namespace Xamla.Graph
{
    public interface IServiceLocator
        : IServiceProvider
    {
        object GetInstance(Type type);
    }

    public interface IModuleFactory
    {
        void Register(string moduleType, ModuleDescription description, Func<IGraphRuntime, IServiceLocator, IModule> factory, Type type);
        void RegisterAlias(string aliasType, string moduleType, bool includeInCatalog = false);
        IModule Create(string moduleType);
        IModule Load(string moduleType);

        /// <summary>
        /// Complete list of available modules.
        /// </summary>
        /// <returns>An isolation copy of the current module list.</returns>
        IEnumerable<ModuleCatalogItem> GetCatalog();

        string GetPinDescription(IPin pin);
        ModuleDescription GetModuleDescription(string moduleType);
        void AddModuleDescription(ModuleDescription description);

        /// <summary>
        /// Get a namespace description object. A description of a parent namespace may be returned if no exact match is found.
        /// </summary>
        /// <param name="namespaceName">The name of the namespace for which to search the description.</param>
        /// <returns></returns>
        NamespaceDescription GetNamespaceDescription(string namespaceName);

        /// <summary>
        /// Register a namespace description.
        /// </summary>
        /// <param name="description">Description of the namespace</param>
        void AddNamespaceDescription(NamespaceDescription description);
    }

    public static class ModuleFactoryExtensions
    {
        public static void RegisterWithType(this IModuleFactory factory, Type type)
        {
            RegisterWithType(factory, type, null);
        }

        public static void RegisterWithType(this IModuleFactory factory, Type type, ModuleDescription description)
        {
            factory.Register(type.FullName, description, (runtime, s) => (IModule)s.GetInstance(type), type);
        }

        public static ModuleDescription GetModuleDescription(this IModuleFactory factory, IModule module)
        {
            return factory.GetModuleDescription(module.ModuleType);
        }

        static ModuleDescription FillDescriptionFromAttribute(Type type, ModuleAttribute attribute)
        {
            return new ModuleDescription
            {
                ModuleType = attribute.ModuleType ?? type.FullName,
                HelpText = attribute.Description,
                ReferenceUrl = attribute.ReferenceUrl,
                IconPath = attribute.IconPath
            };
        }

        public static void RegisterAllModules(this IModuleFactory moduleFactory, Assembly assembly)
        {
            var foundTypes = assembly
                .GetTypes()
                .Where(t => t.GetCustomAttribute<ModuleAttribute>() != null)
                .ToList();

            foreach (var type in foundTypes)
            {
                ModuleAttribute attribute = type.GetCustomAttribute<ModuleAttribute>();
                var description = FillDescriptionFromAttribute(type, attribute);
                moduleFactory.Register(description.ModuleType, description, (runtime, s) => (IModule)s.GetInstance(type), type);
            }

            var typeAliases = assembly
                .GetTypes()
                .Where(t => t.GetCustomAttribute<ModuleTypeAliasAttribute>() != null)
                .ToList();

            foreach (var type in typeAliases)
            {
                ModuleAttribute moduleAttribute = type.GetCustomAttribute<ModuleAttribute>();
                ModuleTypeAliasAttribute aliasAttribute = type.GetCustomAttribute<ModuleTypeAliasAttribute>();
                if (moduleAttribute == null || aliasAttribute == null)
                    continue;

                moduleFactory.RegisterAlias(aliasAttribute.AliasType, moduleAttribute.ModuleType, aliasAttribute.IncludeInCatalog);
            }
        }
    }
}
