namespace MoreCyclopsUpgrades
{
    using Common;
    using Caching;
    using SaveData;
    using UnityEngine;
    using Modules;
    using Modules.Recharging.Nuclear;

    internal static class PowerManager
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

        private static readonly float[] SlowSpeedBonuses = new float[SpeedIndexCount]
        {
            0.15f, 0.10f, 0.05f, 0f, 0f, 0f // Diminishing returns on speed modules
        };

        private static readonly float[] StandardSpeedBonuses = new float[SpeedIndexCount]
        {
            0.35f, 0.25f, 0.20f, 0.15f, 0.10f, 0.05f // Diminishing returns on speed modules
        };

        private static readonly float[] FlankSpeedBonuses = new float[SpeedIndexCount]
        {
            0.40f, 0.20f, 0.10f, 0.05f, 0.025f, 0f // Diminishing returns on speed modules
        };

        private static readonly float[] EnginePowerRatings = new float[PowerIndexCount]
        {
            1f, 3f, 5f, 6f // Lower costs here don't show up until the Mk2
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

        private static float LastKnownPowerRating = -1f;
        private static int LastKnownSpeedIndex = -1;

        private static float[] OriginalSpeeds { get; } = new float[3];

        private static int CurrentReservePower = 0;
        private static float AvailablePower = 0f;
        private static float AvailablePowerRatio = 0f;
        private static int PowerPercentage = 0;
        private static float CurrentBatteryPower = 0f;
        private static int TotalPowerUnits = 0;

        private static float PowerDeficit = 0f;
        private static float AvailableSolarEnergy = 0f;
        private static float AvailableThermalEnergy = 0f;
        private static float SurplusPower = 0f;
        private static bool RenewablePowerAvailable = false;
        private static bool CyclopsDoneCharging = false;
        private static Battery LastBatteryToCharge = null;

        /// <summary>
        /// Updates the Cyclops power index. This manages engine efficiency as well as the power cost of using Silent Running, Sonar, and Defense Shield.
        /// </summary>
        /// <param name="cyclops">The cyclops.</param>
        internal static void UpdatePowerSpeedRating(ref SubRoot cyclops)
        {
            Equipment modules = cyclops.upgradeConsole.modules;

            int powerIndex = UpgradeConsoleCache.PowerIndex;
            int speedIndex = UpgradeConsoleCache.SpeedIndex;

            // Speed modules can affect power rating too
            float nextPowerRating = Mathf.Max(0.01f, EnginePowerRatings[powerIndex] - speedIndex * EnginePowerPenalty);

            if (LastKnownPowerRating != nextPowerRating)
            {
                cyclops.silentRunningPowerCost = SilentRunningPowerCosts[powerIndex];
                cyclops.sonarPowerCost = SonarPowerCosts[powerIndex];
                cyclops.shieldPowerCost = ShieldPowerCosts[powerIndex];

                LastKnownPowerRating = nextPowerRating;

                cyclops.SetPrivateField("currPowerRating", nextPowerRating);

                // Inform the new power rating just like the original method would.
                ErrorMessage.AddMessage(Language.main.GetFormat("PowerRatingNowFormat", nextPowerRating));
            }

            if (LastKnownSpeedIndex == -1)
            {
                // Store the original values before we start to change them
                // This will only run once
                var motorMode = cyclops.GetComponentInChildren<CyclopsMotorMode>();
                OriginalSpeeds[0] = motorMode.motorModeSpeeds[0];
                OriginalSpeeds[1] = motorMode.motorModeSpeeds[1];
                OriginalSpeeds[2] = motorMode.motorModeSpeeds[2];
            }

            if (speedIndex > SpeedIndexCount)
            {
                speedIndex = SpeedIndexCount; // Limit to Max
                ErrorMessage.AddMessage($"Speed rating already at maximum");
            }

            if (LastKnownSpeedIndex != speedIndex)
            {
                float SlowMultiplier = 1f;
                float StandardMultiplier = 1f;
                float FlankMultiplier = 1f;

                // Calculate the speed multiplier with diminishing returns
                for (int s = 0; s < speedIndex; s++)
                {
                    SlowMultiplier += SlowSpeedBonuses[s];
                    StandardMultiplier += StandardSpeedBonuses[s];
                    FlankMultiplier += FlankSpeedBonuses[s];
                }

                // These will apply when changing speed modes
                var motorMode = cyclops.GetComponentInChildren<CyclopsMotorMode>();
                motorMode.motorModeSpeeds[0] = OriginalSpeeds[0] * SlowMultiplier;
                motorMode.motorModeSpeeds[1] = OriginalSpeeds[1] * StandardMultiplier;
                motorMode.motorModeSpeeds[2] = OriginalSpeeds[2] * FlankMultiplier;

                // These will apply immediately
                var subControl = cyclops.GetComponentInChildren<SubControl>();
                CyclopsMotorMode.CyclopsMotorModes currentMode = subControl.cyclopsMotorMode.cyclopsMotorMode;
                subControl.BaseForwardAccel = motorMode.motorModeSpeeds[(int)currentMode];

                LastKnownSpeedIndex = speedIndex;

                ErrorMessage.AddMessage($"Speed rating is now at {(StandardMultiplier * 100):00}%");

                if (speedIndex == SpeedIndexCount)
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
        internal static void UpdateHelmHUD(CyclopsHelmHUDManager cyclopsHelmHUD, ref int lastReservePower)
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
        internal static void RechargeCyclops(ref SubRoot __instance)
        {
            if (!UpgradeConsoleCache.HasChargingModules)
                return; // No charging modules, early exit

            PowerDeficit = __instance.powerRelay.GetMaxPower() - __instance.powerRelay.GetPower();

            SurplusPower = 0f;
            LastBatteryToCharge = null;

            RenewablePowerAvailable = false;

            if (UpgradeConsoleCache.HasSolarModules) // Handle solar power
            {
                AvailableSolarEnergy = GetSolarChargeAmount(ref __instance);

                if (UpgradeConsoleCache.SolarModuleCount > 0 && AvailableSolarEnergy > 0f)
                {
                    SurplusPower += ChargeFromStandardModule(ref __instance, UpgradeConsoleCache.SolarModuleCount * AvailableSolarEnergy, ref PowerDeficit);
                    RenewablePowerAvailable = true;
                }

                foreach (Battery battery in UpgradeConsoleCache.SolarMk2Batteries)
                {
                    SurplusPower += ChargeFromModuleMk2(ref __instance, battery, AvailableSolarEnergy, BatteryDrainRate, ref PowerDeficit);
                    RenewablePowerAvailable |= battery.charge > 0f;

                    if (battery.charge < battery.capacity)
                        LastBatteryToCharge = battery;
                }
            }

            if (UpgradeConsoleCache.HasThermalModules) // Handle thermal power
            {
                AvailableThermalEnergy = GetThermalChargeAmount(ref __instance);

                if (UpgradeConsoleCache.ThermalModuleCount > 0 && AvailableThermalEnergy > 0f)
                {
                    SurplusPower += ChargeFromStandardModule(ref __instance, UpgradeConsoleCache.ThermalModuleCount * AvailableThermalEnergy, ref PowerDeficit);
                    RenewablePowerAvailable = true;
                }

                foreach (Battery battery in UpgradeConsoleCache.ThermalMk2Batteries)
                {
                    SurplusPower += ChargeFromModuleMk2(ref __instance, battery, AvailableThermalEnergy, BatteryDrainRate, ref PowerDeficit);
                    RenewablePowerAvailable |= battery.charge > 0f;

                    if (battery.charge < battery.capacity)
                        LastBatteryToCharge = battery;
                }
            }

            CyclopsDoneCharging = Mathf.Approximately(PowerDeficit, 0f);

            if (UpgradeConsoleCache.HasNuclearModules && // Handle nuclear power
                !CyclopsDoneCharging && // Halt charging if Cyclops is on full charge
                PowerDeficit > NuclearModuleConfig.MinimumEnergyDeficit && // User config for threshold to start charging
                !RenewablePowerAvailable) // Only if there's no renewable power available
            {
                // We'll only charge from the nuclear cells if we aren't getting power from the other modules.
                foreach (NuclearModuleDetails module in UpgradeConsoleCache.NuclearModules)
                {
                    ChargeCyclopsFromBattery(ref __instance, module.NuclearBattery, NuclearDrainRate, ref PowerDeficit);
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
        internal static void UpdateConsoleHUD(CyclopsUpgradeConsoleHUDManager hudManager)
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
        private static int GetTotalReservePower()
        {
            float availableReservePower = 0f;

            foreach (Battery battery in UpgradeConsoleCache.ReserveBatteries)
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
        /// <param name="cyclops">The cyclops.</param>
        /// <param name="chargeAmount">The charge amount.</param>
        /// <param name="powerDeficit">The power deficit.</param>
        /// <returns>
        /// The amount of surplus power this cycle.
        /// This value can be <c>0f</c> if all charge was consumed.
        /// </returns>
        private static float ChargeFromStandardModule(ref SubRoot cyclops, float chargeAmount, ref float powerDeficit)
        {
            if (Mathf.Approximately(powerDeficit, 0f))
                return chargeAmount; // Surplus power

            if (Mathf.Approximately(chargeAmount, 0f))
                return 0f;

            cyclops.powerRelay.AddEnergy(chargeAmount, out float amtStored);
            powerDeficit = Mathf.Max(0f, powerDeficit - chargeAmount);

            return Mathf.Max(0f, chargeAmount - powerDeficit); // Surplus power
        }

        /// <summary>
        /// Charges the Cyclops using a Mk2 charging module.
        /// </summary>
        /// <param name="cyclops">The cyclops.</param>
        /// <param name="batteryInSlot">The battery of the Mk2 charging module.</param>
        /// <param name="chargeAmount">The charge amount.</param>
        /// <param name="drainingRate">The battery power draining rate.</param>
        /// <param name="powerDeficit">The power deficit.</param>
        /// <returns>
        /// The amount of surplus power this cycle.
        /// This value can be <c>0f</c> if all charge was consumed or if the Mk2 module is running on reserver battery power.
        /// </returns>
        private static float ChargeFromModuleMk2(ref SubRoot cyclops, Battery batteryInSlot, float chargeAmount, float batteryDrainRate, ref float powerDeficit)
        {
            if (Mathf.Approximately(chargeAmount, 0f))
            {
                ChargeCyclopsFromBattery(ref cyclops, batteryInSlot, batteryDrainRate, ref powerDeficit);
                return 0f;
            }
            else
            {
                return ChargeCyclopsAndBattery(cyclops, batteryInSlot, ref chargeAmount, ref powerDeficit);
            }
        }

        private static float ChargeAmt = 0;

        /// <summary>
        /// Charges the cyclops from the reserve battery of a non-standard charging module.
        /// </summary>
        /// <param name="cyclops">The cyclops.</param>
        /// <param name="battery">The battery of the non-standard charging module.</param>
        /// <param name="drainingRate">The battery power draining rate.</param>
        /// <param name="powerDeficit">The power deficit.</param>
        private static void ChargeCyclopsFromBattery(ref SubRoot cyclops, Battery battery, float drainingRate, ref float powerDeficit)
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

            cyclops.powerRelay.AddEnergy(ChargeAmt, out float amtStored);
        }

        /// <summary>
        /// Charges the cyclops and specified battery.
        /// This happens if a Mk2 charging module with a reserve battery is currently producing power.
        /// </summary>
        /// <param name="cyclops">The cyclops.</param>
        /// <param name="battery">The battery from the module currently producing power.</param>
        /// <param name="chargeAmount">The charge amount.</param>
        /// <param name="powerDeficit">The power deficit.</param>
        /// <returns>
        /// The amount of surplus power this cycle.
        /// This value can be <c>0f</c> if all charge was consumed.
        /// </returns>
        private static float ChargeCyclopsAndBattery(SubRoot cyclops, Battery battery, ref float chargeAmount, ref float powerDeficit)
        {
            chargeAmount *= Mk2ChargeRateModifier;

            cyclops.powerRelay.AddEnergy(chargeAmount, out float amtStored);
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
        private static void HandleNuclearBatteryDepletion(Equipment modules, string slotName, Battery nuclearBattery)
        {
            if (nuclearBattery.charge > 0f)
                return; // Still has charge, skip

            // Drained nuclear batteries are handled just like how the Nuclear Reactor handles depleated reactor rods

            InventoryItem inventoryItem = modules.RemoveItem(slotName, true, false);
            Object.Destroy(inventoryItem.item.gameObject);
            modules.AddItem(slotName, SpawnDepletedNuclearModule(), true);
            ErrorMessage.AddMessage("Nuclear Reactor Module depleted");
        }

        /// <summary>
        /// Spawns the depleted nuclear module.
        /// </summary>
        /// <returns>Returns a new <see cref="InventoryItem"/> instance of the <see cref="DepletedNuclearModule"/>.</returns>
        private static InventoryItem SpawnDepletedNuclearModule()
        {
            GameObject prefab = CraftData.GetPrefabForTechType(TechType.DepletedReactorRod);
            GameObject gameObject = GameObject.Instantiate(prefab);

            gameObject.GetComponent<PrefabIdentifier>().ClassId = DepletedNuclearModule.DepletedNameID;
            gameObject.GetComponent<TechTag>().type = CyclopsModule.DepletedNuclearModuleID;

            Pickupable pickupable = gameObject.GetComponent<Pickupable>().Pickup(false);
            return new InventoryItem(pickupable);
        }

        /// <summary>
        /// Gets the amount of available energy provided by the currently available sunlight.
        /// </summary>
        /// <param name="cyclops">The cyclops.</param>
        /// <returns>The currently available solar energy.</returns>
        private static float GetSolarChargeAmount(ref SubRoot cyclops)
        {
            // The code here mostly replicates what the UpdateSolarRecharge() method does from the SeaMoth class.
            // Consessions were made for the differences between the Seamoth and Cyclops upgrade modules.

            if (DayNightCycle.main == null)
                return 0f; // Safety check

            // This is 1-to-1 the same way the Seamoth calculates its solar charging rate.

            return SolarChargingFactor *
                   DayNightCycle.main.GetLocalLightScalar() *
                   Mathf.Clamp01((MaxSolarDepth + cyclops.transform.position.y) / MaxSolarDepth); // Distance to surfuce
        }

        /// <summary>
        ///  Gets the amount of available energy provided by the current ambient heat.
        /// </summary>
        /// <param name="cyclops">The cyclops.</param>
        /// <returns>The currently available thermal energy.</returns>
        private static float GetThermalChargeAmount(ref SubRoot cyclops)
        {
            // This code mostly replicates what the UpdateThermalReactorCharge() method does from the SubRoot class

            if (WaterTemperatureSimulation.main == null)
                return 0f; // Safety check

            return ThermalChargingFactor *
                   Time.deltaTime *
                   cyclops.thermalReactorCharge.Evaluate(WaterTemperatureSimulation.main.GetTemperature(cyclops.transform.position)); // Temperature
        }
    }
}
