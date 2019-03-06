namespace CustomCraft2SML.Serialization
{
    using Common.EasyMarkup;
    using System.Collections.Generic;

    internal class CustomCraft2Config : EmPropertyCollection
    {
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
    }
}
