namespace MoreCyclopsUpgrades
{
    using Harmony;
    using UnityEngine;
    using System.Collections.Generic;
    using System;

    [HarmonyPatch(typeof(SubRoot))]
    [HarmonyPatch("UpdateThermalReactorCharge")]
    internal class SubRoot_UpdateThermalReactorCharge_Patcher
    {

        public static bool Prefix(ref SubRoot __instance)
        {
            if (__instance.upgradeConsole == null)
            {
                return true;
            }

            Equipment modules = __instance.upgradeConsole.modules;

            float powerDeficit = __instance.powerRelay.GetMaxPower() - __instance.powerRelay.GetPower();
            float origPowerDeficit = powerDeficit;

            float availableSolarEnergy = SolarChargingManager.GetSolarChargeAmount(ref __instance);

            float availableThermalEnergy = ThermalChargingManager.GetThermalChargeAmount(ref __instance);

            float surplusPower = 0f;
            Battery lastBatteryToCharge = null;
            var nuclearCells = new List<Battery>(6);
            var nuclearSlots = new List<string>(6);

            bool renewablePowerAvailable = false;

            foreach (string slotName in SlotHelper.SlotNames)
            {
                TechType techTypeInSlot = modules.GetTechTypeInSlot(slotName);

                if (techTypeInSlot == SolarCharger.CySolarChargerTechType) // Solar
                {
                    surplusPower += PowerCharging.ChargeFromModule(ref __instance, availableSolarEnergy, ref powerDeficit);
                    renewablePowerAvailable |= availableSolarEnergy > 0f;
                }
                else if (techTypeInSlot == SolarChargerMk2.SolarMk2TechType) // Solar Mk2
                {
                    Battery battery = PowerCharging.GetBatteryInSlot(modules, slotName);
                    surplusPower += PowerCharging.ChargeFromModulelMk2(ref __instance, battery, availableSolarEnergy, SolarChargingManager.BatteryDrainRate, ref powerDeficit);
                    renewablePowerAvailable |= battery.charge > 0f;

                    if (battery.charge < battery.capacity)
                        lastBatteryToCharge = battery;                    
                }
                else if (techTypeInSlot == TechType.CyclopsThermalReactorModule) // Thermal
                {
                    surplusPower += PowerCharging.ChargeFromModule(ref __instance, availableThermalEnergy, ref powerDeficit);
                    renewablePowerAvailable |= availableThermalEnergy > 0f;
                }
                else if (techTypeInSlot == ThermalChargerMk2.ThermalMk2TechType) // Thermal Mk2
                {
                    Battery battery = PowerCharging.GetBatteryInSlot(modules, slotName);
                    surplusPower += PowerCharging.ChargeFromModulelMk2(ref __instance, battery, availableThermalEnergy, ThermalChargingManager.BatteryDrainRate, ref powerDeficit);
                    renewablePowerAvailable |= battery.charge > 0f;

                    if (battery.charge < battery.capacity)
                        lastBatteryToCharge = battery;
                }
                else if (techTypeInSlot == NuclearCharger.CyNukBatteryType) // Nuclear
                {
                    Battery battery = PowerCharging.GetBatteryInSlot(modules, slotName);
                    nuclearCells.Add(battery);
                    nuclearSlots.Add(slotName);
                }
            }
            
            if (nuclearCells.Count > 0 && powerDeficit > 0f && !renewablePowerAvailable) // no renewable power available
            {
                // We'll only charge from the nuclear cells if we aren't getting power from the other modules.
                for (int i = 0; i < nuclearCells.Count; i++)
                {
                    Battery battery = nuclearCells[i];
                    string slotName = nuclearSlots[i];
                    PowerCharging.ChargeCyclopsFromBattery(ref __instance, battery, NuclearChargingManager.BatteryDrainRate, ref powerDeficit);
                    NuclearChargingManager.HandleBatteryDepletion(modules, slotName, battery);
                }
            }

            if (powerDeficit <= 0f && surplusPower > 0f && lastBatteryToCharge != null)
            {
                // Recycle surplus power back into the batteries that need it
                lastBatteryToCharge.charge = Mathf.Min(lastBatteryToCharge.capacity, lastBatteryToCharge.charge + surplusPower);
            }

            // No need to execute original method anymore.
            __instance.thermalReactorUpgrade = false;
            // Original thermal charging is handled in here now.
            return true;
        }
    }
}
