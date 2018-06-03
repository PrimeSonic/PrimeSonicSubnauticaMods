namespace MoreCyclopsUpgrades.Patchers
{
    using Harmony;

    [HarmonyPatch(typeof(SubRoot))]
    [HarmonyPatch("UpdatePowerRating")]
    class SubRoot_UpdatePowerRating_Patcher
    {
        public static void Postfix(ref SubRoot __instance)
        {
            if (__instance.upgradeConsole == null)
            {
                return; // mimicing safety conditions from SetCyclopsUpgrades() method in SubRoot
            }

            PowerIndexManager.UpdatePowerIndex(ref __instance);
        }
    }
}
