namespace CyclopsNuclearPower
{
    using Harmony;

    [HarmonyPatch(typeof(SubRoot))]
    [HarmonyPatch("SetCyclopsUpgrades")]
    internal class SubRoot_SetCyclopsUpgrades_Patcher
    {
        /// <summary>
        /// This patch method handles shuffling the nuclear batteries into and out of the <see cref="NuclearBatteryManager"/>.
        /// </summary>        
        public static void Postfix(ref SubRoot __instance)
        {
            if (__instance.upgradeConsole == null)
            {
                return; // mimicing safety conditions from SetCyclopsUpgrades() method in SubRoot
            }

            NuclearBatteryManager.SetNuclearBatterySlots(ref __instance);
        }        
    }
}
