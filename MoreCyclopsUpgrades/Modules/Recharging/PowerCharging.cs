namespace MoreCyclopsUpgrades
{
    using System.Collections.Generic;
    using Common;
    using UnityEngine;

    internal static class PowerCharging
    {
        private static List<Battery> NuclerCells = new List<Battery>(36);
        private static List<string> NuclerSlots = new List<string>(36);

        private const float Mk2ChargeRateModifier = 1.15f;
        private const float NuclearDrainRate = 0.15f;

        internal const float MaxMk2Charge = 100f;
        internal const float MaxNuclearCharge = 6000f; // Less than the normal 20k for balance

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
                        surplusPower += ChargeFromModule(ref __instance, availableSolarEnergy, ref powerDeficit);
                        renewablePowerAvailable |= availableSolarEnergy > 0f;
                    }
                    else if (techTypeInSlot == CyclopsModule.SolarChargerMk2ID) // Solar Mk2
                    {
                        Battery battery = GetBatteryInSlot(modules, slotName);
                        surplusPower += ChargeFromModulelMk2(ref __instance, battery, availableSolarEnergy, SolarChargingManager.BatteryDrainRate, ref powerDeficit);
                        renewablePowerAvailable |= battery.charge > 0f;

                        if (battery.charge < battery.capacity)
                            lastBatteryToCharge = battery;
                    }
                    else if (techTypeInSlot == TechType.CyclopsThermalReactorModule) // Thermal
                    {
                        surplusPower += PowerCharging.ChargeFromModule(ref __instance, availableThermalEnergy, ref powerDeficit);
                        renewablePowerAvailable |= availableThermalEnergy > 0f;
                    }
                    else if (techTypeInSlot == CyclopsModule.ThermalChargerMk2ID) // Thermal Mk2
                    {
                        Battery battery = GetBatteryInSlot(modules, slotName);
                        surplusPower += ChargeFromModulelMk2(ref __instance, battery, availableThermalEnergy, ThermalChargingManager.BatteryDrainRate, ref powerDeficit);
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
                        HandleBatteryDepletion(modules, slotName, battery);
                    }
                }
            }

            if (powerDeficit <= 0f && surplusPower > 0f && lastBatteryToCharge != null)
            {
                // Recycle surplus power back into the batteries that need it
                lastBatteryToCharge.charge = Mathf.Min(lastBatteryToCharge.capacity, lastBatteryToCharge.charge + surplusPower);
            }
        }

        internal static void UpdateConsoleHUD(CyclopsUpgradeConsoleHUDManager __instance, Equipment modules, AuxUpgradeConsole[] auxUpgradeConsoles)
        {
            int currentReservePower = GetTotalReservePower(modules, auxUpgradeConsoles);

            float currentBatteryPower = __instance.subRoot.powerRelay.GetPower();

            if (currentReservePower > 0f)
            {
                __instance.energyCur.color = Color.cyan; // Distinct color for when reserve power is available
            }
            else
            {
                __instance.energyCur.color = Color.white; // Normal color
            }

            int totalPower = Mathf.CeilToInt(currentBatteryPower + currentReservePower);

            __instance.energyCur.text = IntStringCache.GetStringForInt(totalPower);

            NuclearModuleConfig.SetCyclopsMaxPower(__instance.subRoot.powerRelay.GetMaxPower());
        }

        private static int GetTotalReservePower(Equipment modules, AuxUpgradeConsole[] auxUpgradeConsoles)
        {
            float availableReservePower = 0f;

            availableReservePower += GetReserverPowerInEquipment(modules, availableReservePower);

            foreach (AuxUpgradeConsole auxConsole in auxUpgradeConsoles)
                availableReservePower += GetReserverPowerInEquipment(auxConsole.Modules, availableReservePower);

            return Mathf.FloorToInt(availableReservePower);
        }

        private static float GetReserverPowerInEquipment(Equipment modules, float availableReservePower)
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

            return availableReservePower;
        }

        private static int GetLastPowerPercentage(ref CyclopsHelmHUDManager cyclopsHUD)
        {
            return (int)cyclopsHUD.GetPrivateField("lastPowerPctUsedForString");
        }

        private static Battery GetBatteryInSlot(Equipment modules, string slotName)
        {
            // Get the battery component
            InventoryItem item = modules.GetItemInSlot(slotName);
            Battery batteryInSlot = item.item.GetComponent<Battery>();
            return batteryInSlot;
        }

        private static void ChargeCyclopsFromBattery(ref SubRoot cyclops, Battery batteryInSlot, float drainingRate, ref float powerDeficit)
        {
            if (Mathf.Approximately(powerDeficit, 0f)) // No power deficit left to charge
                return; // Exit

            if (Mathf.Approximately(batteryInSlot.charge, 0f)) // The battery has no charge left
                return; // Skip this battery

            // Mathf.Min is to prevent accidentally taking too much power from the battery
            float chargeAmt = Mathf.Min(powerDeficit, drainingRate);

            if (batteryInSlot.charge > chargeAmt)
            {
                batteryInSlot.charge -= chargeAmt;
            }
            else // Battery about to be fully drained
            {
                chargeAmt = batteryInSlot.charge; // Take what's left
                batteryInSlot.charge = 0f; // Set battery to empty                
            }

            powerDeficit -= chargeAmt; // This is to prevent draining more than needed if the power cells were topped up mid-loop

            cyclops.powerRelay.AddEnergy(chargeAmt, out float amtStored);
        }

        private static float ChargeFromModule(ref SubRoot cyclops, float chargeAmount, ref float powerDeficit)
        {
            if (Mathf.Approximately(powerDeficit, 0f))
                return chargeAmount; // Surplus power

            if (Mathf.Approximately(chargeAmount, 0f))
                return 0f;

            cyclops.powerRelay.AddEnergy(chargeAmount, out float amtStored);
            powerDeficit = Mathf.Max(0f, powerDeficit - chargeAmount);

            return Mathf.Max(0f, chargeAmount - powerDeficit); // Surplus power
        }

        private static float ChargeFromModulelMk2(ref SubRoot cyclops, Battery batteryInSlot, float chargeAmount, float batteryDrainRate, ref float powerDeficit)
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

        private static float ChargeCyclopsAndBattery(SubRoot cyclops, Battery batteryInSlot, ref float chargeAmount, ref float powerDeficit)
        {
            chargeAmount *= Mk2ChargeRateModifier;

            cyclops.powerRelay.AddEnergy(chargeAmount, out float amtStored);
            powerDeficit = Mathf.Max(0f, powerDeficit - chargeAmount);

            batteryInSlot.charge = Mathf.Min(batteryInSlot.capacity, batteryInSlot.charge + chargeAmount);

            return Mathf.Max(0f, chargeAmount - powerDeficit); // Surplus power
        }

        /// <summary>
        /// Replaces a nuclear battery modules with Depleted Reactor Rods when they fully drained.
        /// </summary>
        private static void HandleBatteryDepletion(Equipment modules, string slotName, Battery nuclearBattery)
        {
            if (nuclearBattery.charge <= 0f) // Drained nuclear batteries are handled just like how the Nuclear Reactor handles depleated reactor rods
            {
                InventoryItem inventoryItem = modules.RemoveItem(slotName, true, false);
                Object.Destroy(inventoryItem.item.gameObject);
                modules.AddItem(slotName, SpawnDepletedNuclearModule(), true);
            }
        }

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
