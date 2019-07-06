namespace MoreCyclopsUpgrades.Config
{
    using System;
    using System.Collections.Generic;
    using Common;
    using CommonCyclopsUpgrades;
    using CommonCyclopsUpgrades.Options;
    using MoreCyclopsUpgrades.Config.ChoiceEnums;
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

        private readonly ToggleOption auxConsoleEnabled = new ToggleOption(AuxConsoleEnabledKey, "Enable AuxUpgradeConsole (Requires restart)")
        {
            State = true
        };
        private readonly ChoiceOption challengeMode = new ChoiceOption(ChallengeModeKey, "Challenge Mode")
        {
            Choices = new string[3]
            {
                $"{ChallengeMode.Easy}",
                $"{ChallengeMode.Medium}",
                $"{ChallengeMode.Hard}"
            },
            Index = (int)ChallengeMode.Easy
        };
        private readonly SliderOption deficitThreshHold = new SliderOption(DeficitThresholdKey, "Use non-renewable energy below %")
        {
            MinValue = 10f,
            MaxValue = 99f,
            Value = 95f
        };
        private readonly ChoiceOption showIcons = new ChoiceOption(ChargerIconsKey, "Charging Status Icons")
        {
            Choices = new string[4]
            {
                $"{ShowChargerIcons.Never}",
                $"{ShowChargerIcons.WhenPiloting}",
                $"{ShowChargerIcons.OnHoloDisplay}",
                $"{ShowChargerIcons.Everywhere}",
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
                $"{HelmEnergyDisplay.PowerCellPercentage}",
                $"{HelmEnergyDisplay.PowerCellAmount}",
                $"{HelmEnergyDisplay.PercentageOverPowerCells}",
                $"{HelmEnergyDisplay.CombinedAmount}"
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

                float rechargePenalty = this.RechargePenalty;
                IEnumerable<CyclopsManager> cyManagers = CyclopsManager.GetAllManagers();
                foreach (CyclopsManager mgr in cyManagers)
                {                    
                    mgr.Charge.UpdateRechargePenalty(rechargePenalty);
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
                this.ShowIconsWhilePiloting = value == ShowChargerIcons.Everywhere || value == ShowChargerIcons.WhenPiloting;
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

        public bool ShowIconsWhilePiloting { get; private set; }

        public bool ShowIconsOnHoloDisplay { get; private set; }

        public float RechargePenalty
        {
            get
            {
                switch (this.ChallengeMode)
                {
                    case ChallengeMode.Hard:
                        return 0.70f; // -30%
                    case ChallengeMode.Medium:
                        return 0.85f; // -15%
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
            challengeMode.ChoiceChanged = (int index) => { this.ChallengeMode = (ChallengeMode)index; };

            deficitThreshHold.SaveData = saveData.GetFloatProperty(deficitThreshHold.Id);
            deficitThreshHold.ValueChanged = (float value) => { this.DeficitThreshold = value; };

            showIcons.SaveData = saveData.GetIntProperty(showIcons.Id);
            showIcons.ChoiceChanged = (int index) => { this.ChargerIcons = (ShowChargerIcons)index; };

            debugLogs.SaveData = saveData.GetBoolProperty(debugLogs.Id);
            debugLogs.OptionToggled = (bool value) => { this.DebugLogsEnabled = value; };

            energyDisplay.SaveData = saveData.GetIntProperty(energyDisplay.Id);
            energyDisplay.ChoiceChanged = (int index) => { this.EnergyDisplay = (HelmEnergyDisplay)index; };

            menuOptions.Register();
            initialized = true;
        }

        private void SaveData()
        {
            saveData.SaveToFile();
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
