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
        private static readonly ConcurrentDictionary<string, Type> _typeCache = new();

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
                    var assemblyName = AssemblyName.GetAssemblyName(assemblyFile);
                    if (loadedAssemblyNames.Contains(assemblyName.FullName))
                    {
                        continue;
                    }

                    // Load assembly
                    Assembly.LoadFrom(assemblyFile);
                    Logger.Log.Info($"Loading assembly: {assemblyName.Name}");
                }
                catch (Exception ex)
                {
                    Logger.Log.Tool.Error(DebugInfo.Empty, $"Failed to load assembly: {assemblyFile}. Error: {ex.Message}");
                }
            }

            // Reload assemblies and clear type cache.
            _loadedAssemblies.Clear();
            _loadedAssemblies.AddRange(AppDomain.CurrentDomain.GetAssemblies());
            _typeCache.Clear();
        }

        public static Type GetTypeByName(string fullName)
        {
            if (_typeCache.TryGetValue(fullName, out var cachedType))
            {
                return cachedType;
            }

            var type = Type.GetType(fullName);
            if (type != null)
            {
                _typeCache[fullName] = type;
                return type;
            }

            Assembly matchedAssembly = null;
            foreach (var assembly in _loadedAssemblies)
            {
                type = assembly.GetType(fullName);
                if (type != null)
                {
                    matchedAssembly = assembly;
                    break;
                }
            }

            if (type != null)
            {
                _typeCache[fullName] = type;
                // Move the matched assembly to the front for next lookup
                if (matchedAssembly != null && _loadedAssemblies.Count > 0 && _loadedAssemblies[0] != matchedAssembly)
                {
                    _loadedAssemblies.Remove(matchedAssembly);
                    _loadedAssemblies.Insert(0, matchedAssembly);
                }
                return type;
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

            for (int i = 0; i < _loadedAssemblies.Count; i++)
            {
                var assembly = _loadedAssemblies[i];
                try
                {
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
                catch (ReflectionTypeLoadException ex)
                {
                    foreach (var t in ex.Types)
                    {
                        if (t == null || t == baseType) continue;
                        if (baseType.IsAssignableFrom(t))
                            result.Add(t);
                    }
                }
            }

            return result;
        }
    }
}
