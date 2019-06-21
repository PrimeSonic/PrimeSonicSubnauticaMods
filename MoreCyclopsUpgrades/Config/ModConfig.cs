namespace MoreCyclopsUpgrades.Config
{
    using System;
    using System.Threading;
    using Common;
    using UnityEngine;

    internal class ModConfig : IModConfig
    {
        internal static ModConfig Main { get; } = new ModConfig();

        private ModConfig() { }

        internal const float MinThreshold = 10f;
        internal const float MaxThreshold = 99f;
        internal const bool AuxConsoleDefaultEnabled = true;
        internal const ChallengeLevel DefaultChallenge = ChallengeLevel.Easy;
        internal const float DefaultThreshold = 95f;
        internal const ShowChargerIcons DefaultChargerIcons = ShowChargerIcons.Both;

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

        public ShowChargerIcons ChargerIcons
        {
            get => saveData.ChargerIcons.Value;
            set
            {
                saveData.ChargerIcons.Value = value;
                this.ShowIconsWhilePiloting = (value & ShowChargerIcons.WhenPiloting) == ShowChargerIcons.WhenPiloting;
                this.ShowIconsAtHelm = (value & ShowChargerIcons.HelmDisplay) == ShowChargerIcons.HelmDisplay;
            }
        }

        public bool DebugLogsEnabled
        {
            get => saveData.AuxConsoleEnabled.Value;
            set
            {
                saveData.AuxConsoleEnabled.Value = value;
                QuickLogger.DebugLogsEnabled = value;
            }
        }

        public bool ShowIconsWhilePiloting { get; private set; } = true;

        public bool ShowIconsAtHelm { get; private set; } = true;

        public float RechargePenalty
        {
            get
            {
                switch (this.ChallengeMode)
                {
                    case ChallengeLevel.Hard:
                        return 0.70f;
                    case ChallengeLevel.Normal:
                        return 0.85f;                    
                    default:
                        return 1f;
                }
            }
        }

        internal void Initialize()
        {
            try
            {
                saveData.LoadFromFile();
                this.ShowIconsWhilePiloting = (this.ChargerIcons & ShowChargerIcons.WhenPiloting) == ShowChargerIcons.WhenPiloting;
                this.ShowIconsAtHelm = (this.ChargerIcons & ShowChargerIcons.HelmDisplay) == ShowChargerIcons.HelmDisplay;
            }
            catch (Exception ex)
            {
                QuickLogger.Error($"Error loading config save data: " + ex.ToString());
                saveData.SaveToFile();
                QuickLogger.Info($"Default config save data file created");
            }

            menuOptions.AuxConsoleToggled = (bool value) =>
            {
                this.AuxConsoleEnabled = value;
                SaveData();
            };
            menuOptions.ChallengeModeChanged = (ChallengeLevel value) =>
            {
                this.ChallengeMode = value;
                SaveData();
            };
            menuOptions.DeficitThresholdChanged = (float value) =>
            {
                this.DeficitThreshold = value;
                SaveData();
            };
            menuOptions.ShowChargerIconsChanged = (ShowChargerIcons value) =>
            {
                this.ChargerIcons = value;
                SaveData();
            };
            menuOptions.DebugLogsToggled = (bool value) =>
            {
                this.DebugLogsEnabled = value;
                SaveData();
            };
        }

        private void SaveData()
        {
            var bgWork = new Thread(new ParameterizedThreadStart((object data) => { (data as ModConfigSaveData).SaveToFile(); }));
            bgWork.Start(saveData);
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
