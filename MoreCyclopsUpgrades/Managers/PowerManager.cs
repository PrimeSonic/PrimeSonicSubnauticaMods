namespace MoreCyclopsUpgrades.Managers
{
    using Common;
    using CyclopsUpgrades;
    using CyclopsUpgrades.CyclopsCharging;
    using Modules.Enhancement;
    using MoreCyclopsUpgrades.Modules;
    using MoreCyclopsUpgrades.SaveData;
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// Handles recharging the Cyclops and other power related tasks.
    /// </summary>
    public class PowerManager
    {
        private static readonly ICollection<ChargerCreator> OneTimeUseCyclopsChargers = new List<ChargerCreator>();
        private static readonly ICollection<ChargerCreator> ReusableCyclopsChargers = new List<ChargerCreator>();

        /// <summary>
        /// <para>This event happens right before the PowerManager starts initializing a the registered <see cref="ICyclopsCharger"/>s.</para>
        /// <para>Use this if you need a way to know when you should call <see cref="RegisterOneTimeUseChargerCreator"/> for <see cref="ChargerCreator"/>s that cannot be created from a static context.</para>
        /// </summary>
        public static Action CyclopsChargersInitializing;

        /// <summary>
        /// Registers a <see cref="ChargerCreator"/> method that creates returns a new <see cref="ICyclopsCharger"/> on demand and is only used once.
        /// </summary>
        /// <param name="createEvent">A method that takes no parameters a returns a new instance of an <see cref="ChargerCreator"/>.</param>
        public static void RegisterOneTimeUseChargerCreator(ChargerCreator createEvent)
        {
            OneTimeUseCyclopsChargers.Add(createEvent);
        }

        /// <summary>
        /// Registers a <see cref="ChargerCreator"/> method that creates returns a new <see cref="ICyclopsCharger"/> on demand that can is reused for each new Cyclops.
        /// </summary>
        /// <param name="createEvent">A method that takes no parameters a returns a new instance of an <see cref="ICyclopsCharger"/>.</param>
        public static void RegisterReusableChargerCreator(ChargerCreator createEvent)
        {
            ReusableCyclopsChargers.Add(createEvent);
        }

        internal const float BatteryDrainRate = 0.01f;
        internal const float Mk2ChargeRateModifier = 1.15f; // The MK2 charging modules get a 15% bonus to their charge rate.

        private const float EnginePowerPenalty = 0.7f;

        internal const int MaxSpeedBoosters = 6;
        private const int PowerIndexCount = 4;

        /// <summary>
        /// "Practically zero" for all intents and purposes. Any energy value lower than this should be considered zero.
        /// </summary>
        public const float MinimalPowerValue = 0.001f;

        private static readonly float[] SlowSpeedBonuses = new float[MaxSpeedBoosters]
        {
            0.25f, 0.15f, 0.10f, 0.10f, 0.05f, 0.05f // Diminishing returns on speed modules
            // Max +70%
        };

        private static readonly float[] StandardSpeedBonuses = new float[MaxSpeedBoosters]
        {
            0.40f, 0.30f, 0.20f, 0.15f, 0.10f, 0.05f // Diminishing returns on speed modules
            // Max +120%
        };

        private static readonly float[] FlankSpeedBonuses = new float[MaxSpeedBoosters]
        {
            0.45f, 0.20f, 0.10f, 0.10f, 0.05f, 0.05f // Diminishing returns on speed modules
            // Max +95%
        };

        private static readonly float[] EnginePowerRatings = new float[PowerIndexCount]
        {
            1f, 3f, 5f, 6f
        };

        private static readonly float[] SilentRunningPowerCosts = new float[PowerIndexCount]
        {
            5f, 5f, 4f, 3f // Lower costs here don't show up until the Mk2
        };

        private static readonly float[] SonarPowerCosts = new float[PowerIndexCount]
        {
            10f, 10f, 8f, 7f // Lower costs here don't show up until the Mk2
        };

        private static readonly float[] ShieldPowerCosts = new float[PowerIndexCount]
        {
            50f, 50f, 42f, 34f // Lower costs here don't show up until the Mk2
        };

        internal readonly ICollection<ICyclopsCharger> PowerChargers = new List<ICyclopsCharger>();

        internal UpgradeHandler SpeedBoosters;
        internal TieredUpgradesHandlerCollection<int> EngineEfficientyUpgrades;

        internal CyclopsManager Manager;
        internal readonly SubRoot Cyclops;

        internal readonly int MaxSpeedModules = ModConfig.Settings.MaxSpeedModules();

        private CyclopsMotorMode motorMode;
        private CyclopsMotorMode MotorMode => motorMode ?? (motorMode = Cyclops.GetComponentInChildren<CyclopsMotorMode>());

        private SubControl subControl;
        private SubControl SubControl => subControl ?? (subControl = Cyclops.GetComponentInChildren<SubControl>());

        private float lastKnownPowerRating = -1f;
        private int lastKnownSpeedBoosters = -1;
        private int lastKnownPowerIndex = -1;
        private int rechargeSkip = 10;
        private readonly int extraSkips = ModConfig.Settings.RechargeSkipRate();
        private readonly float rechargePenalty = ModConfig.Settings.RechargePenalty();

        internal PowerManager(SubRoot cyclops)
        {
            Cyclops = cyclops;
            UpgradeManager.UpgradeManagerInitializing += SetupPowerManagerUpgrades;
        }

        private float[] OriginalSpeeds { get; } = new float[3];

        internal bool Initialize(CyclopsManager manager)
        {
            if (Manager != null)
                return false; // Already initialized

            Manager = manager;

            InitializeChargingHandlers();

            // Store the original values before we start to change them
            this.OriginalSpeeds[0] = this.MotorMode.motorModeSpeeds[0];
            this.OriginalSpeeds[1] = this.MotorMode.motorModeSpeeds[1];
            this.OriginalSpeeds[2] = this.MotorMode.motorModeSpeeds[2];

            manager.ChargeManager.SyncBioReactors();

            return true;
        }

        internal void InitializeChargingHandlers()
        {
            CyclopsChargersInitializing?.Invoke();

            foreach (ChargerCreator method in ReusableCyclopsChargers)
                PowerChargers.Add(method.Invoke(Cyclops));

            foreach (ChargerCreator method in OneTimeUseCyclopsChargers)
                PowerChargers.Add(method.Invoke(Cyclops));

            OneTimeUseCyclopsChargers.Clear();
        }

        /// <summary>
        /// Updates the Cyclops power and speed rating.
        /// Power Rating manages engine efficiency as well as the power cost of using Silent Running, Sonar, and Defense Shield.
        /// Speed rating manages bonus speed across all motor modes.
        /// </summary>
        internal void UpdatePowerSpeedRating()
        {
            int powerIndex = EngineEfficientyUpgrades.HighestValue;
            int speedBoosters = SpeedBoosters.Count;

            if (lastKnownPowerIndex != powerIndex)
            {
                lastKnownPowerIndex = powerIndex;

                Cyclops.silentRunningPowerCost = SilentRunningPowerCosts[powerIndex];
                Cyclops.sonarPowerCost = SonarPowerCosts[powerIndex];
                Cyclops.shieldPowerCost = ShieldPowerCosts[powerIndex];
            }

            // Speed modules can affect power rating too
            float efficiencyBonus = EnginePowerRatings[powerIndex];

            for (int i = 0; i < speedBoosters; i++)
            {
                efficiencyBonus *= EnginePowerPenalty;
            }

            int cleanRating = Mathf.CeilToInt(100f * efficiencyBonus);

            while (cleanRating % 5 != 0)
                cleanRating--;

            float powerRating = cleanRating / 100f;

            if (lastKnownPowerRating != powerRating)
            {
                lastKnownPowerRating = powerRating;

                Cyclops.currPowerRating = powerRating;

                // Inform the new power rating just like the original method would.
                ErrorMessage.AddMessage(Language.main.GetFormat("PowerRatingNowFormat", powerRating));
            }

            if (speedBoosters > MaxSpeedModules)
                return; // Exit here

            if (lastKnownSpeedBoosters != speedBoosters)
            {
                lastKnownSpeedBoosters = speedBoosters;

                float SlowMultiplier = 1f;
                float StandardMultiplier = 1f;
                float FlankMultiplier = 1f;

                // Calculate the speed multiplier with diminishing returns
                while (--speedBoosters > -1)
                {
                    SlowMultiplier += SlowSpeedBonuses[speedBoosters];
                    StandardMultiplier += StandardSpeedBonuses[speedBoosters];
                    FlankMultiplier += FlankSpeedBonuses[speedBoosters];
                }

                // These will apply when changing speed modes
                this.MotorMode.motorModeSpeeds[0] = this.OriginalSpeeds[0] * SlowMultiplier;
                this.MotorMode.motorModeSpeeds[1] = this.OriginalSpeeds[1] * StandardMultiplier;
                this.MotorMode.motorModeSpeeds[2] = this.OriginalSpeeds[2] * FlankMultiplier;

                // These will apply immediately
                CyclopsMotorMode.CyclopsMotorModes currentMode = this.MotorMode.cyclopsMotorMode;
                this.SubControl.BaseForwardAccel = this.MotorMode.motorModeSpeeds[(int)currentMode];

                ErrorMessage.AddMessage(CyclopsSpeedBooster.SpeedRatingText(lastKnownSpeedBoosters, Mathf.RoundToInt(StandardMultiplier * 100)));
            }
        }

        /// <summary>
        /// Recharges the cyclops' power cells using all charging modules across all upgrade consoles.
        /// </summary>
        internal void RechargeCyclops()
        {
            if (Time.timeScale == 0f) // Is the game paused?
                return;

            if (rechargeSkip < lastKnownSpeedBoosters + extraSkips)
            {
                rechargeSkip++; // Slightly slows down recharging with more speed boosters and higher difficulty
                return;
            }

            rechargeSkip = 0;

            // When in Creative mode or using the NoPower cheat, inform the chargers that there is no power deficit.
            // This is so that each charge can decide what to do individually rather than skip the entire charging cycle all together.
            float powerDeficit = GameModeUtils.RequiresPower()
                                 ? Cyclops.powerRelay.GetMaxPower() - Cyclops.powerRelay.GetPower()
                                 : 0f;

            Manager.HUDManager.UpdateTextVisibility();

            float power = 0f;
            foreach (ICyclopsCharger charger in PowerChargers)
                power += charger.ProducePower(powerDeficit);

            ChargeCyclops(power, ref powerDeficit);
        }

        private void ChargeCyclops(float availablePower, ref float powerDeficit)
        {
            if (powerDeficit < MinimalPowerValue)
                return; // No need to charge

            if (availablePower < MinimalPowerValue)
                return; // No power available

            availablePower *= rechargePenalty;

            Cyclops.powerRelay.AddEnergy(availablePower, out float amtStored);
            powerDeficit = Mathf.Max(0f, powerDeficit - availablePower);
        }

        private void SetupPowerManagerUpgrades()
        {
            int maxModules = ModConfig.Settings.MaxChargingModules();

            UpgradeManager.RegisterOneTimeUseHandlerCreator(() =>
            {
                QuickLogger.Debug("UpgradeHandler Registered: Engine Upgrades Collection");
                var efficiencyUpgrades = new TieredUpgradesHandlerCollection<int>(0);

                QuickLogger.Debug("UpgradeHandler Registered: Engine Upgrade Mk1");
                TieredUpgradeHandler<int> engine1 = efficiencyUpgrades.CreateTier(TechType.PowerUpgradeModule, 1);

                QuickLogger.Debug("UpgradeHandler Registered: Engine Upgrade Mk2");
                TieredUpgradeHandler<int> engine2 = efficiencyUpgrades.CreateTier(CyclopsModule.PowerUpgradeMk2ID, 2);

                QuickLogger.Debug("UpgradeHandler Registered: Engine Upgrade Mk3");
                TieredUpgradeHandler<int> engine3 = efficiencyUpgrades.CreateTier(CyclopsModule.PowerUpgradeMk3ID, 3);

                EngineEfficientyUpgrades = efficiencyUpgrades;
                return efficiencyUpgrades;
            });

            UpgradeManager.RegisterOneTimeUseHandlerCreator(() =>
            {
                QuickLogger.Debug("UpgradeHandler Registered: SpeedBooster Upgrade");
                var speed = new UpgradeHandler(CyclopsModule.SpeedBoosterModuleID)
                {
                    MaxCount = maxModules,
                    OnFirstTimeMaxCountReached = () =>
                    {
                        ErrorMessage.AddMessage(CyclopsSpeedBooster.MaxRatingAchived);
                    }
                };
                SpeedBoosters = speed;
                return speed;
            });

            UpgradeManager.UpgradeManagerInitializing -= SetupPowerManagerUpgrades;
        }
    }
}
