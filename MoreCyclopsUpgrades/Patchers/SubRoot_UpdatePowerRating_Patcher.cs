namespace MoreCyclopsUpgrades.Patchers
{
    using Harmony;

    [HarmonyPatch(typeof(SubRoot))]
    [HarmonyPatch("UpdatePowerRating")]
    internal class SubRoot_UpdatePowerRating_Patcher
    {
        public static bool Prefix(ref SubRoot __instance)
        {
            if (__instance.upgradeConsole == null)
            {
                return true; // mimicing safety conditions from SetCyclopsUpgrades() method in SubRoot
            }

            PowerIndexManager.UpdatePowerIndex(ref __instance);

            return false; // Completely override the method and do not continue with original execution
        }

    }
}
