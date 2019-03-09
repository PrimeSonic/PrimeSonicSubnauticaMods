namespace MoreCyclopsUpgrades
{
    using Caching;
    using Common;
    using Modules;
    using MoreCyclopsUpgrades.SaveData;
    using UnityEngine;

    internal class CyclopsHUDManager
    {
        public CyclopsManager Manager { get; private set; }

        public uGUI_Icon PowerIconLeftThermal { get; private set; }
        public uGUI_Icon PowerIconCenter { get; private set; }
        public uGUI_Icon PowerIconRightSolar { get; private set; }
        private bool chargingIconsSet = false;

        public SubRoot Cyclops => this.Manager.Cyclops;
        public UpgradeManager UpgradeManager => this.Manager.UpgradeManager;
        public PowerManager PowerManager => this.Manager.PowerManager;

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

            if (!chargingIconsSet)
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

                if (this.PowerManager.chargingFromNuclear)
                {
                    this.PowerIconLeftThermal.enabled = false;
                    this.PowerIconRightSolar.enabled = false;

                    this.PowerIconCenter.sprite = SpriteManager.Get(CyclopsModule.NuclearChargerID);
                    this.PowerIconCenter.enabled = true;
                }
                else
                {
                    this.PowerIconLeftThermal.enabled = this.PowerManager.chargingFromThermal;
                    this.PowerIconRightSolar.enabled = this.PowerManager.chargingFromSolar;

                    this.PowerIconCenter.sprite = SpriteManager.Get(TechType.BaseBioReactor);
                    this.PowerIconCenter.enabled = this.PowerManager.chargingFromBio;
                }

                int currentDepth = (int)cyclopsHelmHUD.crushDamage.GetDepth();
                int currentMaxDepth = (int)cyclopsHelmHUD.crushDamage.crushDepth;

                Color color = currentDepth >= currentMaxDepth ? Color.red : Color.white;

                if (cyclopsHelmHUD.lastDepthUsedForString != currentDepth || cyclopsHelmHUD.lastCrushDepthUsedForString != currentMaxDepth)
                {
                    cyclopsHelmHUD.lastDepthUsedForString = currentDepth;
                    cyclopsHelmHUD.lastCrushDepthUsedForString = currentMaxDepth;
                    cyclopsHelmHUD.depthText.text = string.Format("{0}m / {1}m", currentDepth, currentMaxDepth);
                }

                cyclopsHelmHUD.depthText.color = color;
                cyclopsHelmHUD.engineOffText.gameObject.SetActive(!cyclopsHelmHUD.motorMode.engineOn);
                cyclopsHelmHUD.fireWarningSprite.gameObject.SetActive(cyclopsHelmHUD.fireWarning);
                cyclopsHelmHUD.creatureAttackSprite.gameObject.SetActive(cyclopsHelmHUD.creatureAttackWarning);
                cyclopsHelmHUD.hullDamageWarning = (cyclopsHelmHUD.subLiveMixin.GetHealthFraction() < 0.8f);
            }

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
        }

        private void AddHelmHUDicons(CyclopsHelmHUDManager cyclopsHelmHUD)
        {
            Canvas canvas = cyclopsHelmHUD.powerText.gameObject.GetComponentInParent<Canvas>();

            const float unit = 100;
            const float scale = 1.75f;
            const float zoffset = 0.05f;
            const float yoffset = -250;

            var leftGo = new GameObject("LeftIcon", typeof(RectTransform));
            leftGo.transform.SetParent(canvas.transform, false);
            leftGo.transform.localPosition = new Vector3(unit, yoffset, zoffset);
            leftGo.transform.localScale = new Vector3(scale, scale, scale);
            this.PowerIconLeftThermal = leftGo.AddComponent<uGUI_Icon>();
            this.PowerIconLeftThermal.sprite = SpriteManager.Get(TechType.CyclopsThermalReactorModule);

            var centerGo = new GameObject("CenterIcon", typeof(RectTransform));
            centerGo.transform.SetParent(canvas.transform, false);
            centerGo.transform.localPosition = new Vector3(0, yoffset, zoffset);
            centerGo.transform.localScale = new Vector3(scale, scale, scale);
            this.PowerIconCenter = centerGo.AddComponent<uGUI_Icon>();

            var rightGo = new GameObject("RightIcon", typeof(RectTransform));
            rightGo.transform.SetParent(canvas.transform, false);
            rightGo.transform.localPosition = new Vector3(-unit, yoffset, zoffset);
            rightGo.transform.localScale = new Vector3(scale, scale, scale);
            this.PowerIconRightSolar = rightGo.AddComponent<uGUI_Icon>();
            this.PowerIconRightSolar.sprite = SpriteManager.Get(CyclopsModule.SolarChargerID);

            chargingIconsSet = true;
            QuickLogger.Debug("Linked PowerManager to HelmHUD");
        }
    }
}
