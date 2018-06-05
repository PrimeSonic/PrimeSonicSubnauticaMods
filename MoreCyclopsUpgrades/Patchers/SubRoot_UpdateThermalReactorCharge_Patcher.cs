namespace MoreCyclopsUpgrades
{
    using Harmony;

    [HarmonyPatch(typeof(SubRoot))]
    [HarmonyPatch("UpdateThermalReactorCharge")]
    internal class SubRoot_UpdateThermalReactorCharge_Patcher
    {
        public static void Postfix(ref SubRoot __instance)
        {
            if (__instance.upgradeConsole == null)
            {
                return; // mimicing safety conditions from SetCyclopsUpgrades() method in SubRoot
            }

            bool cyclopsHasPowerCells = __instance.powerRelay.GetPowerStatus() == PowerSystem.Status.Normal;

            Equipment modules = __instance.upgradeConsole.modules;

            float powerDeficit = __instance.powerRelay.GetMaxPower() - __instance.powerRelay.GetPower();

            float solarChargeAmount = SolarChargingManager.GetSolarChargeAmount(ref __instance);

            foreach (string slotName in SlotHelper.SlotNames)
            {
                TechType techTypeInSlot = modules.GetTechTypeInSlot(slotName);

                if (techTypeInSlot == SolarCharger.CySolarChargerTechType)
                {
                    SolarChargingManager.AddSolarCharge(ref __instance, solarChargeAmount, ref powerDeficit);
                }
                else if (techTypeInSlot == SolarChargerMk2.CySolarMk2TechType)
                {
                    SolarChargingManager.AddSolarChargeMk2(ref __instance, solarChargeAmount, ref powerDeficit);
                    SolarChargingManager.ChargeSolarBattery(modules, slotName, solarChargeAmount);
                }
                else if (techTypeInSlot == NuclearCharger.CyNukBatteryType)
                {
                    if (!cyclopsHasPowerCells) // Just to be safe, we won't drain the nuclear batteries if there's a chance that all powercells were removed
                        continue;

                    NuclearChargingManager.DrainNuclearBatteries(ref __instance, modules, slotName, ref powerDeficit);
                }
            }
        }
    }
}
