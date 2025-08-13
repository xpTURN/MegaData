using System;
using System.IO;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace xpTURN.Common
{
    public static class AssemblyUtils
    {
        private static List<Assembly> _loadedAssemblies = new List<Assembly>();
        private static readonly ConcurrentDictionary<string, Type> TypeCache = new();

        public static void LoadAllDependencies()
        {
            // Check currently loaded assemblies
            var loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies();
            HashSet<string> loadedAssemblyNames = new HashSet<string>(loadedAssemblies.Select(a => a.FullName));

            // Search for assembly files
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            var assemblyFiles = Directory.GetFiles(baseDirectory, "*.dll", SearchOption.AllDirectories);

            foreach (var assemblyFile in assemblyFiles)
            {
                try
                {
                    var assamblyName = AssemblyName.GetAssemblyName(assemblyFile);
                    if (loadedAssemblyNames.Contains(assamblyName.FullName))
                    {
                        continue;
                    }

                    // Load assembly
                    Assembly.LoadFrom(assemblyFile);
                    Logger.Log.Info($"Loading assembly: {assamblyName.Name}");
                }
                catch (Exception ex)
                {
                    Logger.Log.Tool.Error(DebugInfo.Empty, $"Failed to load assembly: {assemblyFile}. Error: {ex.Message}");
                }
            }

            //
            _loadedAssemblies.Clear();
            _loadedAssemblies.AddRange(AppDomain.CurrentDomain.GetAssemblies());
        }

        public static Type GetTypeByName(string fullName)
        {
            if (TypeCache.TryGetValue(fullName, out var cachedType))
            {
                return cachedType;
            }

            var type = Type.GetType(fullName);
            if (type != null)
            {
                TypeCache[fullName] = type;
                return type;
            }

            for (int i = 0; i < _loadedAssemblies.Count; i++)
            {
                var assembly = _loadedAssemblies[i];
                type = assembly.GetType(fullName);
                if (type != null)
                {
                    TypeCache[fullName] = type;

                    // Move the matched assembly to the front of the list
                    if (i != 0)
                    {
                        _loadedAssemblies.RemoveAt(i);
                        _loadedAssemblies.Insert(0, assembly);
                    }

                    return type;
                }
            }

            return null;
        }

        /// <summary>
        /// Finds all types in loaded assemblies that inherit from the specified type (fullName).
        /// </summary>
        public static List<Type> GetTypesByBaseName(string fullName)
        {
            var result = new List<Type>();
            Type baseType = GetTypeByName(fullName);
            if (baseType == null)
            {
                Logger.Log.Tool.Error(DebugInfo.Empty, $"Type '{fullName}' not found.");
                return result;
            }

            try
            {
                for (int i = 0; i < _loadedAssemblies.Count; i++)
                {
                    var assembly = _loadedAssemblies[i];
                    foreach (var type in assembly.GetTypes())
                    {
                        if (type == baseType)
                            continue;
                        if (baseType.IsAssignableFrom(type))
                        {
                            result.Add(type);
                        }
                    }
                }
            }
            catch (ReflectionTypeLoadException ex)
            {
                foreach (var t in ex.Types)
                {
                    if (t == null || t == baseType) continue;
                    if (baseType.IsAssignableFrom(t))
                        result.Add(t);
                }
            }

            return result;
        }
    }
}
