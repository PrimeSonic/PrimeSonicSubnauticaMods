namespace MoreCyclopsUpgrades.Managers
{
    using Common;
    using SaveData;
    using UnityEngine;
    using UnityEngine.UI;

    internal enum NumberFormat : byte
    {
        Temperature = (byte)'T',
        Amount = (byte)'A',
        Sun = (byte)'S',
        Percent = (byte)'P',
    }

    internal class CyclopsHUDManager
    {
        private class Indicator
        {
            internal uGUI_Icon Icon;
            internal Text Text;

            public Indicator(uGUI_Icon icon, Text text)
            {
                Icon = icon;
                Text = text;
            }

            internal bool Enabled
            {
                get => Icon.enabled || Text.enabled;
                set
                {
                    Icon.enabled = value;
                    Text.enabled = value;
                }
            }
        }

        public CyclopsManager Manager { get; private set; }

        private readonly Indicator[] HelmIndicatorsOdd = new Indicator[3];
        private readonly Indicator[] HelmIndicatorsEven = new Indicator[2];

        private readonly Indicator[] HealthBarIndicatorsOdd = new Indicator[3];
        private readonly Indicator[] HealthBarIndicatorsEven = new Indicator[2];

        public SubRoot Cyclops => this.Manager.Cyclops;
        public UpgradeManager UpgradeManager => this.Manager.UpgradeManager;
        public PowerManager PowerManager => this.Manager.PowerManager;

        private bool powerIconsInitialized = false;

        internal bool Initialize(CyclopsManager manager)
        {
            if (this.Manager != null)
                return false; // Already initialized

            this.Manager = manager;

            return true;
        }

        /// <summary>
        /// Updates the Cyclops helm HUD  using data from all equipment modules across all upgrade consoles.
        /// </summary>
        /// <param name="cyclopsHelmHUD">The instance.</param>
        /// <param name="lastReservePower">The last reserve power.</param>
        internal void UpdateHelmHUD(CyclopsHelmHUDManager cyclopsHelmHUD)
        {
            if (this.UpgradeManager == null)
            {
                ErrorMessage.AddMessage("UpdateHelmHUD: UpgradeManager is null");
                return;
            }

            if (!powerIconsInitialized)
            {
                AddPowerIcons(cyclopsHelmHUD);
            }

            // Change the color of the Cyclops energy percentage on the HUD
            int currentReservePower = this.PowerManager.GetTotalReservePower();
            cyclopsHelmHUD.powerText.color = currentReservePower > 0f ? Color.cyan : Color.white;
        }

        /// <summary>
        /// Updates the console HUD using data from all equipment modules across all upgrade consoles.
        /// </summary>
        /// <param name="hudManager">The console HUD manager.</param>
        internal void UpdateConsoleHUD(CyclopsUpgradeConsoleHUDManager hudManager)
        {
            hudManager.healthCur.text = IntStringCache.GetStringForInt(Mathf.FloorToInt(hudManager.liveMixin.health));
            int maxHealth = Mathf.CeilToInt(hudManager.liveMixin.health);
            if (hudManager.lastHealthMaxDisplayed != maxHealth)
            {
                hudManager.healthMax.text = "/" + IntStringCache.GetStringForInt(maxHealth);
                hudManager.lastHealthMaxDisplayed = maxHealth;
            }

            int currentReservePower = this.PowerManager.GetTotalReservePower();
            float currentBatteryPower = this.Cyclops.powerRelay.GetPower();
            int TotalPowerUnits = Mathf.CeilToInt(currentBatteryPower + currentReservePower);
            int normalMaxPower = Mathf.CeilToInt(this.Cyclops.powerRelay.GetMaxPower());

            hudManager.energyCur.color = currentReservePower > 0 ? Color.cyan : Color.white;
            hudManager.energyCur.text = FormatNumber(TotalPowerUnits, NumberFormat.Amount);

            if (hudManager.lastMaxSubPowerDisplayed != normalMaxPower)
            {
                hudManager.energyMax.text = "/" + FormatNumber(normalMaxPower, NumberFormat.Amount);
                hudManager.lastMaxSubPowerDisplayed = normalMaxPower;
            }

            NuclearModuleConfig.SetCyclopsMaxPower(this.Cyclops.powerRelay.GetMaxPower());

            UpdatePowerIcons(this.PowerManager.PowerIcons);
        }

        private void AddPowerIcons(CyclopsHelmHUDManager cyclopsHelmHUD)
        {
            Canvas canvas = cyclopsHelmHUD.powerText.gameObject.GetComponentInParent<Canvas>();

            /* --- 3-1-2 --- */
            /* ---- 1-2 ---- */

            // Because the nuclear module only ever kicks in if there are no renewable sources of power
            // We can guarantee that we only ever need at most, 3 icons on diplay.

            const float helmspacing = 135;
            const float helmzoffset = 0.05f;
            const float helmyoffset = -225;
            const float helmscale = 1.40f;
            HelmIndicatorsOdd[0] = CreatePowerIndicatorIcon(canvas, 0, helmyoffset, helmzoffset, helmscale);
            HelmIndicatorsOdd[1] = CreatePowerIndicatorIcon(canvas, helmspacing, helmyoffset, helmzoffset, helmscale);
            HelmIndicatorsOdd[2] = CreatePowerIndicatorIcon(canvas, -helmspacing, helmyoffset, helmzoffset, helmscale);
            HelmIndicatorsEven[0] = CreatePowerIndicatorIcon(canvas, -helmspacing / 2, helmyoffset, helmzoffset, helmscale);
            HelmIndicatorsEven[1] = CreatePowerIndicatorIcon(canvas, helmspacing / 2, helmyoffset, helmzoffset, helmscale);

            Canvas canvas2 = cyclopsHelmHUD.subRoot.GetComponentInChildren<CyclopsHolographicHUD>().healthBar.canvas;
            const float healthbarxoffset = 100;
            const float healthbarspacing = 70;
            const float healthbarzoffset = 0.05f;
            const float healthbaryoffset = -300;
            const float healthbarscale = 0.70f;

            HealthBarIndicatorsOdd[0] = CreatePowerIndicatorIcon(canvas2, healthbarxoffset + 0, healthbaryoffset, healthbarzoffset, healthbarscale);
            HealthBarIndicatorsOdd[1] = CreatePowerIndicatorIcon(canvas2, healthbarxoffset + healthbarspacing, healthbaryoffset, healthbarzoffset, healthbarscale);
            HealthBarIndicatorsOdd[2] = CreatePowerIndicatorIcon(canvas2, healthbarxoffset + -healthbarspacing, healthbaryoffset, healthbarzoffset, healthbarscale);
            HealthBarIndicatorsEven[0] = CreatePowerIndicatorIcon(canvas2, healthbarxoffset + -healthbarspacing / 2, healthbaryoffset, healthbarzoffset, healthbarscale);
            HealthBarIndicatorsEven[1] = CreatePowerIndicatorIcon(canvas2, healthbarxoffset + healthbarspacing / 2, healthbaryoffset, healthbarzoffset, healthbarscale);

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
            textGO.AddComponent<Text>();

            Text text = textGO.GetComponent<Text>();
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

            return new Indicator(icon, text);
        }

        private void UpdatePowerIcons(PowerIconState powerIcons)
        {
            if (!powerIconsInitialized)
                return;

            HelmIndicatorsOdd[0].Enabled = false;
            HelmIndicatorsOdd[1].Enabled = false;
            HelmIndicatorsOdd[2].Enabled = false;
            HelmIndicatorsEven[0].Enabled = false;
            HelmIndicatorsEven[1].Enabled = false;

            HealthBarIndicatorsOdd[0].Enabled = false;
            HealthBarIndicatorsOdd[1].Enabled = false;
            HealthBarIndicatorsOdd[2].Enabled = false;
            HealthBarIndicatorsEven[0].Enabled = false;
            HealthBarIndicatorsEven[1].Enabled = false;

            Indicator[] helmRow = powerIcons.EvenCount ? HelmIndicatorsEven : HelmIndicatorsOdd;
            Indicator[] healthBarRow = powerIcons.EvenCount ? HealthBarIndicatorsEven : HealthBarIndicatorsOdd;
            int index = 0;

            foreach (PowerIconState.PowerIcon icon in powerIcons.ActiveIcons)
            {
                if (index == helmRow.Length)
                {
                    QuickLogger.Debug("Got an unexpected number of icons", true);
                    return;
                }

                Indicator helmIcon = helmRow[index];
                Indicator hpIcon = healthBarRow[index++];

                hpIcon.Icon.sprite = helmIcon.Icon.sprite = SpriteManager.Get(icon.TechType);
                hpIcon.Enabled = helmIcon.Enabled = true;
                hpIcon.Text.text = helmIcon.Text.text = FormatNumber(icon.Value, icon.Format);
                hpIcon.Text.color = helmIcon.Text.color = GetNumberColor(icon.Value, icon.MaxValue, icon.MinValue);
            }
        }

        private static string FormatNumber(float value, NumberFormat format)
        {            
            switch (format)
            {
                case NumberFormat.Temperature:
                    return $"{Mathf.CeilToInt(value)}°C";
                case NumberFormat.Sun:
                    return $"{Mathf.CeilToInt(value)}°Θ";
                case NumberFormat.Amount:
                    return $"{HandleLargeNumbers(value)}";
                case NumberFormat.Percent:
                    return $"{Mathf.CeilToInt(value)}%";
                default:
                    return Mathf.FloorToInt(value).ToString();
            }
        }

        private static string HandleLargeNumbers(float possiblyLargeValue)
        {
            if (possiblyLargeValue > 999999)
            {
                return $"{possiblyLargeValue / 1000000:F2}M";
            }

            if (possiblyLargeValue > 999)
            {
                return $"{possiblyLargeValue / 1000:F2}K";
            }

            return $"{Mathf.CeilToInt(possiblyLargeValue)}";
        }

        private static Color GetNumberColor(float value, float max, float min)
        {
            if (value > max)
                return Color.white;

            if (value <= min)
                return Color.red;

            const float greenHue = 120f / 360f;
            float percentOfMax = (value - min) / (max - min);

            const float saturation = 1f;
            const float lightness = 0.8f;

            return Color.HSVToRGB(percentOfMax * greenHue, saturation, lightness);
        }
    }
}
