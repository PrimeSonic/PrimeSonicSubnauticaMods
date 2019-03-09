namespace MoreCyclopsUpgrades.Managers
{
    using Common;
    using MoreCyclopsUpgrades.SaveData;
    using UnityEngine;

    internal class CyclopsHUDManager
    {
        public CyclopsManager Manager { get; private set; }

        internal readonly uGUI_Icon[] RowOfOdd = new uGUI_Icon[3];
        internal readonly uGUI_Icon[] RowOfEven = new uGUI_Icon[2];

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
                AddHelmHUDicons(cyclopsHelmHUD);
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

        private void AddHelmHUDicons(CyclopsHelmHUDManager cyclopsHelmHUD)
        {
            Canvas canvas = cyclopsHelmHUD.powerText.gameObject.GetComponentInParent<Canvas>();

            /* --- 3-1-2 --- */
            /* ---- 1-2 ---- */

            // Because the nuclear module only ever kicks in if there are no renewable sources of power
            // We can guarantee that we only ever need at most, 3 icons on diplay.
            const float unit = 120;
            RowOfOdd[0] = CreatePowerIcon(canvas, 0);
            RowOfOdd[1] = CreatePowerIcon(canvas, unit);
            RowOfOdd[2] = CreatePowerIcon(canvas, -unit);

            RowOfEven[0] = CreatePowerIcon(canvas, -unit / 2);
            RowOfEven[1] = CreatePowerIcon(canvas, unit / 2);

            powerIconsInitialized = true;

            QuickLogger.Debug("Linked PowerManager to HelmHUD");
        }

        private uGUI_Icon CreatePowerIcon(Canvas canvas, float xoffset)
        {
            const float zoffset = 0.05f;
            const float yoffset = -230;
            const float scale = 1.80f;

            var iconGo = new GameObject("IconGo", typeof(RectTransform));
            iconGo.transform.SetParent(canvas.transform, false);
            iconGo.transform.localPosition = new Vector3(xoffset, yoffset, zoffset);
            iconGo.transform.localScale = new Vector3(scale, scale, scale);
            return iconGo.AddComponent<uGUI_Icon>();
        }

        private void UpdatePowerIcons(PowerIconState powerIcons)
        {
            if (!powerIconsInitialized)
                return;

            RowOfOdd[0].enabled = false;
            RowOfOdd[1].enabled = false;
            RowOfOdd[2].enabled = false;

            RowOfEven[0].enabled = false;
            RowOfEven[1].enabled = false;

            uGUI_Icon[] row = powerIcons.EvenCount ? RowOfEven : RowOfOdd;
            int index = 0;

            foreach (TechType item in powerIcons.ActiveIcons)
            {
                if (index == row.Length)
                {
                    QuickLogger.Debug("Got an unexpected number of icons", true);
                    return;
                }

                uGUI_Icon uiIcon = row[index++];
                uiIcon.sprite = SpriteManager.Get(item);
                uiIcon.enabled = true;                
            }
        }
    }
}
