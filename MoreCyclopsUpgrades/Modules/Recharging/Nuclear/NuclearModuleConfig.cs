namespace MoreCyclopsUpgrades
{
    using System;
    using System.IO;
    using System.Text;
    using System.Threading;
    using Common.EasyMarkup;
    using SMLHelper.V2.Options;

    internal class NuclearModuleConfig : ModOptions
    {
        private const string ConfigFile = @"./QMods/MoreCyclopsUpgrades/NuclearModuleConfig.txt";

        private static bool ConserveNuclearModulePower = false;
        private static float RequiredEnergyDeficit = Default;
        private const float Min = 10f;
        private const float Max = 190f;
        private const float Default = 50f;
        private const string ToggleID = "NukModConserve";
        private const string SliderID = "NukeModActivatesOn";

        internal static float MinimumEnergyDeficit => ConserveNuclearModulePower ? RequiredEnergyDeficit : 0f;

        private readonly EmYesNo EmConserve = new EmYesNo("ConserveNuclearModulePower", ConserveNuclearModulePower);
        private readonly EmProperty<float> EmDeficit = new EmProperty<float>("RequiredEnergyDeficit", Default);

        internal void Initialize()
        {
            try
            {
                LoadFromFile();
            }
            catch (Exception ex)
            {
                Console.WriteLine("[MoreCyclopsUpgrades] Error loading NuclearModuleConfig: " + ex.ToString());
            }

        }

        public NuclearModuleConfig() : base("Nuclear Module Options")
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
            base.AddToggleOption(ToggleID, "Conserve Nuclear Module Power", ConserveNuclearModulePower);
            base.AddSliderOption(SliderID, "Activate Nuclear Module At X Power Deficit", Min, Max, RequiredEnergyDeficit);
        }

        private void ConservationEnabledChanged(object sender, ToggleChangedEventArgs args)
        {
            if (args.Id != ToggleID)
                return;

            ConserveNuclearModulePower = args.Value;
            UpdateFileInBackground();
        }

        private void EnergyDeficitChanged(object sender, SliderChangedEventArgs args)
        {
            if (args.Id != SliderID)
                return;

            RequiredEnergyDeficit = args.Value;
            UpdateFileInBackground();
        }

        private void UpdateFileInBackground()
        {
            Thread bgWriter = new Thread(WriteConfigFile);
            bgWriter.Start();
        }

        private void WriteConfigFile()
        {
            File.WriteAllLines(ConfigFile, new[]
            {
                EmConserve.ToString(),
                EmDeficit.ToString(),
                "",
                "# --------------------------- #",
                "# How to use this config file",
                "",
                "# When 'Conserve Nuclear Module Power' is enabled, the Cyclops will only to use up the non-renewable nuclear power only after it's lost enough power cell charge. #",
                "# Set this to 'NO' if you want nuclear power to keep your Cyclops topped up. #",
                "# Set this to 'YES' if you want coast on power cell charge for a while. #",
                "# This way, you can conserve your nuclear battery for only when it's needed. #",
                "# Conserving your nuclear module's power will help it last longer, especially if you're usually moving between renewable sources of energy anyways. #",
                "# --------------------------- #",
                "# Set the value of 'Required Energy Deficit' to configure how low you're willing to let your Cyclops go down in power before charging from the nuclear battery. #",
                "# The minimum allowed value is '10' #",
                "# The maximum allowed value is '190' #",
                "# For example: If you set this to 50, nuclear charging will only begin after your Cyclops is below 1150/1200 energy. #",
                "",
            }, Encoding.Unicode);
        }

        private void LoadFromFile()
        {
            if (!File.Exists(ConfigFile))
            {
                WriteConfigFile();
                return;
            }

            string[] lines = File.ReadAllLines(ConfigFile, Encoding.Unicode);

            bool readCorrectly =
                EmConserve.FromString(lines[0]) &&
                EmDeficit.FromString(lines[1]);

            if (!readCorrectly)
            {
                WriteConfigFile();
                return;
            }

            ConserveNuclearModulePower = EmConserve.Value;

            if (EmDeficit.Value > Max || EmDeficit.Value < Min)
                RequiredEnergyDeficit = Default;
            else
                RequiredEnergyDeficit = EmDeficit.Value;
        }

    }
}
