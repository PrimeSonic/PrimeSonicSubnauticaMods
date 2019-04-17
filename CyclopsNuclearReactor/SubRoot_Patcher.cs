namespace CyclopsNuclearReactor
{
    using Harmony;

    [HarmonyPatch(typeof(SubRoot))]
    [HarmonyPatch("OnPlayerEntered")]
    internal class SubRoot_OnPlayerEntered_BeQuiet
    {        
        [HarmonyPostfix]
        public static void Postfix(ref SubRoot __instance)
        {
            var mgr = CyNukeChargeManager.GetManager(__instance);
            mgr.SyncReactorsExternally();
        }
    }
}
