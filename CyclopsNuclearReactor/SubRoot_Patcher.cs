namespace CyclopsNuclearReactor
{
    using Harmony;
    using MoreCyclopsUpgrades.API;

    [HarmonyPatch(typeof(SubRoot))]
    [HarmonyPatch(nameof(SubRoot.Start))]
    internal class SubRoot_Start_Postfix
    {
        [HarmonyPostfix]
        public static void Postfix(ref SubRoot __instance)
        {
            if (__instance.isCyclops)
            {
                MCUServices.Find.AuxCyclopsManager<CyNukeManager>(__instance)?.SyncReactorsExternally();
            }
        }
    }
}
