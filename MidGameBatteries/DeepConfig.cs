namespace MidGameBatteries
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using System.Text;
    using Common;
    using Common.EasyMarkup;

    internal enum PowerStyle
    {
        VeryLow,
        Low,
        Normal,
        High,
        VeryHigh,
    }

    internal class DeepConfig : EmPropertyCollection
    {
        private const string MainKey = "MidGameBatConfig";
        private const string PowerStyleKey = "BatteryPower";
        private const float BasePower = 100f;

        private string executingDirectory;
        private string ConfigFile;

        private readonly EmProperty<PowerStyle> PowerStyleConfig;

        private static ICollection<EmProperty> DeepConfigDefinitions => new List<EmProperty>
        {
            new EmProperty<PowerStyle>(PowerStyleKey, PowerStyle.Normal),
        };

        public DeepConfig() : base(MainKey, DeepConfigDefinitions)
        {
            PowerStyleConfig = (EmProperty<PowerStyle>)Properties[PowerStyleKey];
        }

        public PowerStyle SelectedPowerStyle
        {
            get => PowerStyleConfig.Value;
            set => PowerStyleConfig.Value = value;
        }

        public float BatteryCapacity
        {
            get
            {
                switch (this.SelectedPowerStyle)
                {
                    case PowerStyle.VeryLow:
                        return BasePower * 1.5f;
                    case PowerStyle.Low:
                        return BasePower * 2f;
                    case PowerStyle.Normal:
                        return BasePower * 2.5f;
                    case PowerStyle.High:
                        return BasePower * 3f;
                    case PowerStyle.VeryHigh:
                        return BasePower * 3.75f;
                    default:
                        return BasePower * 1.75f;
                }
            }
        }

        internal override EmProperty Copy()
        {
            return new DeepConfig() { SelectedPowerStyle = this.SelectedPowerStyle };
        }

        internal void ReadConfigFile(string mainDirectory)
        {
            executingDirectory = mainDirectory;
            ConfigFile = Path.Combine(executingDirectory, $"{MainKey}.txt");

            try
            {
                Load();
            }
            catch (Exception ex)
            {
                QuickLogger.Message($"Error loading {MainKey}: " + ex.ToString());
                Save();
            }
        }

        public void Save()
        {
            File.WriteAllLines(this.ConfigFile, new[]
            {
                "# --------------------------------------------------------------------------- #",
                "# Updates to this config file will ONLY take effect AFTER restarting the game #",
                "#                 This config file was built using EasyMarkup                 #",
                "# --------------------------------------------------------------------------- #",
                "",
                "# You can configure BatteryPower to customize how much extra power you'll get out of the Deep Battery and Deep Power Cell. #",
                "# Choose one of these options:                                                                                             #",
                "",
                "# VeryLow  : 1.5X more power than the standard batteries. Suitable for players who want a some upgrade but still want to worry about managing energy. #",
                "# Low      :   2X more power than the standard batteries. For players who really want to feel the jump without feeling like they got too much out.    #",
                "# Normal   : 2.5X more power than the standard batteries. The default recommended setting. Exactly half-way between the standard and ion batteries.   #",
                "# High     :   3X more power than the standard batteries. For players who feel the ion batteries come too late in the game to be useful.              #",
                // VeryHigh : 3.75X - EasterEgg for players who don't want to have to think about energy
                "",
                PrettyPrint(),
                "",
                "# NOTE: Changing this setting will only affect new Deep Batteries and Deep Power Cells you craft. #",
                "# Any existing batteries already in your game will not be changed.                                #",
            },
            Encoding.UTF8);
        }

        private void Load()
        {
            if (!File.Exists(this.ConfigFile))
            {
                QuickLogger.Message($"Config file not found. Writing default file.");
                Save();
                return;
            }

            string text = File.ReadAllText(this.ConfigFile, Encoding.UTF8);

            bool readCorrectly = FromString(text);

            if (!readCorrectly)
            {
                QuickLogger.Message($"Config file contained errors. Writing default file.");
                Save();
            }
        }
    }
}
