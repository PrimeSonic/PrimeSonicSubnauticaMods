namespace MoreCyclopsUpgrades
{
    using Caching;
    using Common;
    using Modules;
    using SaveData;
    using UnityEngine;

    internal class PowerManager : MonoBehaviour
    {
        private const float MaxSolarDepth = 200f;
        private const float SolarChargingFactor = 0.03f;
        private const float ThermalChargingFactor = 1.5f;
        private const float BatteryDrainRate = 0.01f;

        private const float Mk2ChargeRateModifier = 1.15f; // The MK2 charging modules get a 15% bonus to their charge rate.

        private const float NuclearDrainRate = 0.15f;

        private const float EnginePowerPenalty = 0.5f; // 50% reduced engine efficiency for each speed booster module

        private const int SpeedIndexCount = 6;
        private const int PowerIndexCount = 4;
        private const int MaxSpeedIndex = SpeedIndexCount - 1;

        private static readonly float[] SlowSpeedBonuses = new float[SpeedIndexCount]
        {
            0.15f, 0.10f, 0.05f, 0.05f, 0.05f, 0.05f // Diminishing returns on speed modules
        };

        private static readonly float[] StandardSpeedBonuses = new float[SpeedIndexCount]
        {
            0.40f, 0.30f, 0.25f, 0.20f, 0.15f, 0.10f // Diminishing returns on speed modules
        };

        private static readonly float[] FlankSpeedBonuses = new float[SpeedIndexCount]
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

        private SubRoot Cyclops { get; set; } = null;
        private UpgradeManager UpgradeManager { get; set; } = null;
        private CyclopsMotorMode MotorMode { get; set; } = null;
        private SubControl SubControl { get; set; } = null;

        private float LastKnownPowerRating = -1f;
        private int LastKnownSpeedIndex = -1;
        private int LastKnownPowerIndex = -1;

        private float[] OriginalSpeeds { get; } = new float[3];

        private int CurrentReservePower = 0;
        private float AvailablePower = 0f;
        private float AvailablePowerRatio = 0f;
        private int PowerPercentage = 0;
        private float CurrentBatteryPower = 0f;
        private int TotalPowerUnits = 0;

        private float PowerDeficit = 0f;
        private float AvailableSolarEnergy = 0f;
        private float AvailableThermalEnergy = 0f;
        private float SurplusPower = 0f;
        private bool RenewablePowerAvailable = false;
        private bool CyclopsDoneCharging = false;
        private Battery LastBatteryToCharge = null;

        public void Initialize(SubRoot cyclops, UpgradeManager upgradeConsoleCache)
        {
            this.Cyclops = cyclops;
            this.UpgradeManager = upgradeConsoleCache;
            this.MotorMode = cyclops.GetComponentInChildren<CyclopsMotorMode>();
            this.SubControl = cyclops.GetComponentInChildren<SubControl>();

            // Store the original values before we start to change them
            this.OriginalSpeeds[0] = this.MotorMode.motorModeSpeeds[0];
            this.OriginalSpeeds[1] = this.MotorMode.motorModeSpeeds[1];
            this.OriginalSpeeds[2] = this.MotorMode.motorModeSpeeds[2];
        }

        /// <summary>
        /// Updates the Cyclops power index. This manages engine efficiency as well as the power cost of using Silent Running, Sonar, and Defense Shield.
        /// </summary>
        /// <param name="cyclops">The this.Cyclops.</param>
        internal void UpdatePowerSpeedRating()
        {
            int powerIndex = this.UpgradeManager.PowerIndex;
            int speedIndex = this.UpgradeManager.SpeedIndex;

            if (LastKnownPowerIndex != powerIndex)
            {
                LastKnownPowerIndex = powerIndex;

                this.Cyclops.silentRunningPowerCost = SilentRunningPowerCosts[powerIndex];
                this.Cyclops.sonarPowerCost = SonarPowerCosts[powerIndex];
                this.Cyclops.shieldPowerCost = ShieldPowerCosts[powerIndex];
            }

            // Speed modules can affect power rating too
            float powerRating = Mathf.Max(0.01f, EnginePowerRatings[powerIndex] - speedIndex * EnginePowerPenalty);

            if (LastKnownPowerRating != powerRating)
            {
                LastKnownPowerRating = powerRating;

                this.Cyclops.SetPrivateField("currPowerRating", powerRating);

                // Inform the new power rating just like the original method would.
                ErrorMessage.AddMessage(Language.main.GetFormat("PowerRatingNowFormat", powerRating));
            }

            if (speedIndex > MaxSpeedIndex)
            {
                ErrorMessage.AddMessage($"Speed rating already at maximum. You have {speedIndex - SpeedIndexCount} too many.");
                return;
            }

            if (LastKnownSpeedIndex != speedIndex)
            {
                LastKnownSpeedIndex = speedIndex;

                float SlowMultiplier = 1f;
                float StandardMultiplier = 1f;
                float FlankMultiplier = 1f;

                // Calculate the speed multiplier with diminishing returns
                while (speedIndex-- > 0)
                {
                    SlowMultiplier += SlowSpeedBonuses[speedIndex];
                    StandardMultiplier += StandardSpeedBonuses[speedIndex];
                    FlankMultiplier += FlankSpeedBonuses[speedIndex];
                }

                // These will apply when changing speed modes
                this.MotorMode.motorModeSpeeds[0] = this.OriginalSpeeds[0] * SlowMultiplier;
                this.MotorMode.motorModeSpeeds[1] = this.OriginalSpeeds[1] * StandardMultiplier;
                this.MotorMode.motorModeSpeeds[2] = this.OriginalSpeeds[2] * FlankMultiplier;

                // These will apply immediately
                CyclopsMotorMode.CyclopsMotorModes currentMode = this.MotorMode.cyclopsMotorMode;
                this.SubControl.BaseForwardAccel = this.MotorMode.motorModeSpeeds[(int)currentMode];

                ErrorMessage.AddMessage($"Speed rating is now at {(StandardMultiplier * 100):00}%");

                if (LastKnownSpeedIndex == MaxSpeedIndex)
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
            CurrentReservePower = GetTotalReservePower();

            if (CurrentReservePower > 0f)
            {
                cyclopsHelmHUD.powerText.color = Color.cyan; // Distinct color for when reserve power is available
            }
            else
            {
                cyclopsHelmHUD.powerText.color = Color.white; // Normal color
            }

            if (lastReservePower != CurrentReservePower)
            {
                AvailablePower = CurrentReservePower + cyclopsHelmHUD.subRoot.powerRelay.GetPower();

                AvailablePowerRatio = AvailablePower / cyclopsHelmHUD.subRoot.powerRelay.GetMaxPower();

                // Min'd with 999 since this textbox can only display 4 characeters
                PowerPercentage = Mathf.Min(999, Mathf.CeilToInt(AvailablePowerRatio * 100f));

                cyclopsHelmHUD.powerText.text = $"{PowerPercentage}%";

                lastReservePower = CurrentReservePower;
            }
        }

        /// <summary>
        /// Recharges the cyclops' power cells using all charging modules across all upgrade consoles.
        /// </summary>
        /// <param name="__instance">The instance.</param>
        internal void RechargeCyclops()
        {
            if (!this.UpgradeManager.HasChargingModules)
                return; // No charging modules, early exit

            PowerDeficit = this.Cyclops.powerRelay.GetMaxPower() - this.Cyclops.powerRelay.GetPower();

            SurplusPower = 0f;
            LastBatteryToCharge = null;

            RenewablePowerAvailable = false;

            if (this.UpgradeManager.HasSolarModules) // Handle solar power
            {
                AvailableSolarEnergy = GetSolarChargeAmount();

                if (this.UpgradeManager.SolarModuleCount > 0 && AvailableSolarEnergy > 0f)
                {
                    SurplusPower += ChargeFromStandardModule(this.UpgradeManager.SolarModuleCount * AvailableSolarEnergy, ref PowerDeficit);
                    RenewablePowerAvailable = true;
                }

                foreach (Battery battery in this.UpgradeManager.SolarMk2Batteries)
                {
                    SurplusPower += ChargeFromModuleMk2(battery, AvailableSolarEnergy, BatteryDrainRate, ref PowerDeficit);
                    RenewablePowerAvailable |= battery.charge > 0f;

                    if (battery.charge < battery.capacity)
                        LastBatteryToCharge = battery;
                }
            }

            if (this.UpgradeManager.HasThermalModules) // Handle thermal power
            {
                AvailableThermalEnergy = GetThermalChargeAmount();

                if (this.UpgradeManager.ThermalModuleCount > 0 && AvailableThermalEnergy > 0f)
                {
                    SurplusPower += ChargeFromStandardModule(this.UpgradeManager.ThermalModuleCount * AvailableThermalEnergy, ref PowerDeficit);
                    RenewablePowerAvailable = true;
                }

                foreach (Battery battery in this.UpgradeManager.ThermalMk2Batteries)
                {
                    SurplusPower += ChargeFromModuleMk2(battery, AvailableThermalEnergy, BatteryDrainRate, ref PowerDeficit);
                    RenewablePowerAvailable |= battery.charge > 0f;

                    if (battery.charge < battery.capacity)
                        LastBatteryToCharge = battery;
                }
            }

            CyclopsDoneCharging = PowerDeficit <= 0.001f;

            if (this.UpgradeManager.HasNuclearModules && // Handle nuclear power
                !CyclopsDoneCharging && // Halt charging if Cyclops is on full charge
                PowerDeficit > NuclearModuleConfig.MinimumEnergyDeficit && // User config for threshold to start charging
                !RenewablePowerAvailable) // Only if there's no renewable power available
            {
                // We'll only charge from the nuclear cells if we aren't getting power from the other modules.
                foreach (NuclearModuleDetails module in this.UpgradeManager.NuclearModules)
                {
                    ChargeCyclopsFromBattery(module.NuclearBattery, NuclearDrainRate, ref PowerDeficit);
                    HandleNuclearBatteryDepletion(module.ParentEquipment, module.SlotName, module.NuclearBattery);
                }
            }

            // If the Cyclops is at full energy and it's generating a surplus of power, it can recharge a reserve battery
            if (CyclopsDoneCharging && SurplusPower > 0f && LastBatteryToCharge != null)
            {
                // Recycle surplus power back into the batteries that need it
                LastBatteryToCharge.charge = Mathf.Min(LastBatteryToCharge.capacity, LastBatteryToCharge.charge + SurplusPower);
            }
        }

        /// <summary>
        /// Updates the console HUD using data from all equipment modules across all upgrade consoles.
        /// </summary>
        /// <param name="hudManager">The console HUD manager.</param>
        internal void UpdateConsoleHUD(CyclopsUpgradeConsoleHUDManager hudManager)
        {
            CurrentReservePower = GetTotalReservePower();

            CurrentBatteryPower = hudManager.subRoot.powerRelay.GetPower();

            if (CurrentReservePower > 0)
            {
                hudManager.energyCur.color = Color.cyan; // Distinct color for when reserve power is available
            }
            else
            {
                hudManager.energyCur.color = Color.white; // Normal color
            }

            TotalPowerUnits = Mathf.CeilToInt(CurrentBatteryPower + CurrentReservePower);

            hudManager.energyCur.text = IntStringCache.GetStringForInt(TotalPowerUnits);

            NuclearModuleConfig.SetCyclopsMaxPower(hudManager.subRoot.powerRelay.GetMaxPower());
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
        /// Gets the battery of the upgrade module in the specified slot.
        /// </summary>
        /// <param name="modules">The equipment modules.</param>
        /// <param name="slotName">The slot name.</param>
        /// <returns>The <see cref="Battery"/> component from the upgrade module.</returns>
        internal static Battery GetBatteryInSlot(Equipment modules, string slotName)
        {
            // Get the battery component
            return modules.GetItemInSlot(slotName).item.GetComponent<Battery>();
        }

        /// <summary>
        /// Charges the Cyclops using a standard charging module.
        /// </summary>
        /// <param name="cyclops">The this.Cyclops.</param>
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
        /// <param name="cyclops">The this.Cyclops.</param>
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

        private float ChargeAmt = 0;

        /// <summary>
        /// Charges the cyclops from the reserve battery of a non-standard charging module.
        /// </summary>
        /// <param name="cyclops">The this.Cyclops.</param>
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
            ChargeAmt = Mathf.Min(powerDeficit, drainingRate);

            if (battery.charge > ChargeAmt)
            {
                battery.charge -= ChargeAmt;
            }
            else // Battery about to be fully drained
            {
                ChargeAmt = battery.charge; // Take what's left
                battery.charge = 0f; // Set battery to empty
            }

            powerDeficit -= ChargeAmt; // This is to prevent draining more than needed if the power cells were topped up mid-loop

            this.Cyclops.powerRelay.AddEnergy(ChargeAmt, out float amtStored);
        }

        /// <summary>
        /// Charges the cyclops and specified battery.
        /// This happens if a Mk2 charging module with a reserve battery is currently producing power.
        /// </summary>
        /// <param name="cyclops">The this.Cyclops.</param>
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
        private void HandleNuclearBatteryDepletion(Equipment modules, string slotName, Battery nuclearBattery)
        {
            if (nuclearBattery.charge > 0f)
                return; // Still has charge, skip

            // Drained nuclear batteries are handled just like how the Nuclear Reactor handles depleated reactor rods

            InventoryItem inventoryItem = modules.RemoveItem(slotName, true, false);
            Object.Destroy(inventoryItem.item.gameObject);
            modules.AddItem(slotName, CyclopsModule.SpawnCyclopsModule(CyclopsModule.DepletedNuclearModuleID), true);
            ErrorMessage.AddMessage("Nuclear Reactor Module depleted");
        }

        /// <summary>
        /// Gets the amount of available energy provided by the currently available sunlight.
        /// </summary>
        /// <param name="cyclops">The this.Cyclops.</param>
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
        /// <param name="cyclops">The this.Cyclops.</param>
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
