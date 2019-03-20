namespace MoreCyclopsUpgrades.Managers
{
    using Common;
    using MoreCyclopsUpgrades.SaveData;
    using UnityEngine;
    using UnityEngine.UI;

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

            bool isCyclopsAlive = cyclopsHelmHUD.subLiveMixin.IsAlive();
            if (isCyclopsAlive)
            {
                // Update Health
                float healthFraction = cyclopsHelmHUD.subLiveMixin.GetHealthFraction();
                cyclopsHelmHUD.hpBar.fillAmount = Mathf.Lerp(cyclopsHelmHUD.hpBar.fillAmount, healthFraction, Time.deltaTime * 2f);

                // Update Noise
                float noisePercent = cyclopsHelmHUD.noiseManager.GetNoisePercent();
                cyclopsHelmHUD.noiseBar.fillAmount = Mathf.Lerp(cyclopsHelmHUD.noiseBar.fillAmount, noisePercent, Time.deltaTime);

                // Update Power (this one is different)
                int currentReservePower = this.PowerManager.GetTotalReservePower();
                cyclopsHelmHUD.powerText.color = currentReservePower > 0f ? Color.cyan : Color.white;

                float availablePower = currentReservePower + this.Cyclops.powerRelay.GetPower();
                float availablePowerRatio = availablePower / this.Cyclops.powerRelay.GetMaxPower();

                // Min'd with 999 since this textbox can only display 4 characeters
                int powerPercentage = Mathf.Min(999, Mathf.CeilToInt(availablePowerRatio * 100f));

                if (cyclopsHelmHUD.lastPowerPctUsedForString != powerPercentage)
                {
                    cyclopsHelmHUD.powerText.text = $"{powerPercentage}%";
                    cyclopsHelmHUD.lastPowerPctUsedForString = powerPercentage;
                }

                int currentDepth = (int)cyclopsHelmHUD.crushDamage.GetDepth();
                int currentMaxDepth = (int)cyclopsHelmHUD.crushDamage.crushDepth;

                if (cyclopsHelmHUD.lastDepthUsedForString != currentDepth || cyclopsHelmHUD.lastCrushDepthUsedForString != currentMaxDepth)
                {
                    cyclopsHelmHUD.lastDepthUsedForString = currentDepth;
                    cyclopsHelmHUD.lastCrushDepthUsedForString = currentMaxDepth;
                    cyclopsHelmHUD.depthText.text = string.Format("{0}m / {1}m", currentDepth, currentMaxDepth);
                }

                cyclopsHelmHUD.depthText.color = currentDepth >= currentMaxDepth ? Color.red : Color.white;
                cyclopsHelmHUD.engineOffText.gameObject.SetActive(!cyclopsHelmHUD.motorMode.engineOn);
                cyclopsHelmHUD.fireWarningSprite.gameObject.SetActive(cyclopsHelmHUD.fireWarning);
                cyclopsHelmHUD.creatureAttackSprite.gameObject.SetActive(cyclopsHelmHUD.creatureAttackWarning);
                cyclopsHelmHUD.hullDamageWarning = (cyclopsHelmHUD.subLiveMixin.GetHealthFraction() < 0.8f);
            }

            UpdateWarnings(cyclopsHelmHUD);
        }

        private static void UpdateWarnings(CyclopsHelmHUDManager cyclopsHelmHUD)
        {
            if (Player.main.currentSub == cyclopsHelmHUD.subRoot && !cyclopsHelmHUD.subRoot.subDestroyed)
            {
                if (cyclopsHelmHUD.fireWarning && cyclopsHelmHUD.creatureAttackWarning)
                {
                    cyclopsHelmHUD.subRoot.voiceNotificationManager.PlayVoiceNotification(cyclopsHelmHUD.subRoot.creatureAttackNotification, true, false);
                }
                else if (cyclopsHelmHUD.creatureAttackWarning)
                {
                    cyclopsHelmHUD.subRoot.voiceNotificationManager.PlayVoiceNotification(cyclopsHelmHUD.subRoot.creatureAttackNotification, cyclopsHelmHUD.subRoot, false);
                }
                else if (cyclopsHelmHUD.fireWarning)
                {
                    cyclopsHelmHUD.subRoot.voiceNotificationManager.PlayVoiceNotification(cyclopsHelmHUD.subRoot.fireNotification, true, false);
                }
                else if (cyclopsHelmHUD.noiseManager.GetNoisePercent() > 0.9f && !(cyclopsHelmHUD as MonoBehaviour).IsInvoking("PlayCavitationWarningAfterSeconds"))
                {
                    (cyclopsHelmHUD as MonoBehaviour).Invoke("PlayCavitationWarningAfterSeconds", 2f);
                }
                else if (cyclopsHelmHUD.hullDamageWarning)
                {
                    cyclopsHelmHUD.subRoot.voiceNotificationManager.PlayVoiceNotification(cyclopsHelmHUD.subRoot.hullDamageNotification, true, false);
                }

                cyclopsHelmHUD.subRoot.subWarning = cyclopsHelmHUD.fireWarning || cyclopsHelmHUD.creatureAttackWarning;

                cyclopsHelmHUD.warningAlpha = Mathf.PingPong(Time.time * 5f, 1f);
                cyclopsHelmHUD.fireWarningSprite.color = new Color(1f, 1f, 1f, cyclopsHelmHUD.warningAlpha);
                cyclopsHelmHUD.creatureAttackSprite.color = new Color(1f, 1f, 1f, cyclopsHelmHUD.warningAlpha);

                if (cyclopsHelmHUD.hudActive)
                {
                    cyclopsHelmHUD.canvasGroup.alpha = Mathf.Lerp(cyclopsHelmHUD.canvasGroup.alpha, 1f, Time.deltaTime * 3f);
                    cyclopsHelmHUD.canvasGroup.interactable = true;
                }
                else
                {
                    cyclopsHelmHUD.canvasGroup.alpha = Mathf.Lerp(cyclopsHelmHUD.canvasGroup.alpha, 0f, Time.deltaTime * 3f);
                    cyclopsHelmHUD.canvasGroup.interactable = false;
                }
            }
            else
            {
                cyclopsHelmHUD.subRoot.subWarning = false;
            }

            if (cyclopsHelmHUD.oldWarningState != cyclopsHelmHUD.subRoot.subWarning)
            {
                cyclopsHelmHUD.subRoot.BroadcastMessage("NewAlarmState", null, SendMessageOptions.DontRequireReceiver);
            }

            cyclopsHelmHUD.oldWarningState = cyclopsHelmHUD.subRoot.subWarning;
        }

        /// <summary>
        /// Updates the console HUD using data from all equipment modules across all upgrade consoles.
        /// </summary>
        /// <param name="hudManager">The console HUD manager.</param>
        internal void UpdateConsoleHUD(CyclopsUpgradeConsoleHUDManager hudManager)
        {
            int currentReservePower = this.PowerManager.GetTotalReservePower();

            float currentBatteryPower = this.Cyclops.powerRelay.GetPower();

            if (currentReservePower > 0)
            {
                hudManager.energyCur.color = Color.cyan; // Distinct color for when reserve power is available
            }
            else
            {
                hudManager.energyCur.color = Color.white; // Normal color
            }

            int TotalPowerUnits = Mathf.CeilToInt(currentBatteryPower + currentReservePower);

            hudManager.energyCur.text = IntStringCache.GetStringForInt(TotalPowerUnits);

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
            text.fontSize = 23;
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
                hpIcon.Text.text = helmIcon.Text.text = $"{Mathf.FloorToInt(icon.Value)}{icon.Format}";
            }
        }
    }
}
