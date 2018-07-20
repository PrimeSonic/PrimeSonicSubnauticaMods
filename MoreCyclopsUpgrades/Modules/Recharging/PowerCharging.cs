namespace MoreCyclopsUpgrades
{
    using System.Collections.Generic;
    using Common;
    using UnityEngine;

    internal static class PowerCharging
    {
        private static List<Battery> NuclerCells = new List<Battery>(36);
        private static List<string> NuclerSlots = new List<string>(36);

        /// <summary>
        /// The MK2 charging modules get a 15% bonus to their charge rate.
        /// </summary>
        private const float Mk2ChargeRateModifier = 1.15f;
        private const float NuclearDrainRate = 0.15f;

        internal const float MaxMk2Charge = 100f;
        internal const float MaxNuclearCharge = 6000f; // Less than the normal 20k for balance

        /// <summary>
        /// Updates the Cyclops helm HUD  using data from all equipment modules across all upgrade consoles.
        /// </summary>
        /// <param name="__instance">The instance.</param>
        /// <param name="modules">The modules.</param>
        /// <param name="auxUpgradeConsoles">The aux upgrade consoles.</param>
        /// <param name="lastReservePower">The last reserve power.</param>
        internal static void UpdateHelmHUD(ref CyclopsHelmHUDManager __instance, Equipment modules, AuxUpgradeConsole[] auxUpgradeConsoles, ref int lastReservePower)
        {
            int currentReservePower = GetTotalReservePower(modules, auxUpgradeConsoles);

            if (currentReservePower > 0f)
            {
                __instance.powerText.color = Color.cyan; // Distinct color for when reserve power is available
            }
            else
            {
                __instance.powerText.color = Color.white; // Normal color
            }

            if (lastReservePower != currentReservePower)
            {
                float availablePower = currentReservePower + __instance.subRoot.powerRelay.GetPower();

                float availablePowerRatio = availablePower / __instance.subRoot.powerRelay.GetMaxPower();

                // Min'd with 999 since this textbox can only display 4 characeters
                int percentage = Mathf.Min(999, Mathf.CeilToInt(availablePowerRatio * 100f));

                __instance.powerText.text = $"{percentage}%";

                lastReservePower = currentReservePower;
            }
        }

        /// <summary>
        /// Recharges the cyclops' power cells using all charging modules across all upgrade consoles.
        /// </summary>
        /// <param name="__instance">The instance.</param>
        /// <param name="coreModules">The core modules.</param>
        /// <param name="auxUpgradeConsoles">The aux upgrade consoles.</param>
        internal static void RechargeCyclops(ref SubRoot __instance, Equipment coreModules, AuxUpgradeConsole[] auxUpgradeConsoles)
        {
            float powerDeficit = __instance.powerRelay.GetMaxPower() - __instance.powerRelay.GetPower();

            float availableSolarEnergy = SolarChargingManager.GetSolarChargeAmount(ref __instance);
            float availableThermalEnergy = ThermalChargingManager.GetThermalChargeAmount(ref __instance);

            float surplusPower = 0f;
            Battery lastBatteryToCharge = null;
            NuclerCells.Clear();
            NuclerSlots.Clear();

            bool renewablePowerAvailable = false;

            Equipment modules = coreModules;

            // Do one large loop for all upgrade consoles
            for (int moduleIndex = -1; moduleIndex < auxUpgradeConsoles.Length; moduleIndex++)
            {
                if (moduleIndex > -1)
                    modules = auxUpgradeConsoles[moduleIndex].Modules;

                foreach (string slotName in SlotHelper.SlotNames)
                {
                    TechType techTypeInSlot = modules.GetTechTypeInSlot(slotName);

                    if (techTypeInSlot == CyclopsModule.SolarChargerID) // Solar
                    {
                        surplusPower += ChargeFromStandardModule(ref __instance, availableSolarEnergy, ref powerDeficit);
                        renewablePowerAvailable |= availableSolarEnergy > 0f;
                    }
                    else if (techTypeInSlot == CyclopsModule.SolarChargerMk2ID) // Solar Mk2
                    {
                        Battery battery = GetBatteryInSlot(modules, slotName);
                        surplusPower += ChargeFromModuleMk2(ref __instance, battery, availableSolarEnergy, SolarChargingManager.BatteryDrainRate, ref powerDeficit);
                        renewablePowerAvailable |= battery.charge > 0f;

                        if (battery.charge < battery.capacity)
                            lastBatteryToCharge = battery;
                    }
                    else if (techTypeInSlot == TechType.CyclopsThermalReactorModule) // Thermal
                    {
                        surplusPower += PowerCharging.ChargeFromStandardModule(ref __instance, availableThermalEnergy, ref powerDeficit);
                        renewablePowerAvailable |= availableThermalEnergy > 0f;
                    }
                    else if (techTypeInSlot == CyclopsModule.ThermalChargerMk2ID) // Thermal Mk2
                    {
                        Battery battery = GetBatteryInSlot(modules, slotName);
                        surplusPower += ChargeFromModuleMk2(ref __instance, battery, availableThermalEnergy, ThermalChargingManager.BatteryDrainRate, ref powerDeficit);
                        renewablePowerAvailable |= battery.charge > 0f;

                        if (battery.charge < battery.capacity)
                            lastBatteryToCharge = battery;
                    }
                    else if (techTypeInSlot == CyclopsModule.NuclearChargerID) // Nuclear
                    {
                        Battery battery = GetBatteryInSlot(modules, slotName);
                        NuclerCells.Add(battery);
                        NuclerSlots.Add(slotName);
                    }
                }

                if (NuclerCells.Count > 0 && powerDeficit > NuclearModuleConfig.MinimumEnergyDeficit && !renewablePowerAvailable) // no renewable power available
                {
                    // We'll only charge from the nuclear cells if we aren't getting power from the other modules.
                    for (int nukCelIndex = 0; nukCelIndex < NuclerCells.Count; nukCelIndex++)
                    {
                        Battery battery = NuclerCells[nukCelIndex];
                        string slotName = NuclerSlots[nukCelIndex];
                        ChargeCyclopsFromBattery(ref __instance, battery, NuclearDrainRate, ref powerDeficit);
                        HandleNuclearBatteryDepletion(modules, slotName, battery);
                    }
                }
            }

            // If the Cyclops is at full energy and it's generating a surplus of power it can recharge a reserve battery
            if (powerDeficit <= 0f && surplusPower > 0f && lastBatteryToCharge != null)
            {
                // Recycle surplus power back into the batteries that need it
                lastBatteryToCharge.charge = Mathf.Min(lastBatteryToCharge.capacity, lastBatteryToCharge.charge + surplusPower);
            }
        }

        /// <summary>
        /// Updates the console HUD using data from all equipment modules across all upgrade consoles.
        /// </summary>
        /// <param name="hudManager">The console HUD manager.</param>
        /// <param name="coreModules">The core modules.</param>
        /// <param name="auxUpgradeConsoles">The aux upgrade consoles.</param>
        internal static void UpdateConsoleHUD(CyclopsUpgradeConsoleHUDManager hudManager, Equipment coreModules, AuxUpgradeConsole[] auxUpgradeConsoles)
        {
            int currentReservePower = GetTotalReservePower(coreModules, auxUpgradeConsoles);

            float currentBatteryPower = hudManager.subRoot.powerRelay.GetPower();

            if (currentReservePower > 0f)
            {
                hudManager.energyCur.color = Color.cyan; // Distinct color for when reserve power is available
            }
            else
            {
                hudManager.energyCur.color = Color.white; // Normal color
            }

            int totalPower = Mathf.CeilToInt(currentBatteryPower + currentReservePower);

            hudManager.energyCur.text = IntStringCache.GetStringForInt(totalPower);

            NuclearModuleConfig.SetCyclopsMaxPower(hudManager.subRoot.powerRelay.GetMaxPower());
        }

        /// <summary>
        /// Gets the total available reserve power across all equipment upgrade modules.
        /// </summary>
        /// <param name="modules">The equipment modules.</param>
        /// <param name="auxUpgradeConsoles">The aux upgrade consoles.</param>
        /// <returns>The <see cref="int"/> value of the total available reserve power.</returns>
        private static int GetTotalReservePower(Equipment modules, AuxUpgradeConsole[] auxUpgradeConsoles)
        {
            float availableReservePower = 0f;

            GetReserverPowerInModules(modules, ref availableReservePower);

            foreach (AuxUpgradeConsole auxConsole in auxUpgradeConsoles)
                GetReserverPowerInModules(auxConsole.Modules, ref availableReservePower);

            return Mathf.FloorToInt(availableReservePower);
        }

        /// <summary>
        /// Updates in incoming parameter with the total available reserve power across upgrade modules in this equipment.
        /// </summary>
        /// <param name="modules">The equipment modules.</param>
        /// <param name="availableReservePower">The available reserve power.</param>
        private static void GetReserverPowerInModules(Equipment modules, ref float availableReservePower)
        {
            foreach (string slotName in SlotHelper.SlotNames)
            {
                TechType techTypeInSlot = modules.GetTechTypeInSlot(slotName);

                if (techTypeInSlot == CyclopsModule.SolarChargerMk2ID ||
                    techTypeInSlot == CyclopsModule.ThermalChargerMk2ID ||
                    techTypeInSlot == CyclopsModule.NuclearChargerID)
                {
                    Battery battery = GetBatteryInSlot(modules, slotName);
                    availableReservePower += battery.charge;
                }
            }
        }

        /// <summary>
        /// Gets the battery of the upgrade module in the specified slot.
        /// </summary>
        /// <param name="modules">The equipment modules.</param>
        /// <param name="slotName">The slot name.</param>
        /// <returns>The <see cref="Battery"/> component from the upgrade module.</returns>
        private static Battery GetBatteryInSlot(Equipment modules, string slotName)
        {
            // Get the battery component
            InventoryItem item = modules.GetItemInSlot(slotName);
            Battery batteryInSlot = item.item.GetComponent<Battery>();
            return batteryInSlot;
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

            cyclops.powerRelay.AddEnergy(chargeAmt, out float amtStored);
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
            if (nuclearBattery.charge <= 0f) // Drained nuclear batteries are handled just like how the Nuclear Reactor handles depleated reactor rods
            {
                InventoryItem inventoryItem = modules.RemoveItem(slotName, true, false);
                Object.Destroy(inventoryItem.item.gameObject);
                modules.AddItem(slotName, SpawnDepletedNuclearModule(), true);
            }
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
            gameObject.AddComponent<TechTag>().type = CyclopsModule.DepletedNuclearModuleID;

            Pickupable pickupable = gameObject.GetComponent<Pickupable>().Pickup(false);
            return new InventoryItem(pickupable);
        }
    }
}
