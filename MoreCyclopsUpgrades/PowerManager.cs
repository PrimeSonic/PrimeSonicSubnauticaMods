namespace MoreCyclopsUpgrades
{
    using Caching;
    using Common;
    using Modules;
    using SaveData;
    using UnityEngine;

    internal class PowerManager
    {
        private const float MaxSolarDepth = 200f;
        private const float SolarChargingFactor = 0.03f;
        private const float ThermalChargingFactor = 1.5f;
        private const float BatteryDrainRate = 0.01f;
        private const float Mk2ChargeRateModifier = 1.15f; // The MK2 charging modules get a 15% bonus to their charge rate.
        private const float NuclearDrainRate = 0.15f;
        private const float EnginePowerPenalty = 0.5f; // 50% reduced engine efficiency for each speed booster module

        private const int MaxSpeedBoosters = 6;
        private const int PowerIndexCount = 4;

        private static readonly float[] SlowSpeedBonuses = new float[MaxSpeedBoosters]
        {
            0.15f, 0.10f, 0.05f, 0.05f, 0.05f, 0.05f // Diminishing returns on speed modules
        };

        private static readonly float[] StandardSpeedBonuses = new float[MaxSpeedBoosters]
        {
            0.40f, 0.30f, 0.25f, 0.20f, 0.15f, 0.10f // Diminishing returns on speed modules
        };

        private static readonly float[] FlankSpeedBonuses = new float[MaxSpeedBoosters]
        {
            0.50f, 0.25f, 0.15f, 0.10f, 0.10f, 0.10f // Diminishing returns on speed modules
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

        public CyclopsManager Manager { get; private set; }

        public SubRoot Cyclops => Manager.Cyclops;

        private UpgradeManager UpgradeManager => Manager.UpgradeManager;

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

            // Store the original values before we start to change them
            this.OriginalSpeeds[0] = this.MotorMode.motorModeSpeeds[0];
            this.OriginalSpeeds[1] = this.MotorMode.motorModeSpeeds[1];
            this.OriginalSpeeds[2] = this.MotorMode.motorModeSpeeds[2];

            return true;
        }

        /// <summary>
        /// Updates the Cyclops power and speed rating.
        /// Power Rating manages engine efficiency as well as the power cost of using Silent Running, Sonar, and Defense Shield.
        /// Speed rating manages bonus speed across all motor modes.
        /// </summary>
        internal void UpdatePowerSpeedRating()
        {
            int powerIndex = this.UpgradeManager.PowerIndex;
            int speedBoosters = this.UpgradeManager.SpeedBoosters;

            if (this.LastKnownPowerIndex != powerIndex)
            {
                this.LastKnownPowerIndex = powerIndex;

                this.Cyclops.silentRunningPowerCost = SilentRunningPowerCosts[powerIndex];
                this.Cyclops.sonarPowerCost = SonarPowerCosts[powerIndex];
                this.Cyclops.shieldPowerCost = ShieldPowerCosts[powerIndex];
            }

            // Speed modules can affect power rating too
            float powerRating = Mathf.Max(0.01f, EnginePowerRatings[powerIndex] - speedBoosters * EnginePowerPenalty);

            if (this.LastKnownPowerRating != powerRating)
            {
                this.LastKnownPowerRating = powerRating;

                this.Cyclops.SetPrivateField("currPowerRating", powerRating);

                // Inform the new power rating just like the original method would.
                ErrorMessage.AddMessage(Language.main.GetFormat("PowerRatingNowFormat", powerRating));
            }

            if (speedBoosters > MaxSpeedBoosters)
            {
                ErrorMessage.AddMessage($"Speed rating already at maximum. You have {speedBoosters - MaxSpeedBoosters} too many.");
                return; // Exit here
            }

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

                ErrorMessage.AddMessage($"Speed rating is now at +{this.LastKnownSpeedBoosters} : {StandardMultiplier * 100:00}%");

                if (this.LastKnownSpeedBoosters == MaxSpeedBoosters)
                {
                    ErrorMessage.AddMessage($"Maximum speed rating reached");
                }
            }
        }

        /// <summary>
        /// Updates the Cyclops helm HUD  using data from all equipment modules across all upgrade consoles.
        /// </summary>
        /// <param name="cyclopsHelmHUD">The instance.</param>
        /// <param name="lastReservePower">The last reserve power.</param>
        internal void UpdateHelmHUD(CyclopsHelmHUDManager cyclopsHelmHUD, ref int lastReservePower)
        {
            if (this.UpgradeManager == null)
            {
                ErrorMessage.AddMessage("UpdateHelmHUD: UpgradeManager is null");
                return;
            }

            int currentReservePower = GetTotalReservePower();

            if (currentReservePower > 0f)
            {
                cyclopsHelmHUD.powerText.color = Color.cyan; // Distinct color for when reserve power is available
            }
            else
            {
                cyclopsHelmHUD.powerText.color = Color.white; // Normal color
            }

            if (lastReservePower != currentReservePower)
            {
                float availablePower = currentReservePower + this.Cyclops.powerRelay.GetPower();

                float availablePowerRatio = availablePower / this.Cyclops.powerRelay.GetMaxPower();

                // Min'd with 999 since this textbox can only display 4 characeters
                int powerPercentage = Mathf.Min(999, Mathf.CeilToInt(availablePowerRatio * 100f));

                cyclopsHelmHUD.powerText.text = $"{powerPercentage}%";

                lastReservePower = currentReservePower;
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

            if (!this.UpgradeManager.HasChargingModules)
                return; // No charging modules, early exit

            float powerDeficit = this.Cyclops.powerRelay.GetMaxPower() - this.Cyclops.powerRelay.GetPower();

            float surplusPower = 0f;
            Battery lastBatteryToCharge = null;

            bool renewablePowerAvailable = false;

            if (this.UpgradeManager.HasSolarModules) // Handle solar power
            {
                float availableSolarEnergy = GetSolarChargeAmount();

                if (this.UpgradeManager.SolarModuleCount > 0 && availableSolarEnergy > 0f)
                {
                    surplusPower += ChargeFromStandardModule(this.UpgradeManager.SolarModuleCount * availableSolarEnergy, ref powerDeficit);
                    renewablePowerAvailable = true;
                }

                foreach (Battery battery in this.UpgradeManager.SolarMk2Batteries)
                {
                    surplusPower += ChargeFromModuleMk2(battery, availableSolarEnergy, BatteryDrainRate, ref powerDeficit);
                    renewablePowerAvailable |= battery.charge > 0f;

                    if (battery.charge < battery.capacity)
                        lastBatteryToCharge = battery;
                }
            }

            if (this.UpgradeManager.HasThermalModules) // Handle thermal power
            {
                float availableThermalEnergy = GetThermalChargeAmount();

                if (this.UpgradeManager.ThermalModuleCount > 0 && availableThermalEnergy > 0f)
                {
                    surplusPower += ChargeFromStandardModule(this.UpgradeManager.ThermalModuleCount * availableThermalEnergy, ref powerDeficit);
                    renewablePowerAvailable = true;
                }

                foreach (Battery battery in this.UpgradeManager.ThermalMk2Batteries)
                {
                    surplusPower += ChargeFromModuleMk2(battery, availableThermalEnergy, BatteryDrainRate, ref powerDeficit);
                    renewablePowerAvailable |= battery.charge > 0f;

                    if (battery.charge < battery.capacity)
                        lastBatteryToCharge = battery;
                }
            }

            bool cyclopsDoneCharging = powerDeficit <= 0.001f;

            if (this.UpgradeManager.HasNuclearModules && // Handle nuclear power
                !cyclopsDoneCharging && // Halt charging if Cyclops is on full charge
                powerDeficit > NuclearModuleConfig.MinimumEnergyDeficit && // User config for threshold to start charging
                !renewablePowerAvailable) // Only if there's no renewable power available
            {
                // We'll only charge from the nuclear cells if we aren't getting power from the other modules.
                foreach (NuclearModuleDetails module in this.UpgradeManager.NuclearModules)
                {
                    ChargeCyclopsFromBattery(module.NuclearBattery, NuclearDrainRate, ref powerDeficit);

                    if (module.NuclearBattery.charge <= 0f)
                        DepleteNuclearBattery(module.ParentEquipment, module.SlotName, module.NuclearBattery);
                }
            }

            // If the Cyclops is at full energy and it's generating a surplus of power, it can recharge a reserve battery
            if (cyclopsDoneCharging && surplusPower > 0f && lastBatteryToCharge != null)
            {
                // Recycle surplus power back into the batteries that need it
                lastBatteryToCharge.charge = Mathf.Min(lastBatteryToCharge.capacity, lastBatteryToCharge.charge + surplusPower);
            }
        }

        /// <summary>
        /// Updates the console HUD using data from all equipment modules across all upgrade consoles.
        /// </summary>
        /// <param name="hudManager">The console HUD manager.</param>
        internal void UpdateConsoleHUD(CyclopsUpgradeConsoleHUDManager hudManager)
        {
            int currentReservePower = GetTotalReservePower();

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

        /// <summary>
        /// Gets the total available reserve power across all equipment upgrade modules.
        /// </summary>
        /// <returns>The <see cref="int"/> value of the total available reserve power.</returns>
        private int GetTotalReservePower()
        {
            float availableReservePower = 0f;

            foreach (Battery battery in this.UpgradeManager.ReserveBatteries)
                availableReservePower += battery.charge;

            return Mathf.FloorToInt(availableReservePower);
        }

        /// <summary>
        /// Charges the Cyclops using a standard charging module.
        /// </summary>
        /// <param name="chargeAmount">The charge amount.</param>
        /// <param name="powerDeficit">The power deficit.</param>
        /// <returns>
        /// The amount of surplus power this cycle.
        /// This value can be <c>0f</c> if all charge was consumed.
        /// </returns>
        private float ChargeFromStandardModule(float chargeAmount, ref float powerDeficit)
        {
            if (Mathf.Approximately(powerDeficit, 0f))
                return chargeAmount; // Surplus power

            if (Mathf.Approximately(chargeAmount, 0f))
                return 0f;

            this.Cyclops.powerRelay.AddEnergy(chargeAmount, out float amtStored);
            powerDeficit = Mathf.Max(0f, powerDeficit - chargeAmount);

            return Mathf.Max(0f, chargeAmount - powerDeficit); // Surplus power
        }

        /// <summary>
        /// Charges the Cyclops using a Mk2 charging module.
        /// </summary>
        /// <param name="batteryInSlot">The battery of the Mk2 charging module.</param>
        /// <param name="chargeAmount">The charge amount.</param>
        /// <param name="drainingRate">The battery power draining rate.</param>
        /// <param name="powerDeficit">The power deficit.</param>
        /// <returns>
        /// The amount of surplus power this cycle.
        /// This value can be <c>0f</c> if all charge was consumed or if the Mk2 module is running on reserver battery power.
        /// </returns>
        private float ChargeFromModuleMk2(Battery batteryInSlot, float chargeAmount, float batteryDrainRate, ref float powerDeficit)
        {
            if (Mathf.Approximately(chargeAmount, 0f))
            {
                ChargeCyclopsFromBattery(batteryInSlot, batteryDrainRate, ref powerDeficit);
                return 0f;
            }
            else
            {
                return ChargeCyclopsAndBattery(batteryInSlot, ref chargeAmount, ref powerDeficit);
            }
        }

        /// <summary>
        /// Charges the cyclops from the reserve battery of a non-standard charging module.
        /// </summary>
        /// <param name="battery">The battery of the non-standard charging module.</param>
        /// <param name="drainingRate">The battery power draining rate.</param>
        /// <param name="powerDeficit">The power deficit.</param>
        private void ChargeCyclopsFromBattery(Battery battery, float drainingRate, ref float powerDeficit)
        {
            if (Mathf.Approximately(powerDeficit, 0f)) // No power deficit left to charge
                return; // Exit

            if (Mathf.Approximately(battery.charge, 0f)) // The battery has no charge left
                return; // Skip this battery

            // Mathf.Min is to prevent accidentally taking too much power from the battery
            float chargeAmt = Mathf.Min(powerDeficit, drainingRate);

            if (battery.charge > chargeAmt)
            {
                battery.charge -= chargeAmt;
            }
            else // Battery about to be fully drained
            {
                chargeAmt = battery.charge; // Take what's left
                battery.charge = 0f; // Set battery to empty
            }

            powerDeficit -= chargeAmt; // This is to prevent draining more than needed if the power cells were topped up mid-loop

            this.Cyclops.powerRelay.AddEnergy(chargeAmt, out float amtStored);
        }

        /// <summary>
        /// Charges the cyclops and specified battery.
        /// This happens if a Mk2 charging module with a reserve battery is currently producing power.
        /// </summary>
        /// <param name="battery">The battery from the module currently producing power.</param>
        /// <param name="chargeAmount">The charge amount.</param>
        /// <param name="powerDeficit">The power deficit.</param>
        /// <returns>
        /// The amount of surplus power this cycle.
        /// This value can be <c>0f</c> if all charge was consumed.
        /// </returns>
        private float ChargeCyclopsAndBattery(Battery battery, ref float chargeAmount, ref float powerDeficit)
        {
            chargeAmount *= Mk2ChargeRateModifier;

            this.Cyclops.powerRelay.AddEnergy(chargeAmount, out float amtStored);
            powerDeficit = Mathf.Max(0f, powerDeficit - chargeAmount);

            battery.charge = Mathf.Min(battery.capacity, battery.charge + chargeAmount);

            return Mathf.Max(0f, chargeAmount - powerDeficit); // Surplus power
        }

        /// <summary>
        /// Replaces a nuclear battery modules with Depleted Reactor Rods when they fully drained.
        /// </summary>
        /// <param name="modules">The equipment modules.</param>
        /// <param name="slotName">Th slot name.</param>
        /// <param name="nuclearBattery">The nuclear battery that just ran out.</param>
        private void DepleteNuclearBattery(Equipment modules, string slotName, Battery nuclearBattery)
        {
            // Drained nuclear batteries are handled just like how the Nuclear Reactor handles depleated reactor rods
            InventoryItem inventoryItem = modules.RemoveItem(slotName, true, false);
            Object.Destroy(inventoryItem.item.gameObject);
            modules.AddItem(slotName, CyclopsModule.SpawnCyclopsModule(CyclopsModule.DepletedNuclearModuleID), true);
            ErrorMessage.AddMessage("Nuclear Reactor Module depleted");
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
    }
}
