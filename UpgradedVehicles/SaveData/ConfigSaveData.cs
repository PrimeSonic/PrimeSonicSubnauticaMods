namespace UpgradedVehicles.SaveData
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using System.Text;
    using Common;
    using EasyMarkup;

    internal class ConfigSaveData : EmPropertyCollection
    {
        internal static readonly string[] SpeedSettingLabels = new[]
        {
            BonusSpeedStyles.Disabled.ToString(),
            BonusSpeedStyles.Slower.ToString(),
            BonusSpeedStyles.Normal.ToString(),
            BonusSpeedStyles.Faster.ToString()
        };

        internal const string ConfigKey = "UpgradedVehiclesOptions";
        internal const string SeamothBonusSpeedID = "SeamothBonusSpeed";
        internal const string ExosuitBonusSpeedID = "ExosuitBonusSpeed";
        internal const string EnableDebugLogsID = "EnableDebugLogging";

        private static readonly string ConfigDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        private static string ConfigFile => Path.Combine(ConfigDirectory, $"{ConfigKey}.txt");

        internal bool ValidDataRead = true;

        internal BonusSpeedStyles SeamothBonusSpeedSetting
        {
            get => EmSeamothBonus.HasValue ? EmSeamothBonus.Value : BonusSpeedStyles.Normal;
            set => EmSeamothBonus.Value = value;
        }

        internal BonusSpeedStyles ExosuitBonusSpeedSetting
        {
            get => EmExosuitBonus.HasValue ? EmExosuitBonus.Value : BonusSpeedStyles.Normal;
            set => EmExosuitBonus.Value = value;
        }

        internal bool DebugLogsEnabled
        {
            get => EmDebugLogs.Value;
            set => EmDebugLogs.Value = value;
        }

        private readonly EmProperty<BonusSpeedStyles> EmSeamothBonus;
        private readonly EmProperty<BonusSpeedStyles> EmExosuitBonus;
        private readonly EmProperty<bool> EmDebugLogs;

        private static ICollection<EmProperty> SaveDataDefinitions => new List<EmProperty>()
        {
            new EmProperty<BonusSpeedStyles>(SeamothBonusSpeedID, BonusSpeedStyles.Normal),
            new EmProperty<BonusSpeedStyles>(ExosuitBonusSpeedID, BonusSpeedStyles.Normal),
            new EmProperty<bool>(EnableDebugLogsID, false){ Optional = true }
        };

        public ConfigSaveData() : base(ConfigKey, SaveDataDefinitions)
        {
            EmSeamothBonus = (EmProperty<BonusSpeedStyles>)Properties[SeamothBonusSpeedID];
            EmExosuitBonus = (EmProperty<BonusSpeedStyles>)Properties[ExosuitBonusSpeedID];
            EmDebugLogs = (EmProperty<bool>)Properties[EnableDebugLogsID];

            OnValueExtractedEvent += IsReadDataValid;
        }

        public ConfigSaveData(BonusSpeedStyles seamothSpeed, BonusSpeedStyles exosuitSpeed) : this()
        {
            this.SeamothBonusSpeedSetting = seamothSpeed;
            this.ExosuitBonusSpeedSetting = exosuitSpeed;
        }

        private void IsReadDataValid()
        {
            ValidDataRead = true;
            switch (this.SeamothBonusSpeedSetting)
            {
                case BonusSpeedStyles.Disabled:
                case BonusSpeedStyles.Slower:
                case BonusSpeedStyles.Normal:
                case BonusSpeedStyles.Faster:
                    break;
                default:
                    ValidDataRead &= false;
                    break;
            }

            switch (this.ExosuitBonusSpeedSetting)
            {
                case BonusSpeedStyles.Disabled:
                case BonusSpeedStyles.Slower:
                case BonusSpeedStyles.Normal:
                case BonusSpeedStyles.Faster:
                    break;
                default:
                    ValidDataRead &= false;
                    break;
            }
        }

        internal override EmProperty Copy()
        {
            return new ConfigSaveData(this.SeamothBonusSpeedSetting, this.ExosuitBonusSpeedSetting);
        }

        internal void InitializeSaveFile()
        {
            try
            {
                Load();
            }
            catch (Exception ex)
            {
                QuickLogger.Info($"Error loading {ConfigKey}: " + ex.ToString());
                Save();
            }
        }

        public void Save()
        {
            File.WriteAllLines(ConfigFile, new[]
            {
                "# -------------------------------------------------------------------- #",
                "# This config file can be edited in-game through the Mods options menu #",
                "#             This config file was built using EasyMarkup              #",
                "# -------------------------------------------------------------------- #",
                "",
                PrettyPrint()
            },
            Encoding.UTF8);
        }

        private void Load()
        {
            if (!File.Exists(ConfigFile))
            {
                QuickLogger.Info($"Config file not found. Writing default file.");
                Save();
                return;
            }

            string text = File.ReadAllText(ConfigFile, Encoding.UTF8);

            bool readCorrectly = FromString(text);

            if (!readCorrectly || !ValidDataRead)
            {
                QuickLogger.Info($"Config file contained errors. Writing default file.");
                Save();
            }
        }
    }
}
