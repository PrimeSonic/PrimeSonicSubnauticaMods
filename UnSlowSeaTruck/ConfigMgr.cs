namespace UnSlowSeaTruck
{
    using System;
    using System.IO;
    using System.Reflection;
    using Oculus.Newtonsoft.Json;

    internal class ConfigMgr
    {
        private static SeaTruckConfig _config;
        internal static SeaTruckConfig Config => _config ?? (_config = new SeaTruckConfig());

        internal void LoadConfig()
        {
            string modDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string configFilePath = Path.Combine(modDirectory, $"{nameof(SeaTruckConfig)}.json");

            if (File.Exists(configFilePath))
            {
                try
                {
                    string serialilzedConfig = File.ReadAllText(configFilePath);
                    SeaTruckConfig loadedConfig = JsonConvert.DeserializeObject<SeaTruckConfig>(serialilzedConfig);
                    Console.WriteLine("[UnSlowSeaTruck] Existing config file loaded.");
                    _config = loadedConfig;
                }
                catch
                {
                    Console.WriteLine("[UnSlowSeaTruck] Failed to load config. Fallback to default values.");
                    _config = WriteDefaultConfig(configFilePath);
                }
            }
            else // Write default file
            {
                _config = WriteDefaultConfig(configFilePath);
            }
        }

        private SeaTruckConfig WriteDefaultConfig(string configFilePath)
        {
            var defaultConfig = new SeaTruckConfig();
            string serialilzedConfig = JsonConvert.SerializeObject(defaultConfig, Formatting.Indented);
            File.WriteAllText(configFilePath, serialilzedConfig);
            return defaultConfig;
        }
    }
}
