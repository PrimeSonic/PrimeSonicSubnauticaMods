namespace UnSlowSeaTruck
{
    using System;
    using System.IO;
    using System.Reflection;
    using Oculus.Newtonsoft.Json;

    internal class ConfigMgr
    {
        internal static TConfig LoadConfig<TConfig>(string fileName = null) where TConfig : class, new()
        {
            var assembly = Assembly.GetCallingAssembly();

            string modDirectory = Path.GetDirectoryName(assembly.Location);
            string configFilePath = Path.Combine(modDirectory, fileName ?? $"{typeof(TConfig).Name}.json");

            if (File.Exists(configFilePath))
            {
                try
                {                    
                    string serialilzedConfig = File.ReadAllText(configFilePath);

                    if (string.IsNullOrEmpty(serialilzedConfig))
                    {
                        Console.WriteLine($"[{assembly.GetName().Name}] Config file was empty. Overwriting with default values.");
                        return WriteDefaultConfig<TConfig>(configFilePath);
                    }

                    TConfig loadedConfig = JsonConvert.DeserializeObject<TConfig>(serialilzedConfig);
                    Console.WriteLine($"[{assembly.GetName().Name}] Existing config file loaded.");
                    return loadedConfig;
                }
                catch
                {
                    Console.WriteLine($"[{assembly.GetName().Name}] Failed to read config file. Overwriting with default values.");
                    return WriteDefaultConfig<TConfig>(configFilePath);
                }
            }
            else // Write default file
            {
                Console.WriteLine($"[{assembly.GetName().Name}] No config file found. Writing default config file.");
                return WriteDefaultConfig<TConfig>(configFilePath);
            }
        }

        private static TConfig WriteDefaultConfig<TConfig>(string configFilePath) where TConfig : class, new()
        {
            var defaultConfig = new TConfig();
            string serialilzedConfig = JsonConvert.SerializeObject(defaultConfig, Formatting.Indented);
            File.WriteAllText(configFilePath, serialilzedConfig);
            return defaultConfig;
        }
    }
}
