namespace MoreCyclopsUpgrades
{
    using Harmony;

    [HarmonyPatch(typeof(SubRoot))]
    [HarmonyPatch("UpdateThermalReactorCharge")]
    internal class SubRoot_UpdateThermalReactorCharge_Patcher
    {
        /// <summary>
        /// This patch method handles actually providing charge to the Cyclops 
        /// while the <see cref="NuclearChargingManager"/> tracks the nuclear battery charge.
        /// </summary>        
        public static void Postfix(ref SubRoot __instance)
        {
            if (__instance.upgradeConsole == null)
            {
                return; // mimicing safety conditions from SetCyclopsUpgrades() method in SubRoot
            }

            SolarChargingManager.UpdateSolarCharger(ref __instance);

            bool cyclopsHasPowerCells = __instance.powerRelay.GetPowerStatus() == PowerSystem.Status.Normal;

            if (!cyclopsHasPowerCells)
            {
                // Don't drain if there are no batteries to charge
                // Potential issue if the player somehow managed to actually drain all their power cells
                return;
            }            

            NuclearChargingManager.UpdateNuclearBatteryCharges(ref __instance);
        }
    }
}
