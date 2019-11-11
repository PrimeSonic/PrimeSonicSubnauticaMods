namespace MoreCyclopsUpgrades.Managers
{
    using Common;
    using MoreCyclopsUpgrades.API.Charging;
    using MoreCyclopsUpgrades.Config;
    using MoreCyclopsUpgrades.Config.ChoiceEnums;
    using UnityEngine;
    using UnityEngine.UI;

    internal class CyclopsHUDManager
    {
        private class Indicator
        {
            internal uGUI_Icon Icon;
            internal Text Text;

            internal Indicator(uGUI_Icon icon, Text text)
            {
                Icon = icon;
                Text = text;
                SetEnabled(false);
            }

            internal void SetEnabled(bool value)
            {
                Icon.enabled = value;
                Text.enabled = value;
            }
        }

        private Indicator[] HelmIndicatorsOdd;
        private Indicator[] HelmIndicatorsEven;
        private Indicator[] HealthBarIndicatorsOdd;
        private Indicator[] HealthBarIndicatorsEven;

        private bool lastKnownTextVisibility = false;
        private bool powerIconTextVisibility = false;

        private const float delayInterval = 0.9876f;
        private float iconUpdateDelay = Time.deltaTime;

        private readonly SubRoot Cyclops;

        private ChargeManager chargeManager;
        private ChargeManager ChargeManager => chargeManager ?? (chargeManager = CyclopsManager.GetManager(Cyclops).Charge);

        private bool powerIconsInitialized = false;

        private CyclopsHolographicHUD holographicHUD;
        private readonly IModConfig settings = ModConfig.Main;
        private readonly int totalPowerInfoIcons;

        private HelmEnergyDisplay lastDisplay = HelmEnergyDisplay.PowerCellPercentage;

        internal CyclopsHUDManager(SubRoot cyclops, int totalIcons)
        {
            Cyclops = cyclops;
            totalPowerInfoIcons = totalIcons;
        }

        internal void FastUpdate(CyclopsHelmHUDManager cyclopsHelmHUD, int lastPowerInt)
        {
            if (!cyclopsHelmHUD.LOD.IsFull() || Player.main.currentSub != Cyclops)
                return; // Same early exit

            if (totalPowerInfoIcons > 0)
            {
                if (!powerIconsInitialized)
                    AddPowerIcons(cyclopsHelmHUD);
                else
                    UpdatePowerIcons();
            }

            if (lastPowerInt < 0f)
                return;

            PowerRelay powerRelay = Cyclops.powerRelay;

            switch (lastDisplay = settings.EnergyDisplay)
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
            if (!Cyclops.LOD.IsFull() || Player.main.currentSub != Cyclops || !Cyclops.live.IsAlive())
            {
                return; // Same early exit
            }

            hudManager.healthCur.text = IntStringCache.GetStringForInt(Mathf.FloorToInt(hudManager.liveMixin.health));
            int maxHealth = Mathf.CeilToInt(hudManager.liveMixin.health);
            if (hudManager.lastHealthMaxDisplayed != maxHealth)
            {
                hudManager.healthMax.text = "/" + IntStringCache.GetStringForInt(maxHealth);
                hudManager.lastHealthMaxDisplayed = maxHealth;
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

            if (totalPowerInfoIcons > 0)
            {
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
        }

        private void AddPowerIcons(CyclopsHelmHUDManager cyclopsHelmHUD)
        {
            cyclopsHelmHUD.powerText.resizeTextForBestFit = true;

            if (totalPowerInfoIcons == 0)
            {
                QuickLogger.Debug($"CyclopsHUDManager 0 Power Info Icons required");
                return;
            }

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
                HelmIndicatorsOdd = new Indicator[1];
                HelmIndicatorsEven = new Indicator[0];
                HealthBarIndicatorsOdd = new Indicator[1];
                HealthBarIndicatorsEven = new Indicator[0];

                HelmIndicatorsOdd[0] = CreatePowerIndicatorIcon(pilotingCanvas, 0, helmyoffset, helmzoffset, helmscale);
                HealthBarIndicatorsOdd[0] = CreatePowerIndicatorIcon(holoCanvas, healthbarxoffset + 0, healthbaryoffset, healthbarzoffset, healthbarscale);
            }
            else
            {
                int totalIcons = totalPowerInfoIcons;

                if (totalIcons % 2 != 0)
                    totalIcons--;

                HelmIndicatorsOdd = new Indicator[totalIcons + 1];
                HelmIndicatorsEven = new Indicator[totalIcons];
                HealthBarIndicatorsOdd = new Indicator[totalIcons + 1];
                HealthBarIndicatorsEven = new Indicator[totalIcons];

                HelmIndicatorsOdd[0] = CreatePowerIndicatorIcon(pilotingCanvas, 0, helmyoffset, helmzoffset, helmscale);
                HealthBarIndicatorsOdd[0] = CreatePowerIndicatorIcon(holoCanvas, healthbarxoffset + 0, healthbaryoffset, healthbarzoffset, healthbarscale);

                int index = 0;
                float healtBarSpacing = 0f;
                float helmSpacing = 0f;
                do
                {
                    healtBarSpacing += spacingUnit;
                    helmSpacing += spacingUnit * 2f;

                    // Add even icons first
                    HelmIndicatorsEven[index + 0] = CreatePowerIndicatorIcon(pilotingCanvas, -helmSpacing, helmyoffset, helmzoffset, helmscale);
                    HelmIndicatorsEven[index + 1] = CreatePowerIndicatorIcon(pilotingCanvas, helmSpacing, helmyoffset, helmzoffset, helmscale);

                    HealthBarIndicatorsEven[index + 0] = CreatePowerIndicatorIcon(holoCanvas, healthbarxoffset + -healtBarSpacing, healthbaryoffset, healthbarzoffset, healthbarscale);
                    HealthBarIndicatorsEven[index + 1] = CreatePowerIndicatorIcon(holoCanvas, healthbarxoffset + healtBarSpacing, healthbaryoffset, healthbarzoffset, healthbarscale);

                    healtBarSpacing += spacingUnit;
                    helmSpacing += spacingUnit * 2f;

                    // Add odd icons next
                    HelmIndicatorsOdd[index + 1] = CreatePowerIndicatorIcon(pilotingCanvas, -helmSpacing, helmyoffset, helmzoffset, helmscale);
                    HelmIndicatorsOdd[index + 2] = CreatePowerIndicatorIcon(pilotingCanvas, helmSpacing, helmyoffset, helmzoffset, helmscale);

                    HealthBarIndicatorsOdd[index + 1] = CreatePowerIndicatorIcon(holoCanvas, healthbarxoffset + -healtBarSpacing, healthbaryoffset, healthbarzoffset, healthbarscale);
                    HealthBarIndicatorsOdd[index + 2] = CreatePowerIndicatorIcon(holoCanvas, healthbarxoffset + healtBarSpacing, healthbaryoffset, healthbarzoffset, healthbarscale);

                    index += 2;

                } while (totalIcons > index);
            }

            powerIconsInitialized = true;

            QuickLogger.Debug("Linked CyclopsHUDManager to HelmHUD");
        }

        private static Indicator CreatePowerIndicatorIcon(Canvas canvas, float xoffset, float yoffset, float zoffset, float scale)
        {
            var iconGo = new GameObject("IconGo", typeof(RectTransform));
            iconGo.transform.SetParent(canvas.transform, false);
            iconGo.transform.localPosition = new Vector3(xoffset, yoffset, zoffset);
            iconGo.transform.localScale = new Vector3(scale, scale, scale);

            uGUI_Icon icon = iconGo.AddComponent<uGUI_Icon>();

            var arial = (Font)Resources.GetBuiltinResource(typeof(Font), "Arial.ttf");
            var textGO = new GameObject("TextGo");

            textGO.transform.SetParent(iconGo.transform, false);

            Text text = textGO.AddComponent<Text>();
            text.font = arial;
            text.material = arial.material;
            text.text = "??";
            text.fontSize = 22;
            text.alignment = TextAnchor.LowerCenter;
            text.color = Color.white;

            Outline outline = textGO.AddComponent<Outline>();
            outline.effectColor = Color.black;

            RectTransform rectTransform = text.GetComponent<RectTransform>();
            rectTransform.localScale = Vector3.one;
            rectTransform.anchoredPosition3D = Vector3.zero;
            rectTransform.anchoredPosition += new Vector2(0f, -15f);
            return new Indicator(icon, text);
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

            Indicator[] helmRow = isEven ? HelmIndicatorsEven : HelmIndicatorsOdd;
            Indicator[] healthBarRow = isEven ? HealthBarIndicatorsEven : HealthBarIndicatorsOdd;

            bool showIconsOnHoloDisplay = settings.ShowIconsOnHoloDisplay;
            bool showIconsWhilePiloting = settings.ShowIconsWhilePiloting;

            int iconIndex = 0;
            for (int c = 0; c < cyclopsChargers.Length; c++)
            {
                CyclopsCharger charger = cyclopsChargers[c];

                if (!charger.ShowStatusIcon)
                    continue;

                Indicator helmIcon = helmRow[iconIndex];
                Indicator hpIcon = healthBarRow[iconIndex++];

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
