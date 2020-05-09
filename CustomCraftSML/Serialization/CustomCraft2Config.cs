namespace CustomCraft2SML.Serialization
{
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using Common;
    using EasyMarkup;

    internal class CustomCraft2Config : EmPropertyCollection
    {
        private static readonly CustomCraft2Config Config = new CustomCraft2Config();

        internal const string CC2Key = "CustomCraft2Configs";
        internal const string FileName = "CustomCraft2Config.txt";
        private const string DebugLogsKey = "DebugLogsEnabled";

        private readonly EmYesNo debugLogging;

        private static ICollection<EmProperty> ConfigProperties => new List<EmProperty>(1)
        {
            new EmYesNo(DebugLogsKey, false),
        };

        public CustomCraft2Config() : this(CC2Key, ConfigProperties)
        {

        }

        public CustomCraft2Config(string key, ICollection<EmProperty> definitions) : base(key, definitions)
        {
            debugLogging = (EmYesNo)Properties[DebugLogsKey];
        }

        internal bool EnabledDebugLogs => debugLogging.HasValue && debugLogging.Value;

        internal override EmProperty Copy()
        {
            return new CustomCraft2Config(this.Key, this.CopyDefinitions);
        }

        internal static void CheckLogLevel()
        {
            if (!File.Exists(FileLocations.ConfigFile))
            {
                File.WriteAllText(FileLocations.ConfigFile, Config.PrettyPrint());
                QuickLogger.DebugLogsEnabled = false;
                QuickLogger.Info("CustomCraft2Config file was not found. Default file written.");
            }
            else
            {
                string configText = File.ReadAllText(FileLocations.ConfigFile, Encoding.UTF8);

                if (Config.FromString(configText))
                {
                    QuickLogger.DebugLogsEnabled = Config.EnabledDebugLogs;
                }
            }

            if (QuickLogger.DebugLogsEnabled)
                QuickLogger.Debug("Debug logging is enable");
            else
                QuickLogger.Info("To enable Debug logging, change the \"DebugLogsEnabled\" attribute in the mod.json file to true");
        }
    }
}
