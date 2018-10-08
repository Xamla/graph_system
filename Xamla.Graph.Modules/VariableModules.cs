using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamla.Graph.MethodModule;

namespace Xamla.Graph.Modules
{
    public enum GraphValueStoreType
    {
        ScriptContext,
        PersistentStore
    };

    public static class VariableModules
    {
        public class VariableDisplayName
            : IDynamicDisplayName
        {
            public string Format(IModule module)
            {
                if (module.Properties.IsConnected(module.GetInputPin("name")))
                {
                    string variableName = module.Properties.Get<string>("name");
                    if (!string.IsNullOrEmpty(variableName))
                        return variableName;
                }
                return null;
            }
        }

        static IGraphRuntime runtime;

        internal static void Init(IGraphRuntime runtime)
        {
            VariableModules.runtime = runtime;
        }

        [StaticModule(ModuleType = "Xamla.Graph.GetStore")]
        public static IValueStore GetStore([InputPin(PropertyMode = PropertyMode.Default)] GraphValueStoreType store)
        {
            if (store == GraphValueStoreType.ScriptContext)
                return runtime.ScriptContext;
            if (store == GraphValueStoreType.PersistentStore)
                return runtime.Root.ValueStore;

            throw new Exception("Unsupported store type specified.");
        }

        [StaticModule(ModuleType = "Xamla.Graph.GetVariable", Flow = true, DynamicDisplayName = typeof(VariableDisplayName))]
        public static object GetVariable(
            [InputPin(PropertyMode = PropertyMode.Default)] string name = "var1",
            [InputPin(PropertyMode = PropertyMode.Default)] GraphValueStoreType store = GraphValueStoreType.PersistentStore,
            [InputPin(PropertyMode = PropertyMode.Default)] object defaultValue = null
        )
        {
            IValueStore valueStore = GetStore(store); ;
            if (!valueStore.TryGetValue(name, out var result))
                return defaultValue;
            return result;
        }

        [StaticModule(ModuleType = "Xamla.Graph.GetVariableBoolean", Flow = true, DynamicDisplayName = typeof(VariableDisplayName))]
        public static bool GetVariableBoolean(
            [InputPin(PropertyMode = PropertyMode.Default)] string name = "var1",
            [InputPin(PropertyMode = PropertyMode.Default)] GraphValueStoreType store = GraphValueStoreType.PersistentStore,
            [InputPin(PropertyMode = PropertyMode.Default)] bool? defaultValue = null
        )
        {
            IValueStore valueStore = GetStore(store);
            return defaultValue.HasValue ? valueStore.GetBoolean(name, defaultValue.Value) : valueStore.GetBoolean(name);
        }

        [StaticModule(ModuleType = "Xamla.Graph.GetVariableInt32", Flow = true, DynamicDisplayName = typeof(VariableDisplayName))]
        public static int GetVariableInt32(
            [InputPin(PropertyMode = PropertyMode.Default)] string name = "var1",
            [InputPin(PropertyMode = PropertyMode.Default)] GraphValueStoreType store = GraphValueStoreType.PersistentStore,
            [InputPin(PropertyMode = PropertyMode.Default)] int? defaultValue = null
        )
        {
            IValueStore valueStore = GetStore(store);
            return defaultValue.HasValue ? valueStore.GetInt32(name, defaultValue.Value) : valueStore.GetInt32(name);
        }

        [StaticModule(ModuleType = "Xamla.Graph.GetVariableFloat64", Flow = true, DynamicDisplayName = typeof(VariableDisplayName))]
        public static double GetVariableFloat64(
            [InputPin(PropertyMode = PropertyMode.Default)] string name = "var1",
            [InputPin(PropertyMode = PropertyMode.Default)] GraphValueStoreType store = GraphValueStoreType.PersistentStore,
            [InputPin(PropertyMode = PropertyMode.Default)] double? defaultValue = null
        )
        {
            IValueStore valueStore = GetStore(store);
            return defaultValue.HasValue ? valueStore.GetFloat64(name, defaultValue.Value) : valueStore.GetFloat64(name);
        }

        [StaticModule(ModuleType = "Xamla.Graph.GetVariableString", Flow = true, DynamicDisplayName = typeof(VariableDisplayName))]
        public static string GetVariableString(
            [InputPin(PropertyMode = PropertyMode.Default)] string name = "var1",
            [InputPin(PropertyMode = PropertyMode.Default)] GraphValueStoreType store = GraphValueStoreType.PersistentStore,
            [InputPin(PropertyMode = PropertyMode.Default)] string defaultValue = null
        )
        {
            IValueStore valueStore = GetStore(store);
            if (defaultValue != null)
                return valueStore.GetString(name, defaultValue);
            else
                return valueStore.GetString(name);
        }

        [StaticModule(ModuleType = "Xamla.Graph.SetVariable", Flow = true, DynamicDisplayName = typeof(VariableDisplayName))]
        public static void SetVariable(
            [InputPin(PropertyMode = PropertyMode.Allow)] object value,
            [InputPin(PropertyMode = PropertyMode.Default)] string name = "var1",
            [InputPin(PropertyMode = PropertyMode.Default)] GraphValueStoreType store = GraphValueStoreType.PersistentStore
        )
        {
            IValueStore valueStore = GetStore(store);
            valueStore[name] = value;
        }

        [StaticModule(ModuleType = "Xamla.Graph.SetVariableBool", Flow = true, DynamicDisplayName = typeof(VariableDisplayName))]
        public static void SetVariableBoolean(
            [InputPin(PropertyMode = PropertyMode.Allow)] bool value,
            [InputPin(PropertyMode = PropertyMode.Default)] string name = "var1",
            [InputPin(PropertyMode = PropertyMode.Default)] GraphValueStoreType store = GraphValueStoreType.PersistentStore
        )
        {
            IValueStore valueStore = GetStore(store);
            valueStore.SetBoolean(name, value);
        }

        [StaticModule(ModuleType = "Xamla.Graph.SetVariableInt32", Flow = true, DynamicDisplayName = typeof(VariableDisplayName))]
        public static void SetVariableInt32(
            [InputPin(PropertyMode = PropertyMode.Allow)] int value,
            [InputPin(PropertyMode = PropertyMode.Default)] string name = "var1",
            [InputPin(PropertyMode = PropertyMode.Default)] GraphValueStoreType store = GraphValueStoreType.PersistentStore
        )
        {
            IValueStore valueStore = GetStore(store);
            valueStore.SetInt32(name, value);
        }

        [StaticModule(ModuleType = "Xamla.Graph.SetVariableFloat64", Flow = true, DynamicDisplayName = typeof(VariableDisplayName))]
        public static void SetVariableFloat64(
            [InputPin(PropertyMode = PropertyMode.Allow)] double value,
            [InputPin(PropertyMode = PropertyMode.Default)] string name = "var1",
            [InputPin(PropertyMode = PropertyMode.Default)] GraphValueStoreType store = GraphValueStoreType.PersistentStore
        )
        {
            IValueStore valueStore = GetStore(store);
            valueStore.SetFloat64(name, value);
        }

        [StaticModule(ModuleType = "Xamla.Graph.SetVariableString", Flow = true, DynamicDisplayName = typeof(VariableDisplayName))]
        public static void SetVariableString(
                [InputPin(PropertyMode = PropertyMode.Allow)] string value,
                [InputPin(PropertyMode = PropertyMode.Default)] string name = "var1",
                [InputPin(PropertyMode = PropertyMode.Default)] GraphValueStoreType store = GraphValueStoreType.PersistentStore
            )
        {
            IValueStore valueStore = GetStore(store);
            valueStore.SetString(name, value);
        }

        [StaticModule(ModuleType = "Xamla.Graph.RemoveVariable", Flow = true)]
        public static void RemoveVariable(
            [InputPin(PropertyMode = PropertyMode.Default)] string name,
            [InputPin(PropertyMode = PropertyMode.Default)] GraphValueStoreType store
        )
        {
            if (store == GraphValueStoreType.PersistentStore)
            {
                runtime.Graph.ValueStore.Remove(name);
            }
            else
            {
                runtime.ScriptContext.Remove(name);
            }
        }

        [StaticModule(ModuleType = "Xamla.Graph.ClearStore", Flow = true)]
        public static void ClearStore(
            [InputPin(PropertyMode = PropertyMode.Default)] GraphValueStoreType store
        )
        {
            if (store == GraphValueStoreType.PersistentStore)
            {
                runtime.Graph.ValueStore.Clear();
            }
            else
            {
                runtime.ScriptContext.Clear();
            }
        }

        [StaticModule(ModuleType = "Xamla.Graph.SaveStore", Flow = true)]
        public static void SaveStore()
        {
            runtime.Graph.ValueStore.Save();
        }
    }
}
