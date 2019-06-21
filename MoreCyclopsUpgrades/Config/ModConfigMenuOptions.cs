namespace MoreCyclopsUpgrades.Config
{
    using SMLHelper.V2.Handlers;
    using SMLHelper.V2.Options;
    using UnityEngine;

    internal class ModConfigMenuOptions : ModOptions
    {
        private const string AuxConsoleId = "MCUConsole";
        private const string ChallengeModeId = "MCUChallenge";
        private const string DeficitThresholdId = "MCUThreshold";
        private const string ShowChargerIconseId = "MCUChargerIcons";
        private const string DebugLogsId = "MCUDebugLogs";

        public delegate void AuxConsoleToggledEvent(bool value);
        public delegate void ChallengeModeChangedEvent(ChallengeLevel value);
        public delegate void DeficitThresholdChangedEvent(float value);
        public delegate void ShowChargerIconsChangedEvent(ShowChargerIcons value);
        public delegate void DebugLogsToggledEvent(bool value);

        internal AuxConsoleToggledEvent AuxConsoleToggled;
        internal ChallengeModeChangedEvent ChallengeModeChanged;
        internal DeficitThresholdChangedEvent DeficitThresholdChanged;
        internal ShowChargerIconsChangedEvent ShowChargerIconsChanged;
        internal DebugLogsToggledEvent DebugLogsToggled;

        private IModConfig initialValues;

        public ModConfigMenuOptions() : base("MoreCyclopsUpgrades Options")
        {
            base.ToggleChanged += (object sender, ToggleChangedEventArgs e) =>
            {
                if (e.Id == AuxConsoleId)
                    AuxConsoleToggled?.Invoke(e.Value);
            };
            base.ChoiceChanged += (object sender, ChoiceChangedEventArgs e) =>
            {
                if (e.Id == ChallengeModeId)
                    ChallengeModeChanged?.Invoke((ChallengeLevel)e.Index);
            };
            base.SliderChanged += (object sender, SliderChangedEventArgs e) =>
            {
                if (e.Id == DeficitThresholdId)
                    DeficitThresholdChanged?.Invoke(Mathf.Round(e.Value));
            };
            base.ChoiceChanged += (object sender, ChoiceChangedEventArgs e) =>
            {
                if (e.Id == ShowChargerIconseId)
                    ShowChargerIconsChanged?.Invoke((ShowChargerIcons)e.Index);
            };
        }

        internal void Register(IModConfig startingValues)
        {
            initialValues = startingValues;

            OptionsPanelHandler.RegisterModOptions(this);
        }

        public override void BuildModOptions()
        {
            base.AddToggleOption(AuxConsoleId, "Enable Aux Upgrade Console (Requires restart)", initialValues.AuxConsoleEnabled);
            base.AddChoiceOption<ChallengeLevel>(ChallengeModeId, "Challenge Mode (Requires restart)", initialValues.ChallengeMode);
            base.AddSliderOption(DeficitThresholdId, "Use non-renewable chargers below %", ModConfig.MinThreshold, ModConfig.MaxThreshold, initialValues.DeficitThreshold);
            base.AddChoiceOption<ShowChargerIcons>(ShowChargerIconseId, "Charging Icons", initialValues.ChargerIcons);
            base.AddToggleOption(AuxConsoleId, "Enable Debug Logs (Requires restart)", initialValues.DebugLogsEnabled);
        }
    }
}
