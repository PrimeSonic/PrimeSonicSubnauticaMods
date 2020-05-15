namespace MoreCyclopsUpgrades.Managers
{
    using System;
    using Common;
    using MoreCyclopsUpgrades.API.Charging;
    using MoreCyclopsUpgrades.Config;
    using MoreCyclopsUpgrades.Config.ChoiceEnums;
    using UnityEngine;
    using UnityEngine.UI;

    internal class CyclopsHUDManager
    {
        private PowerIndicatorIcon[] HelmIndicatorsOdd;
        private PowerIndicatorIcon[] HelmIndicatorsEven;
        private PowerIndicatorIcon[] HealthBarIndicatorsOdd;
        private PowerIndicatorIcon[] HealthBarIndicatorsEven;

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
        private readonly int totalPowerInfoIcons;

        internal CyclopsHUDManager(SubRoot cyclops, int totalIcons)
        {
            Cyclops = cyclops;
            totalPowerInfoIcons = Math.Max(totalIcons, 1); // Include a minimum of 1 for the vanilla thermal charger
        }

        internal void FastUpdate(CyclopsHelmHUDManager cyclopsHelmHUD)
        {
            if (!powerIconsInitialized)
                AddPowerIcons(cyclopsHelmHUD);
            else
                UpdatePowerIcons();

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

            settings.UpdateCyclopsMaxPower(normalMaxPower);

            powerIconTextVisibility =
                    Player.main.currentSub == Cyclops &&
                    holographicHUD != null &&
                    Mathf.Abs(Vector3.Distance(holographicHUD.transform.position, Player.main.transform.position)) <= 4f;

            if (lastKnownTextVisibility != powerIconTextVisibility)
            {
                UpdatePowerIcons();
                lastKnownTextVisibility = powerIconTextVisibility;
            }
        }

        private void AddPowerIcons(CyclopsHelmHUDManager cyclopsHelmHUD)
        {
            cyclopsHelmHUD.powerText.resizeTextForBestFit = true;

            QuickLogger.Debug($"CyclopsHUDManager Adding Power Info Icons for '{totalPowerInfoIcons}' CyclopsChargers");
            holographicHUD = cyclopsHelmHUD.subRoot.GetComponentInChildren<CyclopsHolographicHUD>();

            Canvas pilotingCanvas = cyclopsHelmHUD.powerText.canvas;
            Canvas holoCanvas = holographicHUD.healthBar.canvas;

            const float spacingUnit = 35;
            const float helmzoffset = 0.05f;
            const float helmyoffset = -225;
            const float helmscale = 1.35f;

            const float healthbarxoffset = 120f;
            const float healthbarzoffset = 0.05f;
            const float healthbaryoffset = -300f;
            const float healthbarscale = 0.65f;

            /* --- 3-1-2 --- */
            /* ---- 1-2 ---- */

            if (totalPowerInfoIcons == 1)
            {
                HelmIndicatorsOdd = new PowerIndicatorIcon[1];
                HelmIndicatorsEven = new PowerIndicatorIcon[0];
                HealthBarIndicatorsOdd = new PowerIndicatorIcon[1];
                HealthBarIndicatorsEven = new PowerIndicatorIcon[0];

                HelmIndicatorsOdd[0] = IconCreator.CreatePowerIndicatorIcon(pilotingCanvas, 0, helmyoffset, helmzoffset, helmscale);
                HealthBarIndicatorsOdd[0] = IconCreator.CreatePowerIndicatorIcon(holoCanvas, healthbarxoffset + 0, healthbaryoffset, healthbarzoffset, healthbarscale);
            }
            else
            {
                int totalIcons = totalPowerInfoIcons;

                if (totalIcons % 2 != 0)
                    totalIcons--;

                HelmIndicatorsOdd = new PowerIndicatorIcon[totalIcons + 1];
                HelmIndicatorsEven = new PowerIndicatorIcon[totalIcons];
                HealthBarIndicatorsOdd = new PowerIndicatorIcon[totalIcons + 1];
                HealthBarIndicatorsEven = new PowerIndicatorIcon[totalIcons];

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

            powerIconsInitialized = true;

            QuickLogger.Debug("Linked CyclopsHUDManager to HelmHUD");
        }

        private void UpdatePowerIcons()
        {
            if (!powerIconsInitialized)
                return;

            if (Time.time < iconUpdateDelay)
                return;

            iconUpdateDelay = Time.time + delayInterval;

            HidePowerIcons();

            if (settings.HidePowerIcons)
                return;

            CyclopsCharger[] cyclopsChargers = this.ChargeManager.Chargers;

            bool isEven = true;
            for (int i = 0; i < cyclopsChargers.Length; i++)
            {
                if (cyclopsChargers[i].ShowStatusIcon)
                    isEven = !isEven;
            }

            PowerIndicatorIcon[] helmRow = isEven ? HelmIndicatorsEven : HelmIndicatorsOdd;
            PowerIndicatorIcon[] healthBarRow = isEven ? HealthBarIndicatorsEven : HealthBarIndicatorsOdd;

            bool showIconsOnHoloDisplay = settings.ShowIconsOnHoloDisplay;
            bool showIconsWhilePiloting = settings.ShowIconsWhilePiloting;

            int iconIndex = 0;
            for (int c = 0; c < cyclopsChargers.Length; c++)
            {
                CyclopsCharger charger = cyclopsChargers[c];

                if (!charger.ShowStatusIcon)
                    continue;

                PowerIndicatorIcon helmIcon = helmRow[iconIndex];
                PowerIndicatorIcon hpIcon = healthBarRow[iconIndex++];

                hpIcon.SetEnabled(showIconsOnHoloDisplay);
                helmIcon.SetEnabled(showIconsWhilePiloting);

                hpIcon.Icon.sprite = helmIcon.Icon.sprite = charger.StatusSprite();

                hpIcon.Text.enabled = powerIconTextVisibility;
                hpIcon.Text.text = helmIcon.Text.text = charger.StatusText();

                if (charger.ProvidingPower)
                    hpIcon.Text.color = helmIcon.Text.color = charger.StatusTextColor();
                else
                    hpIcon.Text.color = helmIcon.Text.color = Color.white;
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
    }
}
