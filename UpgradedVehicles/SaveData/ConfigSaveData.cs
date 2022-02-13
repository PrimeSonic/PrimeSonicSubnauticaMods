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
        internal const string SeaTruckBonusSpeedID = "SeaTruckBonusSpeed";
        internal const string ExosuitBonusSpeedID = "ExosuitBonusSpeed";
        internal const string EnableDebugLogsID = "EnableDebugLogging";

        private static readonly string ConfigDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        private static string ConfigFile => Path.Combine(ConfigDirectory, $"{ConfigKey}.txt");

        internal bool ValidDataRead = true;

#if SUBNAUTICA
        internal BonusSpeedStyles SeamothBonusSpeedSetting
        {
            get => EmSeamothBonus.HasValue ? EmSeamothBonus.Value : BonusSpeedStyles.Normal;
            set => EmSeamothBonus.Value = value;
        }

#elif BELOWZERO
        internal BonusSpeedStyles SeaTruckBonusSpeedSetting
        {
            get => EmSeaTruckBonus.HasValue ? EmSeaTruckBonus.Value : BonusSpeedStyles.Normal;
            set => EmSeaTruckBonus.Value = value;
        }

#endif
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

#if SUBNAUTICA
        private readonly EmProperty<BonusSpeedStyles> EmSeamothBonus;
#elif BELOWZERO
        private readonly EmProperty<BonusSpeedStyles> EmSeaTruckBonus;
#endif
        private readonly EmProperty<BonusSpeedStyles> EmExosuitBonus;
        private readonly EmProperty<bool> EmDebugLogs;

        private static ICollection<EmProperty> SaveDataDefinitions => new List<EmProperty>()
        {
#if SUBNAUTICA
            new EmProperty<BonusSpeedStyles>(SeamothBonusSpeedID, BonusSpeedStyles.Normal),
#elif BELOWZERO
            new EmProperty<BonusSpeedStyles>(SeaTruckBonusSpeedID, BonusSpeedStyles.Normal),
#endif
            new EmProperty<BonusSpeedStyles>(ExosuitBonusSpeedID, BonusSpeedStyles.Normal),
            new EmProperty<bool>(EnableDebugLogsID, false){ Optional = true }
        };

        public ConfigSaveData() : base(ConfigKey, SaveDataDefinitions)
        {
#if SUBNAUTICA
            EmSeamothBonus = (EmProperty<BonusSpeedStyles>)Properties[SeamothBonusSpeedID];
#elif BELOWZERO
            EmSeaTruckBonus = (EmProperty<BonusSpeedStyles>)Properties[SeaTruckBonusSpeedID];
#endif
            EmExosuitBonus = (EmProperty<BonusSpeedStyles>)Properties[ExosuitBonusSpeedID];
            EmDebugLogs = (EmProperty<bool>)Properties[EnableDebugLogsID];

            OnValueExtractedEvent += IsReadDataValid;
        }

#if SUBNAUTICA
        public ConfigSaveData(BonusSpeedStyles seamothSpeed, BonusSpeedStyles exosuitSpeed) : this()
        {
            this.SeamothBonusSpeedSetting = seamothSpeed;
#elif BELOWZERO
        public ConfigSaveData(BonusSpeedStyles seaTruckSpeed, BonusSpeedStyles exosuitSpeed) : this()
        {
            this.SeaTruckBonusSpeedSetting = seaTruckSpeed;
#endif
            this.ExosuitBonusSpeedSetting = exosuitSpeed;
        }

        private void IsReadDataValid()
        {
            ValidDataRead = true;
#if SUBNAUTICA
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

#elif BELOWZERO
            switch (this.SeaTruckBonusSpeedSetting)
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

#endif

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
            return new ConfigSaveData(
#if SUBNAUTICA
            this.SeamothBonusSpeedSetting,
#elif BELOWZERO
            this.SeaTruckBonusSpeedSetting,
#endif
            this.ExosuitBonusSpeedSetting);
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
