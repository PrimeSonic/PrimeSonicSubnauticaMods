namespace CyclopsNuclearReactor
{
    using Harmony;
    using MoreCyclopsUpgrades.API;

    [HarmonyPatch(typeof(SubRoot))]
    [HarmonyPatch("Awake")]
    internal class SubRoot_Awake
    {
        [HarmonyPostfix]
        public static void Postfix(ref SubRoot __instance)
        {
            if (__instance.isCyclops)
            {
                MCUServices.Find.CyclopsCharger<CyNukeChargeManager>(__instance)?.SyncReactorsExternally();
            }
        }
    }
}
