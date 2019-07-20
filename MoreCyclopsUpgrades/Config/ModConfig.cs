namespace MoreCyclopsUpgrades.Config
{
    using System;
    using System.Collections.Generic;
    using Common;
    using MoreCyclopsUpgrades.Config.ChoiceEnums;
    using MoreCyclopsUpgrades.Config.Options;
    using MoreCyclopsUpgrades.Managers;
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
        private const string HelmEnergyDisplayKey = "HelmEnergyDisplay";

        private readonly ToggleOption auxConsoleEnabled = new ToggleOption(AuxConsoleEnabledKey, "Enable AuxUpgradeConsole                        (Restart game)")
        {
            State = true
        };
        private readonly ChoiceOption challengeMode = new ChoiceOption(ChallengeModeKey, "Challenge (Engine Penalty)")
        {
            Choices = new string[3]
            {
                $"{ChallengeMode.Easy.AsDisplay()}",
                $"{ChallengeMode.Medium.AsDisplay()}",
                $"{ChallengeMode.Hard.AsDisplay()}"
            },
            Index = (int)ChallengeMode.Easy
        };
        private readonly SliderOption deficitThreshHold = new SliderOption(DeficitThresholdKey, "Conserve chargers when over %")
        {
            MinValue = 10f,
            MaxValue = 99f,
            Value = 95f
        };
        private readonly ChoiceOption showIcons = new ChoiceOption(ChargerIconsKey, "Charging Status Icons")
        {
            Choices = new string[4]
            {
                $"{ShowChargerIcons.Never.AsDisplay()}",
                $"{ShowChargerIcons.OnPilotingHUD.AsDisplay()}",
                $"{ShowChargerIcons.OnHoloDisplay.AsDisplay()}",
                $"{ShowChargerIcons.Everywhere.AsDisplay()}",
            },
            Index = (int)ShowChargerIcons.Everywhere
        };
        private readonly ToggleOption debugLogs = new ToggleOption(DebugLogsEnabledKey, "Enable Debug Logs")
        {
#if DEBUG
            State = true // Default debug logs to true for Debug builds
#else
            State = false // Default debug logs to false for Release builds
#endif
        };
        private readonly ChoiceOption energyDisplay = new ChoiceOption(HelmEnergyDisplayKey, "Helm HUD Energy Display")
        {
            Choices = new string[4]
            {
                $"{HelmEnergyDisplay.PowerCellPercentage.AsDisplay()}",
                $"{HelmEnergyDisplay.PowerCellAmount.AsDisplay()}",
                $"{HelmEnergyDisplay.PercentageOverPowerCells.AsDisplay()}",
                $"{HelmEnergyDisplay.CombinedAmount.AsDisplay()}"
            },
            Index = (int)HelmEnergyDisplay.PowerCellPercentage
        };

        private readonly ModConfigSaveData saveData;
        private readonly ModConfigMenuOptions menuOptions;

        private ModConfig()
        {
            var configOptions = new List<ConfigOption>(6)
            {
                auxConsoleEnabled, challengeMode, deficitThreshHold, showIcons, debugLogs, energyDisplay
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

        public ChallengeMode ChallengeMode
        {
            get => (ChallengeMode)challengeMode.SaveData.Value;
            set
            {
                challengeMode.SaveData.Value = (int)value;
                challengeMode.Index = (int)value;
                SaveData();

                float rechargePenalty = 1f - value.ChallengePenalty();
                this.RechargePenalty = rechargePenalty;

                for (int m = 0; m < CyclopsManager.Managers.Count; m++)
                {
                    CyclopsManager mgr = CyclopsManager.Managers[m];
                    mgr.Charge.RechargePenalty = rechargePenalty;
                    mgr.Cyclops.UpdatePowerRating();
                }
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
                this.ShowIconsOnHoloDisplay = value == ShowChargerIcons.Everywhere || value == ShowChargerIcons.OnHoloDisplay;
                this.ShowIconsWhilePiloting = value == ShowChargerIcons.Everywhere || value == ShowChargerIcons.OnPilotingHUD;
                this.HidePowerIcons = value == ShowChargerIcons.Never;
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

        public HelmEnergyDisplay EnergyDisplay
        {
            get => (HelmEnergyDisplay)energyDisplay.SaveData.Value;
            set
            {
                energyDisplay.SaveData.Value = (int)value;
                energyDisplay.Index = (int)value;
                SaveData();
            }
        }

        public bool HidePowerIcons { get; private set; } = false;

        public bool ShowIconsWhilePiloting { get; private set; } = true;

        public bool ShowIconsOnHoloDisplay { get; private set; } = true;

        public float RechargePenalty { get; private set; } = 1f;

        internal void Initialize()
        {
            QuickLogger.Info("Initializing mod config");
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

            // Load values from Save Data
            auxConsoleEnabled.SaveData = saveData.GetBoolProperty(auxConsoleEnabled.Id);
            challengeMode.SaveData = saveData.GetIntProperty(challengeMode.Id);
            deficitThreshHold.SaveData = saveData.GetFloatProperty(deficitThreshHold.Id);
            showIcons.SaveData = saveData.GetIntProperty(showIcons.Id);
            debugLogs.SaveData = saveData.GetBoolProperty(debugLogs.Id);
            energyDisplay.SaveData = saveData.GetIntProperty(energyDisplay.Id);

            // Update current settings to match save data
            this.AuxConsoleEnabled = auxConsoleEnabled.SaveData.Value;
            this.ChallengeMode = (ChallengeMode)challengeMode.SaveData.Value;
            this.DeficitThreshold = deficitThreshHold.SaveData.Value;
            this.ChargerIcons = (ShowChargerIcons)showIcons.SaveData.Value;
            this.DebugLogsEnabled = debugLogs.SaveData.Value;
            this.EnergyDisplay = (HelmEnergyDisplay)energyDisplay.SaveData.Value;

            // Link event handlers to accept changes from in-game menu
            auxConsoleEnabled.OptionToggled = (bool value) => { this.AuxConsoleEnabled = value; };
            challengeMode.ChoiceChanged = (int index) => { this.ChallengeMode = (ChallengeMode)index; };
            deficitThreshHold.ValueChanged = (float value) => { this.DeficitThreshold = value; };
            showIcons.ChoiceChanged = (int index) => { this.ChargerIcons = (ShowChargerIcons)index; };
            debugLogs.OptionToggled = (bool value) => { this.DebugLogsEnabled = value; };
            energyDisplay.ChoiceChanged = (int index) => { this.EnergyDisplay = (HelmEnergyDisplay)index; };

            menuOptions.Register();

            initialized = true;
        }

        private void SaveData()
        {
            saveData.SaveToFile();
        }

        private float CyclopsMaxPower = 1f;

        public float MinimumEnergyDeficit { get; private set; } = 60f;
        public float EmergencyEnergyDeficit { get; private set; } = 600f;

        public void UpdateCyclopsMaxPower(float maxPower)
        {
            CyclopsMaxPower = maxPower;
            float ratio = this.DeficitThreshold / 100f;
            this.MinimumEnergyDeficit = Mathf.Round(CyclopsMaxPower - CyclopsMaxPower * ratio);

            const float emergencyPowerKickIn = 0.5f;
            this.EmergencyEnergyDeficit = Mathf.Round(CyclopsMaxPower - CyclopsMaxPower * emergencyPowerKickIn);
        }
    }
}
