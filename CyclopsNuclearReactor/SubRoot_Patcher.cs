namespace CyclopsNuclearReactor
{
    using Harmony;

    [HarmonyPatch(typeof(SubRoot))]
    [HarmonyPatch("Awake")]
    internal class SubRoot_Awake
    {
        [HarmonyPostfix]
        public static void Postfix(ref SubRoot __instance)
        {
            if (__instance.isCyclops)
            {
                var mgr = CyNukeChargeManager.GetManager(__instance);
                mgr.SyncReactorsExternally();
            }
        }
    }
}
