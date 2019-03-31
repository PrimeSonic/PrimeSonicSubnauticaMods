namespace MoreCyclopsUpgrades.Managers
{
    using Common;
    using CyclopsUpgrades;
    using Modules.Enhancement;
    using Monobehaviors;
    using SaveData;
    using System.Collections.Generic;
    using UnityEngine;

    internal class PowerManager
    {
        internal const float MaxSolarDepth = 200f;
        internal const float SolarChargingFactor = 0.03f;
        internal const float ThermalChargingFactor = 1.5f;
        internal const float BatteryDrainRate = 0.01f;
        internal const float Mk2ChargeRateModifier = 1.15f; // The MK2 charging modules get a 15% bonus to their charge rate.
        internal const float NuclearDrainRate = 0.15f;

        private const float EnginePowerPenalty = 0.7f;

        private const int MaxSpeedBoosters = 6;
        private const int PowerIndexCount = 4;
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

        public List<CyBioReactorMono> CyBioReactors { get; } = new List<CyBioReactorMono>();
        private List<CyBioReactorMono> TempCache = new List<CyBioReactorMono>();

        internal bool HasBioReactors => this.CyBioReactors.Count > 0;
        internal PowerIconState PowerIcons { get; } = new PowerIconState();

        internal UpgradeHandler SpeedBoosters { get; set; }
        internal ChargingUpgradeHandler SolarCharger { get; set; }
        internal ChargingUpgradeHandler ThermalCharger { get; set; }
        internal BatteryUpgradeHandler SolarChargerMk2 { get; set; }
        internal BatteryUpgradeHandler ThermalChargerMk2 { get; set; }
        internal BatteryUpgradeHandler NuclearCharger { get; set; }
        internal TieredUpgradesHandlerCollection<int> EngineEfficientyUpgrades { get; set; }
        internal BioBoosterUpgradeHandler BioBoosters { get; set; }

        public CyclopsManager Manager { get; private set; }

        public SubRoot Cyclops => this.Manager.Cyclops;

        private UpgradeManager UpgradeManager => this.Manager.UpgradeManager;

        private CyclopsMotorMode motorMode;
        private CyclopsMotorMode MotorMode => motorMode ?? (motorMode = this.Cyclops.GetComponentInChildren<CyclopsMotorMode>());

        private SubControl subControl;
        private SubControl SubControl => subControl ?? (subControl = this.Cyclops.GetComponentInChildren<SubControl>());

        private float LastKnownPowerRating { get; set; } = -1f;
        private int LastKnownSpeedBoosters { get; set; } = -1;
        private int LastKnownPowerIndex { get; set; } = -1;

        private float[] OriginalSpeeds { get; } = new float[3];

        public bool Initialize(CyclopsManager manager)
        {
            if (this.Manager != null)
                return false; // Already initialized

            this.Manager = manager;

            if (this.MotorMode == null)
                return false;

            // Store the original values before we start to change them
            this.OriginalSpeeds[0] = this.MotorMode.motorModeSpeeds[0];
            this.OriginalSpeeds[1] = this.MotorMode.motorModeSpeeds[1];
            this.OriginalSpeeds[2] = this.MotorMode.motorModeSpeeds[2];

            SyncBioReactors();

            return true;
        }

        internal void SyncBioReactors()
        {
            TempCache.Clear();

            CyBioReactorMono[] cyBioReactors = this.Cyclops.GetAllComponentsInChildren<CyBioReactorMono>();

            foreach (CyBioReactorMono cyBioReactor in cyBioReactors)
            {
                if (TempCache.Contains(cyBioReactor))
                    continue; // This is a workaround because of the object references being returned twice in this array.

                TempCache.Add(cyBioReactor);

                if (cyBioReactor.ParentCyclops == null)
                {
                    QuickLogger.Debug("CyBioReactorMono synced externally");
                    // This is a workaround to get a reference to the Cyclops into the AuxUpgradeConsole
                    cyBioReactor.ConnectToCyclops(this.Cyclops, this.Manager);
                }
            }

            if (TempCache.Count != this.CyBioReactors.Count)
            {
                this.CyBioReactors.Clear();
                this.CyBioReactors.AddRange(TempCache);
            }
        }

        /// <summary>
        /// Updates the Cyclops power and speed rating.
        /// Power Rating manages engine efficiency as well as the power cost of using Silent Running, Sonar, and Defense Shield.
        /// Speed rating manages bonus speed across all motor modes.
        /// </summary>
        internal void UpdatePowerSpeedRating()
        {
            int powerIndex = this.EngineEfficientyUpgrades.HighestValue;
            int speedBoosters = this.SpeedBoosters.Count;

            if (this.LastKnownPowerIndex != powerIndex)
            {
                this.LastKnownPowerIndex = powerIndex;

                this.Cyclops.silentRunningPowerCost = SilentRunningPowerCosts[powerIndex];
                this.Cyclops.sonarPowerCost = SonarPowerCosts[powerIndex];
                this.Cyclops.shieldPowerCost = ShieldPowerCosts[powerIndex];
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

            if (this.LastKnownPowerRating != powerRating)
            {
                this.LastKnownPowerRating = powerRating;

                this.Cyclops.currPowerRating = powerRating;

                // Inform the new power rating just like the original method would.
                ErrorMessage.AddMessage(Language.main.GetFormat("PowerRatingNowFormat", powerRating));
            }

            if (speedBoosters > MaxSpeedBoosters)
                return; // Exit here

            if (this.LastKnownSpeedBoosters != speedBoosters)
            {
                this.LastKnownSpeedBoosters = speedBoosters;

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

                ErrorMessage.AddMessage(CyclopsSpeedBooster.SpeedRatingText(this.LastKnownSpeedBoosters, Mathf.RoundToInt(StandardMultiplier * 100)));
            }
        }

        /// <summary>
        /// Recharges the cyclops' power cells using all charging modules across all upgrade consoles.
        /// </summary>
        internal void RechargeCyclops()
        {
            if (this.UpgradeManager == null)
            {
                ErrorMessage.AddMessage("RechargeCyclops: UpgradeManager is null");
                return;
            }

            if (Time.timeScale == 0f) // Is the game paused?
                return;

            if (!this.UpgradeManager.HasChargingModules && !this.HasBioReactors)
                return; // No charging modules, early exit

            float powerDeficit = this.Cyclops.powerRelay.GetMaxPower() - this.Cyclops.powerRelay.GetPower();

            float surplusPower = 0f;
            bool renewablePowerAvailable = false;

            this.Manager.HUDManager.UpdateTextVisibility();

            // Handle solar power
            if (this.SolarCharger.HasUpgrade || this.SolarChargerMk2.HasUpgrade)
            {
                float solarStatus = GetSolarStatus();
                float availableSolarEnergy = SolarChargingFactor * solarStatus;
                this.PowerIcons.SolarStatus = solarStatus * 100;
                this.PowerIcons.Solar = availableSolarEnergy > MinimalPowerValue;

                surplusPower += this.SolarCharger.ChargeCyclops(this.Cyclops, ref availableSolarEnergy, ref powerDeficit);

                bool usingSolarBatteryPower = false;

                if (this.PowerIcons.Solar)
                {
                    surplusPower += this.SolarChargerMk2.ChargeCyclops(this.Cyclops, ref availableSolarEnergy, ref powerDeficit);
                }
                else
                {
                    this.SolarChargerMk2.ChargeCyclops(this.Cyclops, BatteryDrainRate, ref powerDeficit);
                    usingSolarBatteryPower |= !this.PowerIcons.Thermal && this.SolarChargerMk2.BatteryHasCharge;
                }

                this.PowerIcons.SolarBattery = usingSolarBatteryPower;
                this.PowerIcons.SolarBatteryCharge = this.SolarChargerMk2.TotalBatteryCharge;
                this.PowerIcons.SolarBatteryCapacity = this.SolarChargerMk2.TotalBatteryCapacity;
                renewablePowerAvailable |= this.PowerIcons.Solar || this.PowerIcons.SolarBattery;
            }
            else
            {
                this.PowerIcons.Solar = false;
                this.PowerIcons.SolarBattery = false;
            }

            // Handle thermal power
            if (this.ThermalCharger.HasUpgrade || this.ThermalChargerMk2.HasUpgrade)
            {
                float thermalStatus = GetThermalStatus();
                float availableThermalEnergy = ThermalChargingFactor * Time.deltaTime * this.Cyclops.thermalReactorCharge.Evaluate(thermalStatus);
                this.PowerIcons.ThermalStatus = thermalStatus;
                this.PowerIcons.Thermal = availableThermalEnergy > MinimalPowerValue;

                surplusPower += this.ThermalCharger.ChargeCyclops(this.Cyclops, ref availableThermalEnergy, ref powerDeficit);

                bool usingThermalBatteryPower = false;

                if (this.PowerIcons.Thermal)
                {
                    surplusPower += this.ThermalChargerMk2.ChargeCyclops(this.Cyclops, ref availableThermalEnergy, ref powerDeficit);
                }
                else
                {
                    this.ThermalChargerMk2.ChargeCyclops(this.Cyclops, BatteryDrainRate, ref powerDeficit);
                    usingThermalBatteryPower |= !this.PowerIcons.Thermal && this.ThermalChargerMk2.BatteryHasCharge;
                }

                this.PowerIcons.ThermalBattery = usingThermalBatteryPower;
                this.PowerIcons.ThermalBatteryCharge = this.ThermalChargerMk2.TotalBatteryCharge;
                this.PowerIcons.ThermalBatteryCapacity = this.ThermalChargerMk2.TotalBatteryCapacity;
                renewablePowerAvailable |= this.PowerIcons.Thermal || this.PowerIcons.ThermalBattery;
            }
            else
            {
                this.PowerIcons.Thermal = false;
                this.PowerIcons.ThermalBattery = false;
            }

            // Handle bio power
            if (this.CyBioReactors.Count > 0)
            {
                float totalBioCharge = 0f;
                float bioCapacity = 0f;

                int countWithPower = 0;
                foreach (CyBioReactorMono reactor in this.CyBioReactors)
                {
                    if (!reactor.HasPower)
                        continue;

                    countWithPower++;
                    reactor.ChargeCyclops(BatteryDrainRate, ref powerDeficit);
                    totalBioCharge += reactor.Battery._charge;
                    bioCapacity = reactor.Battery._capacity;
                }

                bool hasBioPower = countWithPower > 0;
                this.PowerIcons.Bio = hasBioPower;
                renewablePowerAvailable |= hasBioPower;
                this.PowerIcons.BioCharge = totalBioCharge;
                this.PowerIcons.BioCapacity = bioCapacity * countWithPower;
            }
            else
            {
                this.PowerIcons.Bio = false;
            }

            bool cyclopsDoneCharging = powerDeficit <= MinimalPowerValue;
            bool hasSurplusPower = surplusPower > MinimalPowerValue;
            bool activelyCharging = !this.PowerIcons.Solar && !this.PowerIcons.Thermal;

            this.PowerIcons.SolarBattery &= activelyCharging;
            this.PowerIcons.ThermalBattery &= activelyCharging;


            this.PowerIcons.Nuclear =
                this.NuclearCharger.HasUpgrade &&
                !renewablePowerAvailable && // Only if there's no renewable power available        
                !hasSurplusPower;

            this.PowerIcons.NuclearCharge = this.NuclearCharger.TotalBatteryCharge;
            this.PowerIcons.NuclearCapacity = this.NuclearCharger.TotalBatteryCapacity;

            if (this.PowerIcons.Nuclear && // Nuclear power enabled
                !cyclopsDoneCharging && // Halt charging if Cyclops is on full charge                
                powerDeficit > NuclearModuleConfig.MinimumEnergyDeficit) // User config for threshold to start charging                
            {
                // We'll only charge from the nuclear cells if we aren't getting power from the other modules.
                this.NuclearCharger.ChargeCyclops(this.Cyclops, NuclearDrainRate, ref powerDeficit);
            }

            // If the Cyclops is at full energy and it's generating a surplus of power, it can recharge a reserve battery
            if (cyclopsDoneCharging && hasSurplusPower)
            {
                // Recycle surplus power back into the batteries that need it

                if (surplusPower % 2 != 0) // Let this be pseudo-random so we aren't always charging the same battery first each time
                {
                    this.SolarChargerMk2.RechargeBatteries(ref surplusPower);
                    this.ThermalChargerMk2.RechargeBatteries(ref surplusPower);
                }
                else
                {
                    this.ThermalChargerMk2.RechargeBatteries(ref surplusPower);
                    this.SolarChargerMk2.RechargeBatteries(ref surplusPower);
                }
            }
        }

        /// <summary>
        /// Gets the total available reserve power across all equipment upgrade modules.
        /// </summary>
        /// <returns>The <see cref="int"/> value of the total available reserve power.</returns>
        internal int GetTotalReservePower()
        {
            float availableReservePower = 0f;
            availableReservePower += this.SolarChargerMk2.TotalBatteryCharge;
            availableReservePower += this.ThermalChargerMk2.TotalBatteryCharge;
            availableReservePower += this.NuclearCharger.TotalBatteryCharge;

            foreach (CyBioReactorMono reactor in this.CyBioReactors)
                availableReservePower += reactor.Battery._charge;

            return Mathf.FloorToInt(availableReservePower);
        }

        /// <summary>
        /// Gets the amount of available energy provided by the currently available sunlight.
        /// </summary>
        /// <returns>The currently available solar energy.</returns>
        private float GetSolarChargeAmount()
        {
            // The code here mostly replicates what the UpdateSolarRecharge() method does from the SeaMoth class.
            // Consessions were made for the differences between the Seamoth and Cyclops upgrade modules.

            if (DayNightCycle.main == null)
                return 0f; // Safety check

            // This is 1-to-1 the same way the Seamoth calculates its solar charging rate.

            return SolarChargingFactor *
                   DayNightCycle.main.GetLocalLightScalar() *
                   Mathf.Clamp01((MaxSolarDepth + this.Cyclops.transform.position.y) / MaxSolarDepth); // Distance to surfuce
        }

        private float GetSolarStatus()
        {
            if (DayNightCycle.main == null)
                return 0f; // Safety check

            return DayNightCycle.main.GetLocalLightScalar() *
                   Mathf.Clamp01((MaxSolarDepth + this.Cyclops.transform.position.y) / MaxSolarDepth);
        }

        /// <summary>
        ///  Gets the amount of available energy provided by the current ambient heat.
        /// </summary>
        /// <returns>The currently available thermal energy.</returns>
        private float GetThermalChargeAmount()
        {
            // This code mostly replicates what the UpdateThermalReactorCharge() method does from the SubRoot class

            if (WaterTemperatureSimulation.main == null)
                return 0f; // Safety check

            return ThermalChargingFactor *
                   Time.deltaTime *
                   this.Cyclops.thermalReactorCharge.Evaluate(WaterTemperatureSimulation.main.GetTemperature(this.Cyclops.transform.position)); // Temperature
        }

        private float GetThermalStatus()
        {
            if (WaterTemperatureSimulation.main == null)
                return 0f; // Safety check

            return WaterTemperatureSimulation.main.GetTemperature(this.Cyclops.transform.position);
        }
    }
}
