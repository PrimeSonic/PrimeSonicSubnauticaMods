namespace MoreCyclopsUpgrades.Config
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using Common;
    using Common.EasyMarkup;

    internal class ModConfigSaveData : EmPropertyCollection
    {
        #region Constants
        private const string ConfigKey = "ModConfig";
        private const string AuxConsoleEnabledKey = "AuxConsoleEnabled";
        private const string ChallengeModeKey = "ChallengeMode";
        private const string DeficitThresholdKey = "DeficitThreshold";
        private readonly string versionLine = $"# VERSION {QuickLogger.GetAssemblyVersion()} #";
        #endregion

        private static List<EmProperty> CreateDefinitions()
        {
            return new List<EmProperty>()
            {
                new EmYesNo(AuxConsoleEnabledKey, defaultValue: true),
                new EmProperty<ChallengeLevel>(ChallengeModeKey, ModConfig.DefaultChallenge),
                new EmProperty<float>(DeficitThresholdKey, ModConfig.DefaultThreshold)
            };
        }

        public ModConfigSaveData() : this(CreateDefinitions())
        {
        }

        private ModConfigSaveData(ICollection<EmProperty> definitions) : base(ConfigKey, definitions)
        {
            this.AuxConsoleEnabled = (EmYesNo)Properties[AuxConsoleEnabledKey];
            this.ChallengeMode = (EmProperty<ChallengeLevel>)Properties[ChallengeModeKey];
            this.DeficitThreshold = (EmProperty<float>)Properties[DeficitThresholdKey];

            OnValueExtractedEvent += () =>
            {
                hasValidData = this.AuxConsoleEnabled.HasData() &
                               this.ChallengeMode.HasData() &
                               this.DeficitThreshold.HasDataInRange(ModConfig.MinThreshold, ModConfig.MaxThreshold);
            };
        }

        private bool hasValidData = true;

        internal EmYesNo AuxConsoleEnabled { get; }
        internal EmProperty<ChallengeLevel> ChallengeMode { get; }
        internal EmProperty<float> DeficitThreshold { get; }

        internal override EmProperty Copy()
        {
            return new ModConfigSaveData(this.CopyDefinitions);
        }

        internal void SaveToFile()
        {
            string extraText = $"# This config file can be edited in-game through the Mods options menu #{Environment.NewLine}";
            this.Save(extraText);
        }

        internal void LoadFromFile()
        {
            string fileLocation = this.DefaultFileLocation();
            if (!File.Exists(fileLocation))
            {
                QuickLogger.Debug("Mod config file not found. Writing default file.");
                SaveToFile();
                return;
            }

            string text = File.ReadAllText(fileLocation, Encoding.UTF8);

            if (!text.StartsWith(versionLine))
            {
                QuickLogger.Debug("Mod config file was out of date. Writing default file.");
                SaveToFile();
                return;
            }

            bool readCorrectly = base.FromString(text);

            if (!readCorrectly || !hasValidData)
            {
                QuickLogger.Debug("Mod config file contained error. Writing default file.");
                SaveToFile();
                return;
            }
        }
    }
}
