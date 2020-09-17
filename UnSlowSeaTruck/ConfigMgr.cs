namespace UnSlowSeaTruck
{
    using System;
    using System.IO;
    using System.Reflection;
    using Newtonsoft.Json;

    internal class ConfigMgr
    {
        private static readonly string ModName = Assembly.GetExecutingAssembly().GetName().Name;
        private static readonly string ModDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        internal static TConfig LoadConfig<TConfig>() where TConfig : class, new()
        {
            string configFilePath = Path.Combine(ModDirectory, $"{typeof(TConfig).Name}.json");

            if (File.Exists(configFilePath))
            {
                try
                {                    
                    string serialilzedConfig = File.ReadAllText(configFilePath);

                    if (string.IsNullOrEmpty(serialilzedConfig))
                    {
                        Console.WriteLine($"[{ModName}] Config file was empty. Overwriting with default values.");
                        return WriteDefaultConfig<TConfig>(configFilePath);
                    }

                    TConfig loadedConfig = JsonConvert.DeserializeObject<TConfig>(serialilzedConfig);
                    Console.WriteLine($"[{ModName}] Existing config file loaded.");
                    return loadedConfig;
                }
                catch
                {
                    Console.WriteLine($"[{ModName}] Failed to read config file. Overwriting with default values.");
                    return WriteDefaultConfig<TConfig>(configFilePath);
                }
            }
            else // Write default file
            {
                Console.WriteLine($"[{ModName}] No config file found. Writing default config file.");
                return WriteDefaultConfig<TConfig>(configFilePath);
            }
        }

        internal static void SaveConfig<TConfig>(TConfig config) where TConfig : class
        {
            string configFilePath = Path.Combine(ModDirectory, $"{typeof(TConfig).Name}.json");
            string serialilzedConfig = JsonConvert.SerializeObject(config, Formatting.Indented);
            File.WriteAllText(configFilePath, serialilzedConfig);
        }

        private static TConfig WriteDefaultConfig<TConfig>(string configFilePath) where TConfig : class, new()
        {
            var defaultConfig = new TConfig();
            SaveConfig(defaultConfig);
            return defaultConfig;
        }
    }
}
