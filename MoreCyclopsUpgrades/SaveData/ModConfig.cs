namespace MoreCyclopsUpgrades.SaveData
{
    using Common;
    using Common.EasyMarkup;
    using SMLHelper.V2.Handlers;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using System.Text;

    internal class ModConfig : EmPropertyCollection
    {
        private static readonly ModConfigOptions Options = new ModConfigOptions();

        internal static ModConfig Settings;

        private readonly string SaveFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), $"{ConfigKey}.txt");

        private readonly string versionLine = $"# VERSION {QuickLogger.GetAssemblyVersion()} #";

        private bool ValidDataRead = true;

        private const string ConfigKey = "MoreCyclopsUpgradesConfig";
        private const string EmAuxEnabledKey = "EnableAuxiliaryUpgradeConsoles";
        private const string EmUpgradesEnabledKey = "EnableNewUpgradeModules";
        private const string EmBioEnergyEnabledKey = "EnableCyclopsBioReactor";
        private const string EmPowerLevelKey = "CyclopsPowerLevel";

        internal bool EnableAuxiliaryUpgradeConsoles => EmAuxEnabled.Value;

        internal bool EnableNewUpgradeModules => EmUpgradesEnabled.Value;

        internal bool EnableBioReactors => EmBioEnergyEnabled.Value;

        internal CyclopsPowerLevels PowerLevel
        {
            get => EmPowerLevel.Value;
            set => EmPowerLevel.Value = value;
        }

        internal int MaxChargingModules()
        {
            switch (EmPowerLevel.Value)
            {
                case CyclopsPowerLevels.Leviathan:
                    return 12;
                case CyclopsPowerLevels.Ampeel:
                    return 6;
                case CyclopsPowerLevels.Crabsnake:
                    return 3;
                case CyclopsPowerLevels.Peeper:
                    return 1;
                default:
                    throw new ArgumentOutOfRangeException("Invalid difficulty selected");
            }
        }

        internal int MaxSpeedModules()
        {
            switch (EmPowerLevel.Value)
            {
                case CyclopsPowerLevels.Leviathan:
                    return 6;
                case CyclopsPowerLevels.Ampeel:
                    return 4;
                case CyclopsPowerLevels.Crabsnake:
                    return 2;
                case CyclopsPowerLevels.Peeper:
                    return 1;
                default:
                    throw new ArgumentOutOfRangeException("Invalid difficulty selected");
            }
        }

        internal int RechargeSkipRate()
        {
            switch (EmPowerLevel.Value)
            {
                case CyclopsPowerLevels.Leviathan:
                    return 0;
                case CyclopsPowerLevels.Ampeel:
                    return 1;
                case CyclopsPowerLevels.Crabsnake:
                    return 2;
                case CyclopsPowerLevels.Peeper:
                    return 3;
                default:
                    throw new ArgumentOutOfRangeException("Invalid difficulty selected");
            }
        }

        internal float MaxCyclopsHealth()
        {
            switch (EmPowerLevel.Value)
            {
                case CyclopsPowerLevels.Leviathan:
                    return 1600f;
                case CyclopsPowerLevels.Ampeel:
                    return 1400f;
                case CyclopsPowerLevels.Crabsnake:
                    return 1200f;
                case CyclopsPowerLevels.Peeper:
                    return 1000f;
                default:
                    throw new ArgumentOutOfRangeException("Invalid difficulty selected");
            }
        }

        internal float RechargePenalty()
        {
            switch (EmPowerLevel.Value)
            {
                case CyclopsPowerLevels.Leviathan:
                    return 1.0f;
                case CyclopsPowerLevels.Ampeel:
                    return 0.98f;
                case CyclopsPowerLevels.Crabsnake:
                    return 0.96f;
                case CyclopsPowerLevels.Peeper:
                    return 0.94f;
                default:
                    throw new ArgumentOutOfRangeException("Invalid difficulty selected");
            }
        }

        private readonly EmYesNo EmAuxEnabled;
        private readonly EmYesNo EmUpgradesEnabled;
        private readonly EmYesNo EmBioEnergyEnabled;
        private readonly EmProperty<CyclopsPowerLevels> EmPowerLevel;

        private static ICollection<EmProperty> definitions = new List<EmProperty>()
        {
            new EmYesNo(EmAuxEnabledKey, true),
            new EmYesNo(EmUpgradesEnabledKey, true),
            new EmYesNo(EmBioEnergyEnabledKey, true),
            new EmProperty<CyclopsPowerLevels>(EmPowerLevelKey, CyclopsPowerLevels.Ampeel),
        };

        public ModConfig() : base(ConfigKey, definitions)
        {
            EmAuxEnabled = (EmYesNo)Properties[EmAuxEnabledKey];
            EmUpgradesEnabled = (EmYesNo)Properties[EmUpgradesEnabledKey];
            EmBioEnergyEnabled = (EmYesNo)Properties[EmBioEnergyEnabledKey];
            EmPowerLevel = (EmProperty<CyclopsPowerLevels>)Properties[EmPowerLevelKey];

            OnValueExtractedEvent += Validate;
        }

        internal static void Initialize()
        {
            try
            {
                OptionsPanelHandler.RegisterModOptions(Options);
                Settings = new ModConfig();
                Settings.LoadFromFile();
            }
            catch (Exception ex)
            {
                QuickLogger.Warning("Error loading {ConfigKey}: " + ex.ToString());
                Settings.WriteConfigFile();
            }
        }

        private void Validate()
        {
            if (!EmAuxEnabled.HasValue)
            {
                QuickLogger.Warning($"Config value for {ConfigKey}>{EmAuxEnabled.Key} was out of range. Replaced with default.");
                ValidDataRead &= false;
            }

            if (!EmUpgradesEnabled.HasValue)
            {
                QuickLogger.Warning($"Config value for {ConfigKey}>{EmUpgradesEnabled.Key} was out of range. Replaced with default.");
                ValidDataRead &= false;
            }

            if (!EmBioEnergyEnabled.HasValue)
            {
                QuickLogger.Warning($"Config value for {ConfigKey}>{EmBioEnergyEnabled.Key} was out of range. Replaced with default.");
                ValidDataRead &= false;
            }
        }

        internal override EmProperty Copy()
        {
            return new ModConfig();
        }

        private void WriteConfigFile()
        {

            File.WriteAllLines(SaveFile, new[]
            {
                versionLine,
                PrettyPrint(),
                "",
                "# Here's the full details on what these configurations do: #",
                "",
                "# 'Enable Auxiliary Upgrade Consoles' #",
                "# When this option is enabled, the new Auxiliary Cyclops Upgrade Console will be patched into the game. #",
                "# This is an upgrade console you can build inside your cyclops to give yourself an extra six upgrade slots. #",
                "# Set this to 'NO' if you want the added challenge of having to manage with only six upgrade slots. #",
                "# Set this to 'YES' if you want to be able to equip every cyclops upgrade module you ever come across. #",
                "",
                "# 'Enable New Upgrade Modules' #",
                "# When this option is enabled, all seven upgrade modules created for this mod will be patched into the game. #",
                "# Set this to 'NO' if you want a closer to vanilla experience. #",
                "# Set this to 'YES' if you want to unleash the full potential of your cyclops. #",
                "",
                "# 'Enable Cyclops Bioreactor' #",
                "# When this option is enabled, the Cyclops Bioreactor will be patched into the game. #",
                "# Set this to 'NO' if you want a closer to vanilla experience. #",
                "# Set this to 'YES' if you want additional power options for your Cyclops. #",
                "",
                "# 'Cyclops Power Level' #",
                "# This setting lets you configure the overall balance of the entire mod. #",
                "# If you find that the Cyclops is too easy to too hard to maintain, try changing this setting. #",
                "# Changing this will limit the number of charging modules that can be added to the Cyclops of any one type. #",
               $"# {CyclopsPowerLevels.Leviathan} (Easy Mode) #",
               $"# {CyclopsPowerLevels.Ampeel} (Modest Difficulty) #",
               $"# {CyclopsPowerLevels.Crabsnake} (Moderate Difficulty) #",
               $"# {CyclopsPowerLevels.Peeper} (Hard Mode) #",
                "",

            }, Encoding.UTF8);
        }

        private void LoadFromFile()
        {
            if (!File.Exists(SaveFile))
            {
                QuickLogger.Debug("Mod config file not found. Writing default file.");
                WriteConfigFile();
                return;
            }

            string text = File.ReadAllText(SaveFile, Encoding.UTF8);

            if (!text.StartsWith(versionLine))
            {
                QuickLogger.Debug("Mod config file was out of date. Writing default file.");
                WriteConfigFile();
                return;
            }

            bool readCorrectly = base.FromString(text);

            if (!readCorrectly || !ValidDataRead)
            {
                QuickLogger.Debug("Mod config file contained error. Writing default file.");
                WriteConfigFile();
                return;
            }
        }
    }
}
