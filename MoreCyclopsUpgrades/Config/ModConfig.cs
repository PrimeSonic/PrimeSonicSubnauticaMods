namespace MoreCyclopsUpgrades.Config
{
    using System;
    using Common;
    using SMLHelper.V2.Handlers;
    using UnityEngine;

    internal class ModConfig
    {
        internal static ModConfig Main { get; } = new ModConfig();

        private ModConfig() { }

        internal const float MinThreshold = 10f;
        internal const float MaxThreshold = 99f;
        internal const bool AuxConsoleDefaultEnabled = true;
        internal const ChallengeLevel DefaultChallenge = ChallengeLevel.Easy;
        internal const float DefaultThreshold = 95f;

        private readonly ModConfigSaveData saveData = new ModConfigSaveData();
        private readonly ModConfigMenuOptions menuOptions = new ModConfigMenuOptions();

        public bool AuxConsoleEnabled
        {
            get => saveData.AuxConsoleEnabled.Value;
            set => saveData.AuxConsoleEnabled.Value = value;
        }

        public ChallengeLevel ChallengeMode
        {
            get => saveData.ChallengeMode.Value;
            set => saveData.ChallengeMode.Value = value;
        }

        public float DeficitThreshold
        {
            get => saveData.DeficitThreshold.Value;
            set => saveData.DeficitThreshold.Value = value;
        }

        internal void Initialize()
        {
            try
            {
                saveData.LoadFromFile();                
            }
            catch (Exception ex)
            {
                QuickLogger.Error($"Error loading config save data: " + ex.ToString());
                saveData.SaveToFile();
                QuickLogger.Info($"Default config save data file created");
            }

            OptionsPanelHandler.RegisterModOptions(menuOptions);

            menuOptions.AuxConsoleToggled += (bool value) =>
            {
                this.AuxConsoleEnabled = value;
                saveData.SaveToFile();
            };
            menuOptions.ChallengeModeChanged += (ChallengeLevel value) =>
            {
                this.ChallengeMode = value;
                saveData.SaveToFile();
            };
            menuOptions.DeficitThresholdChanged += (float value) =>
            {
                this.DeficitThreshold = value;
                saveData.SaveToFile();
            };
        }

        private float CyclopsMaxPower = 1f;

        internal float MinimumEnergyDeficit { get; private set; } = 1140f;

        internal void UpdateCyclopsMaxPower(float maxPower)
        {
            if (CyclopsMaxPower == maxPower)
                return;

            CyclopsMaxPower = maxPower;

            this.MinimumEnergyDeficit = Mathf.Round(CyclopsMaxPower - CyclopsMaxPower * this.DeficitThreshold / 100f);
        }
    }
}
