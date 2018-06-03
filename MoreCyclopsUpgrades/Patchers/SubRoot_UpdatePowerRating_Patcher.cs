namespace MoreCyclopsUpgrades.Patchers
{
    using Harmony;

    [HarmonyPatch(typeof(SubRoot))]
    [HarmonyPatch("UpdatePowerRating")]
    class SubRoot_UpdatePowerRating_Patcher
    {
        public static void Postfix(ref SubRoot __instance)
        {
            PowerIndexManager.UpdatePowerIndex(ref __instance);
        }
    }
}
