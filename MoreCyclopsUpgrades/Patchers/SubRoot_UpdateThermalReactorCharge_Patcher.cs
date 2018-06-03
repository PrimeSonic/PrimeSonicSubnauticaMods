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

            // Solar charging is safe even if batteries missing
            SolarChargingManager.UpdateSolarCharger(ref __instance);

            bool cyclopsHasPowerCells = __instance.powerRelay.GetPowerStatus() == PowerSystem.Status.Normal;

            if (!cyclopsHasPowerCells)
            {
                // Just to be safe, we won't drain the nuclear batteries if there's a chance that all powercells were removed
                return;
            }

            NuclearChargingManager.UpdateNuclearBatteryCharges(ref __instance);
        }
    }
}
