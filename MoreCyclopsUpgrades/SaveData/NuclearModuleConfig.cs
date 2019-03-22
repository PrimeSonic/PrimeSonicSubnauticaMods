namespace MoreCyclopsUpgrades.SaveData
{
    using MoreCyclopsUpgrades.Managers;
    using SMLHelper.V2.Options;
    using System;
    using System.IO;
    using System.Text;
    using UnityEngine;

    internal class NuclearModuleConfig : ModOptions
    {
        private const string OldConfigFile = @"./QMods/MoreCyclopsUpgrades/Config.txt";
        private const string ConfigFile = "./QMods/MoreCyclopsUpgrades/" + EmNuclearConfig.ConfigKey + ".txt";

        private static float RequiredEnergyDeficit = 1140f;
        private const float MinPercent = 10f;
        private const float MaxPercent = 99f;
        private const float DefaultPercent = 95f;
        private const string ToggleID = "NukModConserve";
        private const string SliderID = "NukeModActivatesAt";

        private static float CyclopsMaxPower = 1;

        internal static float MinimumEnergyDeficit => EmConfig.ConserveNuclearModulePower ? RequiredEnergyDeficit : PowerManager.MinimalPowerValue;

        internal static EmNuclearConfig EmConfig = new EmNuclearConfig(MinPercent, MaxPercent, DefaultPercent);

        internal void Initialize()
        {
            try
            {
                LoadFromFile();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[MoreCyclopsUpgrades] Error loading {EmNuclearConfig.ConfigKey}: " + ex.ToString());
                WriteConfigFile();
            }
        }

        internal static void SetCyclopsMaxPower(float maxPower)
        {
            if (CyclopsMaxPower == maxPower)
                return;

            CyclopsMaxPower = maxPower;

            UpdateRequiredDeficit();
        }

        private static void UpdateRequiredDeficit()
        {
            RequiredEnergyDeficit = Mathf.Round(CyclopsMaxPower - CyclopsMaxPower * EmConfig.RequiredEnergyPercentage / 100f);
        }

        public NuclearModuleConfig() : base("Cyclops Nuclear Module Options")
        {
            base.ToggleChanged += ConservationEnabledChanged;
            base.SliderChanged += EnergyDeficitChanged;
        }

        /// <summary>
        /// <para>Builds up the configuration the options.</para>
        /// <para>This method should be composed of calls into the following methods:
        /// <seealso cref="M:SMLHelper.V2.Options.ModOptions.AddSliderOption(System.String,System.String,System.Single,System.Single,System.Single)" /> | <seealso cref="M:SMLHelper.V2.Options.ModOptions.AddToggleOption(System.String,System.String,System.Boolean)" /> | <seealso cref="M:SMLHelper.V2.Options.ModOptions.AddChoiceOption(System.String,System.String,System.String[],System.Int32)" />.</para>
        /// <para>Make sure you have subscribed to the events in the constructor to handle what happens when the value is changed:
        /// <seealso cref="E:SMLHelper.V2.Options.ModOptions.SliderChanged" /> | <seealso cref="E:SMLHelper.V2.Options.ModOptions.ToggleChanged" /> | <seealso cref="E:SMLHelper.V2.Options.ModOptions.ChoiceChanged" />.</para>
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        public override void BuildModOptions()
        {
            base.AddToggleOption(ToggleID, "Conserve Power", EmConfig.ConserveNuclearModulePower);
            base.AddSliderOption(SliderID, "Start charging below %", MinPercent, MaxPercent, EmConfig.RequiredEnergyPercentage);
        }

        private void ConservationEnabledChanged(object sender, ToggleChangedEventArgs args)
        {
            if (args.Id != ToggleID)
                return;

            EmConfig.ConserveNuclearModulePower = args.Value;
            WriteConfigFile();
        }

        private void EnergyDeficitChanged(object sender, SliderChangedEventArgs args)
        {
            if (args.Id != SliderID)
                return;

            EmConfig.RequiredEnergyPercentage = Mathf.Round(args.Value);
            UpdateRequiredDeficit();
            WriteConfigFile();
        }

        private void WriteConfigFile()
        {
            File.WriteAllLines(ConfigFile, new[]
            {
                "# -------------------------------------------------------------------- #",
                "# This config file can be edited in-game through the Mods options menu #",
                "#             This config file was built using EasyMarkup              #",
                "# -------------------------------------------------------------------- #",
                "",
                EmConfig.PrettyPrint(),
                "",
                "# Here's the full details on what these configurations do: #",
                "",
                $"# '{EmNuclearConfig.EmConserveDescription}' #",
                "# When this option is enabled, the Cyclops will only to use up the non-renewable nuclear power only after it's lost enough power cell charge. #",
                "# Set this to 'NO' if you want nuclear power to keep your Cyclops topped up. #",
                "# Set this to 'YES' if you want coast on power cell charge for a while. #",
                "# This way, you can conserve your nuclear battery for only when it's needed. #",
                "# Conserving your nuclear module's power will help it last longer, especially if you're usually moving between renewable sources of energy anyways. #",
                "",
                $"# '{EmNuclearConfig.EmDeficitDescription}' #",
                "# Set the value of this option to configure how low you're willing to let your Cyclops go down in power before charging from the nuclear battery. #",
                "# The minimum allowed value is '10' percent #",
                "# The maximum allowed value is '99' percent #",
                "# If you want your nuclear option only for absolute emergencies, set this to a lower value. #",
                "# If you want to keep your power cells topped up, set this to a higher value. #",
                $"# This option is ignored if '{EmNuclearConfig.EmConserveDescription}' is set to 'NO'. #",
            }, Encoding.UTF8);
        }

        private void LoadFromFile()
        {
            if (File.Exists(OldConfigFile))
            {
                Console.WriteLine($"[MoreCyclopsUpgrades] Found original nuclear module config file.");
                // Renamed the config file because we're going to add a new one
                File.Move(OldConfigFile, ConfigFile);
                Console.WriteLine($"[MoreCyclopsUpgrades] Nuclear module config file renamed.");
            }

            if (!File.Exists(ConfigFile))
            {
                Console.WriteLine($"[MoreCyclopsUpgrades] Nuclear module config file not found. Writing default file.");
                WriteConfigFile();
                return;
            }

            string text = File.ReadAllText(ConfigFile, Encoding.UTF8);

            bool readCorrectly = EmConfig.FromString(text);

            if (!readCorrectly || !EmConfig.ValidDataRead)
            {
                Console.WriteLine($"[MoreCyclopsUpgrades] Nuclear module config file contained errors. Writing default file.");
                WriteConfigFile();
                return;
            }
        }
    }
}
