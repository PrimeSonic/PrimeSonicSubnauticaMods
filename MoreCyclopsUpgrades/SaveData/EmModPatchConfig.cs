namespace MoreCyclopsUpgrades.SaveData
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using Common;
    using Common.EasyMarkup;

    internal class EmModPatchConfig : EmPropertyCollection
    {
        internal bool ValidDataRead = true;

        private const string ConfigKey = "MoreCyclopsUpgradesConfig";
        private const string ConfigFile = "./QMods/MoreCyclopsUpgrades/" + ConfigKey + ".txt";

        private const string EmAuxEnabledKey = "EnableAuxiliaryUpgradeConsoles";
        private const string EmUpgradesEnabledKey = "EnableNewUpgradeModules";
        private const string EmBioEnergyEnabledKey = "EnableCyclopsBioReactor";

        internal bool EnableAuxiliaryUpgradeConsoles
        {
            get => EmAuxEnabled.Value;
            set => EmAuxEnabled.Value = value;
        }

        internal bool EnableNewUpgradeModules
        {
            get => EmUpgradesEnabled.Value;
            set => EmUpgradesEnabled.Value = value;
        }

        internal bool EnableBioEnergy
        {
            get => EmBioEnergyEnabled.Value;
            set => EmBioEnergyEnabled.Value = value;
        }

        private readonly EmYesNo EmAuxEnabled;
        private readonly EmYesNo EmUpgradesEnabled;
        private readonly EmYesNo EmBioEnergyEnabled;

        private static ICollection<EmProperty> definitions = new List<EmProperty>(2)
        {
            new EmYesNo(EmAuxEnabledKey, true),
            new EmYesNo(EmUpgradesEnabledKey, true),
            new EmYesNo(EmBioEnergyEnabledKey, true),
        };

        public EmModPatchConfig() : base(ConfigKey, definitions)
        {
            EmAuxEnabled = (EmYesNo)Properties[EmAuxEnabledKey];
            EmUpgradesEnabled = (EmYesNo)Properties[EmUpgradesEnabledKey];
            EmBioEnergyEnabled = (EmYesNo)Properties[EmBioEnergyEnabledKey];

            OnValueExtractedEvent += Validate;
        }

        internal void Initialize()
        {
            try
            {
                LoadFromFile();
            }
            catch (Exception ex)
            {
                QuickLogger.Warning("Error loading {ConfigKey}: " + ex.ToString());
                WriteConfigFile();
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

        internal override EmProperty Copy() => new EmModPatchConfig();

        private void WriteConfigFile()
        {
            File.WriteAllLines(ConfigFile, new[]
            {
                "# ----------------------------------------------------------------------------- #",
                "# Changes to this config file will only take effect next time you boot the game #",
                "#                 This config file was built using EasyMarkup                   #",
                "# ----------------------------------------------------------------------------- #",
                "",
                PrettyPrint(),
                "",
                "# Here's the full details on what these configurations do: #",
                "",
                $"# 'Enable Auxiliary Upgrade Consoles' #",
                "# When this option is enabled, the new Auxiliary Cyclops Upgrade Console will be patched into the game. #",
                "# This is an upgrade console you can build inside your cyclops to give yourself an extra six upgrade slots. #",
                "# Set this to 'NO' if you want the added challenge of having to manage with only six upgrade slots. #",
                "# Set this to 'YES' if you want to be able to equip every cyclops upgrade module you ever come across. #",
                "",
                $"# 'Enable New Upgrade Modules' #",
                "# When this option is enabled, all seven upgrade modules created for this mod will be patched into the game. #",
                "# Set this to 'NO' if you want a closer to vanilla experience. #",
                "# Set this to 'YES' if you want to unleash the full potential of your cyclops. #",
                "",
                $"# 'Enable Cyclops BioReactor' #",
                "# When this option is enabled, the Cyclops Bioreactor will be patched into the game. #",
                "# Set this to 'NO' if you want a closer to vanilla experience. #",
                "# Set this to 'YES' if you want additional power options for your Cyclops. #",
                "",
                "# ----------------------------------------------------------------------------- #",
                "# Because of how the Auxiliary Upgrade Console interacts with the new upgrade modules, #",
                "# it was not possible to separate these two options into two distinct mods. #",
                "# With that in mind, these options were enabled to accomodate players who wanted just one of these options. #",
                "# ----------------------------------------------------------------------------- #",
            }, Encoding.UTF8);
        }

        private void LoadFromFile()
        {
            if (!File.Exists(ConfigFile))
            {
                QuickLogger.Message("Mod config file not found. Writing default file.");
                WriteConfigFile();
                return;
            }

            string text = File.ReadAllText(ConfigFile, Encoding.UTF8);

            bool readCorrectly = base.FromString(text);

            if (!readCorrectly || !ValidDataRead)
            {
                QuickLogger.Warning("Mod config file contained error. Writing default file.");
                WriteConfigFile();
                return;
            }
        }
    }
}
