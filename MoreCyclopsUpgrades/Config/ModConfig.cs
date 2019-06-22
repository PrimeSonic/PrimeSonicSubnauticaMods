namespace MoreCyclopsUpgrades.Config
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using Common;
    using MoreCyclopsUpgrades.Config.Options;
    using UnityEngine;

    internal class ModConfig : IModConfig
    {
        private static readonly ModConfig main = new ModConfig();
        internal static IModConfig Main
        {
            get
            {
                if (!main.initialized)
                    main.Initialize();

                return main;
            }
        }

        private bool initialized = false;

        private const string AuxConsoleEnabledKey = "AuxConsoleEnabled";
        private const string ChallengeModeKey = "ChallengeMode";
        private const string DeficitThresholdKey = "DeficitThreshold";
        private const string ChargerIconsKey = "ShowChargerIcons";
        private const string DebugLogsEnabledKey = "EnableDebugLogs";

        private readonly ToggleOption auxConsoleEnabled = new ToggleOption(AuxConsoleEnabledKey, "Enable Aux Upgrade Console (Requires restart)")
        {
            State = true
        };
        private readonly ChoiceOption challengeMode = new ChoiceOption(ChallengeModeKey, "Challenge Mode (Requires restart)")
        {
            Choices = new string[3] { $"{ChallengeLevel.Easy}", $"{ChallengeLevel.Normal}", $"{ChallengeLevel.Hard}" },
            Index = (int)ChallengeLevel.Easy
        };
        private readonly SliderOption deficitThreshHold = new SliderOption(DeficitThresholdKey, "Use non-renewable energy below %")
        {
            MinValue = 10f,
            MaxValue = 99f,
            Value = 95f
        };
        private readonly ChoiceOption showIcons = new ChoiceOption(ChargerIconsKey, "Charging Status Icons")
        {
            Choices = new string[4] { $"{ShowChargerIcons.Never}", $"{ShowChargerIcons.WhenPiloting}", $"{ShowChargerIcons.OnHoloDisplay}", $"{ShowChargerIcons.Everywhere}", },
            Index = (int)ShowChargerIcons.Everywhere
        };
        private readonly ToggleOption debugLogs = new ToggleOption(DebugLogsEnabledKey, "Enable Debug Logs(Requires restart)")
        {
            State = false
        };

        private readonly ModConfigSaveData saveData;
        private readonly ModConfigMenuOptions menuOptions;

        private ModConfig()
        {
            var configOptions = new List<ConfigOption>(5)
            {
                auxConsoleEnabled, challengeMode, deficitThreshHold, showIcons, debugLogs
            };

            saveData = new ModConfigSaveData(configOptions);
            menuOptions = new ModConfigMenuOptions(configOptions);
        }

        public bool AuxConsoleEnabled
        {
            get => auxConsoleEnabled.SaveData.Value;
            set
            {
                auxConsoleEnabled.SaveData.Value = value;
                auxConsoleEnabled.State = value;
                SaveData();
            }
        }

        public ChallengeLevel ChallengeMode
        {
            get => (ChallengeLevel)challengeMode.SaveData.Value;
            set
            {
                challengeMode.SaveData.Value = (int)value;
                challengeMode.Index = (int)value;
                SaveData();
            }
        }

        public float DeficitThreshold
        {
            get => deficitThreshHold.SaveData.Value;
            set
            {
                float roundedValue = Mathf.Round(value);
                deficitThreshHold.SaveData.Value = roundedValue;
                deficitThreshHold.Value = roundedValue;
                SaveData();
            }
        }

        public ShowChargerIcons ChargerIcons
        {
            get => (ShowChargerIcons)showIcons.SaveData.Value;
            set
            {
                showIcons.SaveData.Value = (int)value;
                showIcons.Index = (int)value;
                SaveData();
            }
        }

        public bool DebugLogsEnabled
        {
            get => debugLogs.SaveData.Value;
            set
            {
                debugLogs.SaveData.Value = value;
                debugLogs.State = value;
                QuickLogger.DebugLogsEnabled = value;
                SaveData();
            }
        }

        public bool ShowIconsWhilePiloting => this.ChargerIcons == ShowChargerIcons.Everywhere || this.ChargerIcons == ShowChargerIcons.WhenPiloting;

        public bool ShowIconsOnHoloDisplay => this.ChargerIcons == ShowChargerIcons.Everywhere || this.ChargerIcons == ShowChargerIcons.OnHoloDisplay;

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
                    default: // ChallengeLevel.Easy
                        return 1.0f;
                }
            }
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

            auxConsoleEnabled.SaveData = saveData.GetBoolProperty(auxConsoleEnabled.Id);
            auxConsoleEnabled.OptionToggled = (bool value) => { this.AuxConsoleEnabled = value; };

            challengeMode.SaveData = saveData.GetIntProperty(challengeMode.Id);
            challengeMode.ChoiceChanged = (int value) => { this.ChallengeMode = (ChallengeLevel)value; };

            deficitThreshHold.SaveData = saveData.GetFloatProperty(deficitThreshHold.Id);
            deficitThreshHold.ValueChanged = (float value) => { this.DeficitThreshold = value; };

            showIcons.SaveData = saveData.GetIntProperty(showIcons.Id);
            showIcons.ChoiceChanged = (int value) => { this.ChargerIcons = (ShowChargerIcons)value; };

            debugLogs.SaveData = saveData.GetBoolProperty(debugLogs.Id);
            debugLogs.OptionToggled = (bool value) => { this.DebugLogsEnabled = value; };

            initialized = true;
        }

        private void SaveData()
        {
            var bgWork = new Thread(new ParameterizedThreadStart((object data) => { (data as ModConfigSaveData).SaveToFile(); }));
            bgWork.Start(saveData);
        }

        private float CyclopsMaxPower = 1f;

        public float MinimumEnergyDeficit { get; private set; } = 1140f;

        public void UpdateCyclopsMaxPower(float maxPower)
        {
            if (CyclopsMaxPower == maxPower)
                return;

            CyclopsMaxPower = maxPower;

            this.MinimumEnergyDeficit = Mathf.Round(CyclopsMaxPower - CyclopsMaxPower * this.DeficitThreshold / 100f);
        }
    }
}
