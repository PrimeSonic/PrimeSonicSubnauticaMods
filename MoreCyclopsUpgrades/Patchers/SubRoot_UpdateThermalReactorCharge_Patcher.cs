namespace MoreCyclopsUpgrades
{
    using Harmony;

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

            float solarChargeAmount = SolarChargingManager.GetSolarChargeAmount(ref __instance);

            float thermalChargeAmount = ThermalChargingManager.GetThermalChargeAmount(ref __instance);

            foreach (string slotName in SlotHelper.SlotNames)
            {
                TechType techTypeInSlot = modules.GetTechTypeInSlot(slotName);

                if (techTypeInSlot == SolarCharger.CySolarChargerTechType) // Solar
                {
                    PowerChargingManager.ChargeFromModule(ref __instance, solarChargeAmount, ref powerDeficit);
                }
                else if (techTypeInSlot == SolarChargerMk2.SolarMk2TechType) // Solar Mk2
                {
                    Battery battery = PowerChargingManager.GetBatteryInSlot(modules, slotName);
                    PowerChargingManager.ChargeFromModulelMk2(ref __instance, battery, solarChargeAmount, SolarChargingManager.BatteryDrainRate, ref powerDeficit);                    
                }
                else if(techTypeInSlot == TechType.CyclopsThermalReactorModule) // Thermal
                {
                    PowerChargingManager.ChargeFromModule(ref __instance, solarChargeAmount, ref powerDeficit);
                }
                else if (techTypeInSlot == ThermalChargerMk2.ThermalMk2TechType) // Thermal Mk2
                {
                    Battery battery = PowerChargingManager.GetBatteryInSlot(modules, slotName);
                    PowerChargingManager.ChargeFromModulelMk2(ref __instance, battery, solarChargeAmount, ThermalChargingManager.BatteryDrainRate, ref powerDeficit);
                }
                else if (techTypeInSlot == NuclearCharger.CyNukBatteryType) // Nuclear
                {
                    if (!cyclopsHasPowerCells) // Just to be safe, we won't drain the nuclear batteries if there's a chance that all powercells were removed.
                        continue; // Nuclear power cells don't recharge.

                    Battery battery = PowerChargingManager.GetBatteryInSlot(modules, slotName);
                    BatteryState batteryState = PowerChargingManager.DrainBattery(ref __instance, battery, NuclearChargingManager.BatteryDrainRate, ref powerDeficit);
                    NuclearChargingManager.HandleDepletedBattery(modules, slotName, batteryState);
                }
            }

            return false; // No need to execute original method anymore
        }
    }
}
