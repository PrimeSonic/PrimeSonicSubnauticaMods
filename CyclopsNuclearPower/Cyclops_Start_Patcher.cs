namespace CyclopsNuclearPower
{
    using Harmony;

    [HarmonyPatch(typeof(SubRoot))]
    [HarmonyPatch("Start")]
    internal class Cyclops_Start_Patcher
    {
        /// <summary>
        /// This repeats the logic of SetCyclopsUpgrades on Awake 
        /// to handle nuclear batteries that were left in the console between saves.
        /// </summary>
        /// <param name="__instance"></param>
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
