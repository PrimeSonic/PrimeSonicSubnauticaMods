namespace MoreCyclopsUpgrades
{
    using Harmony;
    using UnityEngine;

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

            bool cyclopsHasPowerCells = __instance.powerRelay.GetPowerStatus() == PowerSystem.Status.Normal;

            Equipment modules = __instance.upgradeConsole.modules;

            float powerDeficit = __instance.powerRelay.GetMaxPower() - __instance.powerRelay.GetPower();

            float availableSolarEnergy = SolarChargingManager.GetSolarChargeAmount(ref __instance);

            float availableThermalEnergy = ThermalChargingManager.GetThermalChargeAmount(ref __instance);

            float surplusPower = 0f;
            Battery lastBatteryToCharge = null;

            foreach (string slotName in SlotHelper.SlotNames)
            {
                TechType techTypeInSlot = modules.GetTechTypeInSlot(slotName);

                if (techTypeInSlot == SolarCharger.CySolarChargerTechType) // Solar
                {
                    surplusPower += PowerCharging.ChargeFromModule(ref __instance, availableSolarEnergy, ref powerDeficit);
                }
                else if (techTypeInSlot == SolarChargerMk2.SolarMk2TechType) // Solar Mk2
                {
                    Battery battery = PowerCharging.GetBatteryInSlot(modules, slotName);
                    surplusPower += PowerCharging.ChargeFromModulelMk2(ref __instance, battery, availableSolarEnergy, SolarChargingManager.BatteryDrainRate, ref powerDeficit);
                    if (battery.charge < battery.capacity)
                        lastBatteryToCharge = battery;
                }
                else if (techTypeInSlot == TechType.CyclopsThermalReactorModule) // Thermal
                {
                    surplusPower += PowerCharging.ChargeFromModule(ref __instance, availableThermalEnergy, ref powerDeficit);
                }
                else if (techTypeInSlot == ThermalChargerMk2.ThermalMk2TechType) // Thermal Mk2
                {
                    Battery battery = PowerCharging.GetBatteryInSlot(modules, slotName);
                    surplusPower += PowerCharging.ChargeFromModulelMk2(ref __instance, battery, availableThermalEnergy, ThermalChargingManager.BatteryDrainRate, ref powerDeficit);
                    if (battery.charge < battery.capacity)
                        lastBatteryToCharge = battery;
                }
                else if (techTypeInSlot == NuclearCharger.CyNukBatteryType) // Nuclear
                {
                    if (!cyclopsHasPowerCells) // Just to be safe, we won't drain the nuclear batteries if there's a chance that all powercells were removed.
                        continue; // Nuclear power cells don't recharge.

                    Battery battery = PowerCharging.GetBatteryInSlot(modules, slotName);
                    PowerCharging.ChargeCyclopsFromBattery(ref __instance, battery, NuclearChargingManager.BatteryDrainRate, ref powerDeficit);
                    NuclearChargingManager.HandleBatteryDepletion(modules, slotName, battery);
                }
            }

            if (powerDeficit <= 0f && surplusPower > 0f && lastBatteryToCharge != null)
            {
                // Recycle surplus power back into the batteries that need it
                lastBatteryToCharge.charge = Mathf.Min(lastBatteryToCharge.capacity, lastBatteryToCharge.charge + surplusPower);
            }

            return false; // No need to execute original method anymore
        }
    }
}
