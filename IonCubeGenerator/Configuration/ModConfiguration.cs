namespace IonCubeGenerator.Configuration
{
    using System;
    using System.IO;
    using System.Reflection;
    using Common;
#if SUBNAUTICA
    using Oculus.Newtonsoft.Json;
#elif BELOWZERO
    using Newtonsoft.Json;
#endif
    using SMLHelper.V2.Handlers;

    internal class ModConfiguration
    {
        private static bool _isInitialized;

        /// <summary>
        /// A singleton of the class <see cref="ModConfiguration"/>
        /// </summary>
        internal static readonly ModConfiguration Singleton = new ModConfiguration();
        
        /// <summary>
        /// The configuration file path.
        /// </summary>
        [JsonIgnore] 
        internal string ConfigurationFilePath { get; set; }

        /// <summary>
        /// Allows the ION Cube generator to play audio in game.
        /// </summary>
        [JsonProperty]
        internal bool AllowSFX { get; set; } = true;

        private void LoadConfigurationFromFile()
        {
            try
            {

                if (File.Exists(ConfigurationFilePath))
                {
                    // == Load Configuration == //
                    string configJson = File.ReadAllText(ConfigurationFilePath);

                    JsonSerializerSettings settings = new JsonSerializerSettings { MissingMemberHandling = MissingMemberHandling.Ignore };

                    // == LoadData == //
                    var json = JsonConvert.DeserializeObject<ModConfiguration>(configJson, settings);

                    Singleton.AllowSFX = json.AllowSFX;
                }
                else
                {
                    CreateModConfiguration(ConfigurationFilePath);
                }
            }
            catch (Exception e)
            {
                QuickLogger.Error(e.Message);
            }
        }

        private void CreateModConfiguration(string configurationFilePath)
        {
            var saveDataJson = JsonConvert.SerializeObject(new ModConfiguration(), Formatting.Indented);

            File.WriteAllText(configurationFilePath, saveDataJson);
        }

        private static string ModPath { get; } = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        /// <summary>
        /// Saves the mod configuration file in the mod directory
        /// </summary>
        internal void SaveModConfiguration()
        {
           QuickLogger.Info("Saving Mod Configuration.");
           try
           {
               var saveDataJson = JsonConvert.SerializeObject(this, Formatting.Indented);
               
               File.WriteAllText(ConfigurationFilePath, saveDataJson);
           }
           catch (Exception e)
           {
               QuickLogger.Error($"{e.Message}\n{e.StackTrace}");
           }
        }

        /// <summary>
        /// Creates an in-game menu option for the ION Cube generator and loads the configuration file located in the
        /// mod directory. If one doesn't exist the default settings will be used and a new configuration
        /// file will be generated.
        /// </summary>
        internal static void Initialize()
        {
            if (!_isInitialized)
            {
                OptionsPanelHandler.RegisterModOptions(new MenuConfiguration());
                Singleton.ConfigurationFilePath =  Path.Combine(ModPath, "config.json");
                Singleton.LoadConfigurationFromFile();
                _isInitialized = true;
            }

        }
    }
}
