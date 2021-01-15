namespace MoreCyclopsUpgrades.Managers
{
    using System;
    using System.Collections.Generic;
    using Common;
    using MoreCyclopsUpgrades.API.Charging;
    using MoreCyclopsUpgrades.API.StatusIcons;
    using MoreCyclopsUpgrades.Config;
    using MoreCyclopsUpgrades.Config.ChoiceEnums;
    using UnityEngine;
    using UnityEngine.UI;

    internal class CyclopsHUDManager
    {
        private class StatusIconCreator
        {
            public readonly CyclopsStatusIconCreator Creator;
            public readonly string StatusIconName;

            public StatusIconCreator(CyclopsStatusIconCreator creator, string stutusIconName)
            {
                Creator = creator;
                StatusIconName = stutusIconName;
            }
        }

        internal static bool TooLateToRegister { get; private set; }

        private static readonly List<StatusIconCreator> StatusIconCreators = new List<StatusIconCreator>();
        internal static readonly List<CyclopsStatusIcon> StatusIcons = new List<CyclopsStatusIcon>();

        internal static void RegisterStatusIconCreator(CyclopsStatusIconCreator creatorEvent, string name)
        {
            if (StatusIconCreators.Find(s => s.Creator == creatorEvent || s.StatusIconName == name) != null)
            {
                QuickLogger.Warning($"Duplicate CyclopsStatusIconCreator '{name}' was blocked");
                return;
            }

            QuickLogger.Info($"Received CyclopsStatusIconCreator '{name}'");
            StatusIconCreators.Add(new StatusIconCreator(creatorEvent, name));
        }

        internal static Atlas.Sprite CyclopsThermometer;

        private IndicatorIcon TemperatureReadout;
        private IndicatorIcon[] HelmIndicatorsOdd;
        private IndicatorIcon[] HelmIndicatorsEven;
        private IndicatorIcon[] HealthBarIndicatorsOdd;
        private IndicatorIcon[] HealthBarIndicatorsEven;

        private bool lastKnownTextVisibility = false;
        private bool powerIconTextVisibility = false;

        private const float delayInterval = 0.9876f;
        private float iconUpdateDelay = Time.deltaTime;

        private SubRoot Cyclops;
        private Text upgradesText;

        private ChargeManager chargeManager;
        private ChargeManager ChargeManager => chargeManager ?? (chargeManager = CyclopsManager.GetManager(ref Cyclops).Charge);

        private bool powerIconsInitialized = false;
        private bool consoleIconsRemoved = false;

        private CyclopsHolographicHUD holographicHUD;
        private readonly IModConfig settings = ModConfig.Main;

        internal CyclopsHUDManager(SubRoot cyclops)
        {
            Cyclops = cyclops;            
        }

        internal void FastUpdate(CyclopsHelmHUDManager cyclopsHelmHUD)
        {
            if (!powerIconsInitialized)
                AddStatusIcons(cyclopsHelmHUD);
            else
                UpdateStatusIcons();

            PowerRelay powerRelay = Cyclops.powerRelay;

            switch (settings.EnergyDisplay)
            {
                case HelmEnergyDisplay.PowerCellAmount:
                    cyclopsHelmHUD.powerText.text = NumberFormatter.FormatValue(powerRelay.GetPower());
                    break;
                case HelmEnergyDisplay.PercentageOverPowerCells:
                    float percentOver = (powerRelay.GetPower() + this.ChargeManager.GetTotalReservePower()) / powerRelay.GetMaxPower();
                    cyclopsHelmHUD.powerText.text = $"{NumberFormatter.FormatValue(percentOver * 100f)}%";
                    break;
                case HelmEnergyDisplay.CombinedAmount:
                    cyclopsHelmHUD.powerText.text = NumberFormatter.FormatValue(powerRelay.GetPower() + this.ChargeManager.GetTotalReservePower());
                    break;
                default: // HelmEnergyDisplay.PowerCellPercentage:
                    float percent = powerRelay.GetPower() / powerRelay.GetMaxPower();
                    cyclopsHelmHUD.powerText.text = $"{NumberFormatter.FormatValue(percent * 100f)}%";
                    break;
            }
        }

        /// <summary>
        /// Updates the console HUD using data from all equipment modules across all upgrade consoles.
        /// </summary>
        /// <param name="hudManager">The console HUD manager.</param>
        internal void SlowUpdate(CyclopsUpgradeConsoleHUDManager hudManager)
        {
            if (!consoleIconsRemoved)
            {
                hudManager.ToggleAllIconsOff();
                consoleIconsRemoved = true;
            }

            if (upgradesText == null)
                upgradesText = hudManager.subRoot.transform.Find("UpgradeConsoleHUD")?.Find("Canvas_Main")?.Find("Text")?.GetComponent<Text>();

            if (upgradesText != null)
            {
                upgradesText.fontSize = 70;
                upgradesText.text = hudManager.subRoot.GetSubName();
            }

            int currentReservePower = this.ChargeManager.GetTotalReservePower();
            float currentBatteryPower = Cyclops.powerRelay.GetPower();

            int TotalPowerUnits = Mathf.CeilToInt(currentBatteryPower + currentReservePower);
            float normalMaxPower = Cyclops.powerRelay.GetMaxPower();
            int normalMaxPowerInt = Mathf.CeilToInt(normalMaxPower);

            hudManager.energyCur.color = NumberFormatter.GetNumberColor(currentBatteryPower, normalMaxPower, 0f);
            hudManager.energyCur.text = NumberFormatter.FormatValue(TotalPowerUnits);

            if (hudManager.lastMaxSubPowerDisplayed != normalMaxPowerInt)
            {
                hudManager.energyMax.text = "/" + NumberFormatter.FormatValue(normalMaxPowerInt);
                hudManager.lastMaxSubPowerDisplayed = normalMaxPowerInt;
            }

            powerIconTextVisibility =
                    Player.main.currentSub == Cyclops &&
                    holographicHUD != null &&
                    Mathf.Abs(Vector3.Distance(holographicHUD.transform.position, Player.main.transform.position)) <= 4f;

            if (lastKnownTextVisibility != powerIconTextVisibility)
            {
                UpdateStatusIcons();
                lastKnownTextVisibility = powerIconTextVisibility;
            }
        }

        private void AddStatusIcons(CyclopsHelmHUDManager cyclopsHelmHUD)
        {
            TooLateToRegister = true;
            for (int i = 0; i < StatusIconCreators.Count; i++)
            {
                StatusIconCreator creator = StatusIconCreators[i];

                QuickLogger.Debug($"CyclopsHUDManager creating standalone status icon '{creator.StatusIconName}'");
                CyclopsStatusIcon statusIcon = creator.Creator.Invoke(Cyclops);

                if (statusIcon == null)
                {
                    QuickLogger.Warning($"CyclopsHUDManager '{creator.StatusIconName}' was null");
                }
                else
                {
                    StatusIcons.Add(statusIcon);
                    QuickLogger.Debug($"Created CyclopsStatusIcon '{creator.StatusIconName}'");
                }
            }

            int totalPowerInfoIcons = Math.Max(StatusIcons.Count, 1); // Include a minimum of 1 for the vanilla thermal charger
            cyclopsHelmHUD.powerText.resizeTextForBestFit = true;

            QuickLogger.Debug($"CyclopsHUDManager Adding '{totalPowerInfoIcons}' Status Info Icons");
            holographicHUD = cyclopsHelmHUD.subRoot.GetComponentInChildren<CyclopsHolographicHUD>();

            Canvas pilotingCanvas = cyclopsHelmHUD.powerText.canvas;
            Canvas holoCanvas = holographicHUD.healthBar.canvas;

            const float spacingUnit = 35;
            const float helmzoffset = 0.05f;
            const float helmyoffset = -225;
            const float helmscale = 1.35f;

            const float healthbarxoffset = 120f;
            const float thermometerxoffset = healthbarxoffset * 1.96f;
            const float healthbarzoffset = 0.05f;
            const float healthbaryoffset = -300f;
            const float healthbarscale = 0.65f;
            const float thermometerscale = healthbarscale * 1.25f;

            /* --- 3-1-2 --- */
            /* ---- 1-2 ---- */

            if (totalPowerInfoIcons == 1)
            {
                HelmIndicatorsOdd = new IndicatorIcon[1];
                HelmIndicatorsEven = new IndicatorIcon[0];
                HealthBarIndicatorsOdd = new IndicatorIcon[1];
                HealthBarIndicatorsEven = new IndicatorIcon[0];

                HelmIndicatorsOdd[0] = IconCreator.CreatePowerIndicatorIcon(pilotingCanvas, 0, helmyoffset, helmzoffset, helmscale);
                HealthBarIndicatorsOdd[0] = IconCreator.CreatePowerIndicatorIcon(holoCanvas, healthbarxoffset + 0, healthbaryoffset, healthbarzoffset, healthbarscale);
            }
            else
            {
                int totalIcons = totalPowerInfoIcons;

                if (totalIcons % 2 != 0)
                    totalIcons--;

                HelmIndicatorsOdd = new IndicatorIcon[totalIcons + 1];
                HelmIndicatorsEven = new IndicatorIcon[totalIcons];
                HealthBarIndicatorsOdd = new IndicatorIcon[totalIcons + 1];
                HealthBarIndicatorsEven = new IndicatorIcon[totalIcons];

                HelmIndicatorsOdd[0] = IconCreator.CreatePowerIndicatorIcon(pilotingCanvas, 0, helmyoffset, helmzoffset, helmscale);
                HealthBarIndicatorsOdd[0] = IconCreator.CreatePowerIndicatorIcon(holoCanvas, healthbarxoffset + 0, healthbaryoffset, healthbarzoffset, healthbarscale);

                int index = 0;
                float healtBarSpacing = 0f;
                float helmSpacing = 0f;
                do
                {
                    healtBarSpacing += spacingUnit;
                    helmSpacing += spacingUnit * 2f;

                    // Add even icons first
                    HelmIndicatorsEven[index + 0] = IconCreator.CreatePowerIndicatorIcon(pilotingCanvas, -helmSpacing, helmyoffset, helmzoffset, helmscale);
                    HelmIndicatorsEven[index + 1] = IconCreator.CreatePowerIndicatorIcon(pilotingCanvas, helmSpacing, helmyoffset, helmzoffset, helmscale);

                    HealthBarIndicatorsEven[index + 0] = IconCreator.CreatePowerIndicatorIcon(holoCanvas, healthbarxoffset + -healtBarSpacing, healthbaryoffset, healthbarzoffset, healthbarscale);
                    HealthBarIndicatorsEven[index + 1] = IconCreator.CreatePowerIndicatorIcon(holoCanvas, healthbarxoffset + healtBarSpacing, healthbaryoffset, healthbarzoffset, healthbarscale);

                    healtBarSpacing += spacingUnit;
                    helmSpacing += spacingUnit * 2f;

                    // Add odd icons next
                    HelmIndicatorsOdd[index + 1] = IconCreator.CreatePowerIndicatorIcon(pilotingCanvas, -helmSpacing, helmyoffset, helmzoffset, helmscale);
                    HelmIndicatorsOdd[index + 2] = IconCreator.CreatePowerIndicatorIcon(pilotingCanvas, helmSpacing, helmyoffset, helmzoffset, helmscale);

                    HealthBarIndicatorsOdd[index + 1] = IconCreator.CreatePowerIndicatorIcon(holoCanvas, healthbarxoffset + -healtBarSpacing, healthbaryoffset, healthbarzoffset, healthbarscale);
                    HealthBarIndicatorsOdd[index + 2] = IconCreator.CreatePowerIndicatorIcon(holoCanvas, healthbarxoffset + healtBarSpacing, healthbaryoffset, healthbarzoffset, healthbarscale);

                    index += 2;

                } while (totalIcons > index);
            }

            TemperatureReadout = IconCreator.CreatePowerIndicatorIcon(holoCanvas, thermometerxoffset, -2.5f, healthbarzoffset, thermometerscale);
            TemperatureReadout.Icon.sprite = CyclopsThermometer ?? SpriteManager.Get(TechType.CyclopsThermalReactorModule);

            powerIconsInitialized = true;

            QuickLogger.Debug("Linked CyclopsHUDManager to HelmHUD");
        }

        private void UpdateStatusIcons()
        {
            if (!powerIconsInitialized)
                return;

            if (Time.time < iconUpdateDelay)
                return;

            iconUpdateDelay = Time.time + delayInterval;

            HidePowerIcons();
            if (settings.ShowThermometer)
            {
                float temperature = Cyclops.GetTemperature();
                TemperatureReadout.Text.text = NumberFormatter.FormatValue(temperature) + "°C";
                TemperatureReadout.Text.color = GetHeatColor(temperature);
                TemperatureReadout.SetEnabled(true);
            }
            else
            {
                TemperatureReadout.SetEnabled(false);
            }

            if (settings.HidePowerIcons)
                return;            

            bool isEven = true;
            for (int i = 0; i < StatusIcons.Count; i++)
            {
                if (StatusIcons[i].ShowStatusIcon)
                    isEven = !isEven;
            }

            IndicatorIcon[] helmRow = isEven ? HelmIndicatorsEven : HelmIndicatorsOdd;
            IndicatorIcon[] healthBarRow = isEven ? HealthBarIndicatorsEven : HealthBarIndicatorsOdd;

            bool showIconsOnHoloDisplay = settings.ShowIconsOnHoloDisplay;
            bool showIconsWhilePiloting = settings.ShowIconsWhilePiloting;

            int iconIndex = 0;
            for (int c = 0; c < StatusIcons.Count; c++)
            {
                CyclopsStatusIcon statusIcon = StatusIcons[c];

                if (!statusIcon.ShowStatusIcon)
                    continue;

                IndicatorIcon helmIcon = helmRow[iconIndex];
                IndicatorIcon hpIcon = healthBarRow[iconIndex++];

                hpIcon.SetEnabled(showIconsOnHoloDisplay);
                helmIcon.SetEnabled(showIconsWhilePiloting);

                hpIcon.Icon.sprite = helmIcon.Icon.sprite = statusIcon.StatusSprite();

                hpIcon.Text.enabled = powerIconTextVisibility;
                hpIcon.Text.text = helmIcon.Text.text = statusIcon.StatusText();
                
                hpIcon.Text.color = helmIcon.Text.color = statusIcon.StatusTextColor();                
            }
        }

        private void HidePowerIcons()
        {
            for (int i = 0; i < HelmIndicatorsOdd.Length; i++)
            {
                HelmIndicatorsOdd[i].SetEnabled(false);
                HealthBarIndicatorsOdd[i].SetEnabled(false);
            }

            for (int i = 0; i < HelmIndicatorsEven.Length; i++)
            {
                HelmIndicatorsEven[i].SetEnabled(false);
                HealthBarIndicatorsEven[i].SetEnabled(false);
            }
        }

        private Color GetHeatColor(float temperatureValue)
        {
            const float white = 0f; // Invalid or freezing
            const float blue = 10f; // Cool water, doesn't thermal charge
            const float green = 35f; // Able to thermal charge, comfortable for player
            const float yellow = 50f; // Better thermal charging, damages player without protection
            const float red = 70f; // Best thermal charging, can damage player even with protection

            const float blue_white = blue - white;
            const float green_blue = green - blue;
            const float yellow_green = yellow - green;
            const float red_yellow = red - yellow;

            if (temperatureValue < white)
                return Color.white;

            if (temperatureValue < blue)
                return Color.Lerp(Color.white, Color.blue, (temperatureValue - white) / blue_white);

            if (temperatureValue < green)
                return Color.Lerp(Color.blue, Color.green, (temperatureValue - blue) / green_blue);

            if (temperatureValue < yellow)
                return Color.Lerp(Color.green, Color.yellow, (temperatureValue - green) / yellow_green);

            if (temperatureValue < red)
                return Color.Lerp(Color.yellow, Color.red, (temperatureValue - yellow) / red_yellow);

            return Color.red;
        }
    }
}
