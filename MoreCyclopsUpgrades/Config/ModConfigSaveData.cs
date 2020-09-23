namespace MoreCyclopsUpgrades.Config
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using Common;
    using EasyMarkup;
    using MoreCyclopsUpgrades.Config.Options;

    internal class ModConfigSaveData : EmPropertyCollection
    {
        private const string ConfigKey = "ModConfig";

        private readonly string versionLine = $"# MoreCyclopsUpgrades v{QuickLogger.GetAssemblyVersion()} #";

        private static List<EmProperty> CreateDefinitions(List<ConfigOption> options)
        {
            var properties = new List<EmProperty>(options.Count);

            foreach (ConfigOption item in options)
            {
                switch (item.OptionType)
                {
                    case OptionTypes.Slider when item is SliderOption slider:
                        properties.Add(new EmProperty<float>(slider.Id, slider.Value) { Optional = true });
                        break;
                    case OptionTypes.Choice when item is ChoiceOption choice:
                        properties.Add(new EmProperty<int>(choice.Id, choice.Index) { Optional = true });
                        break;
                    case OptionTypes.Toggle when item is ToggleOption toggle:
                        properties.Add(new EmProperty<bool>(toggle.Id, toggle.State) { Optional = true });
                        break;
                }
            }

            return properties;
        }

        private readonly List<ConfigOption> configOptions;
        private bool hasValidData = true;

        public ModConfigSaveData(List<ConfigOption> options) : this(CreateDefinitions(options))
        {
            configOptions = options;
        }

        private ModConfigSaveData(ICollection<EmProperty> definitions) : base(ConfigKey, definitions)
        {
            OnValueExtractedEvent += () =>
            {
                foreach (ConfigOption item in configOptions)
                {
                    switch (item.OptionType)
                    {
                        case OptionTypes.Slider when item is SliderOption slider:
                            hasValidData &= ((EmProperty<float>)this[slider.Id]).HasDataInRange(slider.MinValue, slider.MaxValue);
                            break;
                        case OptionTypes.Choice when item is ChoiceOption choice:
                            hasValidData &= ((EmProperty<int>)this[choice.Id]).HasData();
                            break;
                        case OptionTypes.Toggle when item is ToggleOption toggle:
                            hasValidData &= ((EmProperty<bool>)this[toggle.Id]).HasData();
                            break;
                        default:
                            break;
                    }
                }
            };
        }

        public EmProperty<float> GetFloatProperty(SliderOption option)
        {
            return (EmProperty<float>)this[option.Id];
        }

        public EmProperty<int> GetIntProperty(ChoiceOption option)
        {
            return (EmProperty<int>)this[option.Id];
        }

        public EmProperty<bool> GetBoolProperty(ToggleOption option)
        {
            return (EmProperty<bool>)this[option.Id];
        }

        internal override EmProperty Copy()
        {
            return new ModConfigSaveData(configOptions);
        }

        internal void SaveToFile()
        {
            string extraText = $"{versionLine}{Environment.NewLine}" +
                               $"# This config file can be edited in-game through the Mods options menu #{Environment.NewLine}";
            this.Save(extraText);
        }

        internal void LoadFromFile()
        {
            string fileLocation = this.DefaultFileLocation();
            if (!File.Exists(fileLocation))
            {
                QuickLogger.Warning("Mod config file not found. Writing default file.");
                SaveToFile();
                return;
            }

            string text = File.ReadAllText(fileLocation, Encoding.UTF8);

            if (!text.StartsWith(versionLine))
            {
                QuickLogger.Warning("Mod config file was out of date. Writing default file.");
                SaveToFile();
                return;
            }

            bool readCorrectly = base.FromString(text);

            if (!readCorrectly || !hasValidData)
            {
                QuickLogger.Warning("Mod config file contained error. Writing default file.");
                SaveToFile();
                return;
            }
        }
    }
}
