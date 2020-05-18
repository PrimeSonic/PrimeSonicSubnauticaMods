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

        internal static void LoadOnDemand()
        {
            if (!main.initialized)
                main.Initialize();
        }

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

        private readonly ToggleOption auxConsoleEnabled = new ToggleOption(nameof(AuxConsoleEnabled), "Enable AuxUpgradeConsole (Restart game)")
        {
            State = true,
            OptionToggled = (bool value, ModConfig config) => { config.AuxConsoleEnabled = value; },
        };
        private readonly ChoiceOption challengeMode = new ChoiceOption(nameof(ChallengeMode), "Challenge (Engine Penalty)")
        {
            Choices = new string[3]
            {
                $"{ChallengeMode.Easy.AsDisplay()}",
                $"{ChallengeMode.Medium.AsDisplay()}",
                $"{ChallengeMode.Hard.AsDisplay()}"
            },
            Index = (int)ChallengeMode.Easy,
            ChoiceChanged = (int index, ModConfig config) => { config.ChallengeMode = (ChallengeMode)index; }
        };
        private readonly SliderOption deficitThreshHold = new SliderOption(nameof(DeficitThreshold), "Conserve chargers when over %")
        {
            MinValue = 10f,
            MaxValue = 100f,
            Value = 95f,
            ValueChanged = (float value, ModConfig config) => { config.DeficitThreshold = value; }
        };
        private readonly ChoiceOption showIcons = new ChoiceOption(nameof(ShowChargerIcons), "Charging Status Icons")
        {
            Choices = new string[4]
            {
                $"{ShowChargerIcons.Never.AsDisplay()}",
                $"{ShowChargerIcons.OnPilotingHUD.AsDisplay()}",
                $"{ShowChargerIcons.OnHoloDisplay.AsDisplay()}",
                $"{ShowChargerIcons.Everywhere.AsDisplay()}",
            },
            Index = (int)ShowChargerIcons.Everywhere,
            ChoiceChanged = (int index, ModConfig config) => { config.ChargerIcons = (ShowChargerIcons)index; }
        };
        private readonly ToggleOption debugLogs = new ToggleOption(nameof(EnableDebugLogs), "Enable Debug Logs")
        {
            State = false,
            OptionToggled = (bool value, ModConfig config) => { config.EnableDebugLogs = value; }
        };
        private readonly ChoiceOption energyDisplay = new ChoiceOption(nameof(HelmEnergyDisplay), "Helm HUD Energy Display")
        {
            Choices = new string[4]
            {
                $"{HelmEnergyDisplay.PowerCellPercentage.AsDisplay()}",
                $"{HelmEnergyDisplay.PowerCellAmount.AsDisplay()}",
                $"{HelmEnergyDisplay.PercentageOverPowerCells.AsDisplay()}",
                $"{HelmEnergyDisplay.CombinedAmount.AsDisplay()}"
            },
            Index = (int)HelmEnergyDisplay.PowerCellPercentage,
            ChoiceChanged = (int index, ModConfig config) => { config.EnergyDisplay = (HelmEnergyDisplay)index; }
        };
        private readonly ToggleOption showThermometer = new ToggleOption(nameof(ShowThermometer), "Show Thermometer")
        {
            State = true,
            OptionToggled = (bool value, ModConfig config) => { config.ShowThermometer = value; }
        };

        private readonly ModConfigSaveData saveData;
        private readonly ModConfigMenuOptions menuOptions;
        private readonly List<ConfigOption> configOptions;

        private ModConfig()
        {
            configOptions = new List<ConfigOption>(7)
            {
                auxConsoleEnabled, challengeMode, deficitThreshHold, showIcons, debugLogs, energyDisplay, showThermometer
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
                saveData.SaveToFile();
            }
        }

        public ChallengeMode ChallengeMode
        {
            get => (ChallengeMode)challengeMode.SaveData.Value;
            set
            {
                challengeMode.SaveData.Value = (int)value;
                challengeMode.Index = (int)value;
                saveData.SaveToFile();

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
                saveData.SaveToFile();
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
                saveData.SaveToFile();
            }
        }

        public bool EnableDebugLogs
        {
            get => debugLogs.SaveData.Value;
            set
            {
                debugLogs.SaveData.Value = value;
                debugLogs.State = value;
                QuickLogger.DebugLogsEnabled = value;
                saveData.SaveToFile();
            }
        }

        public HelmEnergyDisplay EnergyDisplay
        {
            get => (HelmEnergyDisplay)energyDisplay.SaveData.Value;
            set
            {
                energyDisplay.SaveData.Value = (int)value;
                energyDisplay.Index = (int)value;
                saveData.SaveToFile();
            }
        }

        public bool HidePowerIcons { get; private set; } = false;

        public bool ShowIconsWhilePiloting { get; private set; } = true;

        public bool ShowThermometer
        {
            get => showThermometer.SaveData.Value;
            set
            {
                showThermometer.SaveData.Value = value;
                showThermometer.State = value;
                saveData.SaveToFile();
            }
        }

        public bool ShowIconsOnHoloDisplay { get; private set; } = true;

        public float RechargePenalty { get; private set; } = 1f;

        internal void Initialize()
        {
            QuickLogger.Info("Initializing config settings");
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

            
            foreach (var option in configOptions)
            {
                // Load values from Save Data
                option.LoadFromSaveData(saveData);

                // Update current settings to match save data
                option.UpdateProperty(this);
            }            

            // Link event handlers to accept changes from in-game menu
            menuOptions.RegisterEvents(this);

            QuickLogger.DebugLogsEnabled = this.EnableDebugLogs;
            QuickLogger.Info($"Debug logging is {(this.EnableDebugLogs ? "en" : "dis")}abled");

            initialized = true;
        }

        private float CyclopsMaxPower = 1f;

        public float MinimumEnergyDeficit { get; private set; } = 60f;

        public float EmergencyEnergyDeficit { get; private set; } = 6f;

        public void UpdateCyclopsMaxPower(float maxPower)
        {
            if (CyclopsMaxPower == maxPower)
                return;

            CyclopsMaxPower = maxPower;
            float ratio = this.DeficitThreshold / 100f;
            this.MinimumEnergyDeficit = Mathf.Round(CyclopsMaxPower - CyclopsMaxPower * ratio);

            this.EmergencyEnergyDeficit = Mathf.Round(CyclopsMaxPower / 2f);
        }

    }
}
