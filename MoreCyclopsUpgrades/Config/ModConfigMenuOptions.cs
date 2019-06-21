namespace MoreCyclopsUpgrades.Config
{
    using SMLHelper.V2.Options;
    using UnityEngine;

    internal class ModConfigMenuOptions : ModOptions
    {
        private const string AuxConsoleId = "MCUConsole";
        private const string ChallengeModeId = "MCUChallenge";
        private const string DeficitThresholdId = "MCUThreshold";

        public delegate void AuxConsoleToggledEvent(bool value);
        public delegate void ChallengeModeChangedEvent(ChallengeLevel value);
        public delegate void DeficitThresholdChangedEvent(float value);

        internal AuxConsoleToggledEvent AuxConsoleToggled;
        internal ChallengeModeChangedEvent ChallengeModeChanged;
        internal DeficitThresholdChangedEvent DeficitThresholdChanged;

        private bool consoleStartingState;
        private ChallengeLevel challengeStartingState;
        private float thresholdStartingState;

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
        }

        internal void Register(bool console, ChallengeLevel challenge, float threshold)
        {
            consoleStartingState = console;
            challengeStartingState = challenge;
            thresholdStartingState = threshold;
        }

        public override void BuildModOptions()
        {
            base.AddToggleOption(AuxConsoleId, "Enable Aux Upgrade Console (Requires restart!)", consoleStartingState);
            base.AddChoiceOption<ChallengeLevel>(ChallengeModeId, "Challenge Mode (Requires restart!)", challengeStartingState);
            base.AddSliderOption(DeficitThresholdId, "Use non-renewable chargers below %", ModConfig.MinThreshold, ModConfig.MaxThreshold, thresholdStartingState);
        }
    }
}
