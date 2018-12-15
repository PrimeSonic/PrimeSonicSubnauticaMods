namespace UpgradedVehicles.SaveData
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using Common;
    using Common.EasyMarkup;

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

        private const string ConfigDirectory = "./QMods/UpgradedVehicles";
        private const string ConfigFile = ConfigDirectory + "/" + ConfigKey + ".txt";

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

        internal int SeamothBonusSpeedIndex
        {
            get => (int)this.SeamothBonusSpeedSetting;
            set => this.SeamothBonusSpeedSetting = (BonusSpeedStyles)value;
        }

        internal int ExosuitBonusSpeedIndex
        {
            get => (int)this.ExosuitBonusSpeedSetting;
            set => this.ExosuitBonusSpeedSetting = (BonusSpeedStyles)value;
        }

        private readonly EmProperty<BonusSpeedStyles> EmSeamothBonus;
        private readonly EmProperty<BonusSpeedStyles> EmExosuitBonus;

        private static ICollection<EmProperty> SaveDataDefinitions => new List<EmProperty>()
        {
            new EmProperty<BonusSpeedStyles>(SeamothBonusSpeedID, BonusSpeedStyles.Normal),
            new EmProperty<BonusSpeedStyles>(ExosuitBonusSpeedID, BonusSpeedStyles.Normal)
        };

        public ConfigSaveData() : base(ConfigKey, SaveDataDefinitions)
        {
            EmSeamothBonus = (EmProperty<BonusSpeedStyles>)Properties[SeamothBonusSpeedID];
            EmExosuitBonus = (EmProperty<BonusSpeedStyles>)Properties[ExosuitBonusSpeedID];

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

        internal override EmProperty Copy() => new ConfigSaveData(this.SeamothBonusSpeedSetting, this.ExosuitBonusSpeedSetting);

        internal void InitializeSaveFile()
        {
            try
            {
                Load();
            }
            catch (Exception ex)
            {
                QuickLogger.Message($"Error loading {ConfigKey}: " + ex.ToString());
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
                QuickLogger.Message($"Config file not found. Writing default file.");
                Save();
                return;
            }

            string text = File.ReadAllText(ConfigFile, Encoding.UTF8);

            bool readCorrectly = FromString(text);

            if (!readCorrectly || !ValidDataRead)
            {
                QuickLogger.Message($"Config file contained errors. Writing default file.");
                Save();
            }
        }
    }
}
