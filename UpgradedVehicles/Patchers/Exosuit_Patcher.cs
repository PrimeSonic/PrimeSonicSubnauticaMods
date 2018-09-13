namespace UpgradedVehicles.Patchers
{
    using Harmony;

    [HarmonyPatch(typeof(Exosuit))]
    [HarmonyPatch("OnUpgradeModuleChange")]
    internal class Exosuit_OnUpgradeModuleChange_Patcher
    {
        [HarmonyPostfix]
        internal static void Postfix(ref Exosuit __instance, TechType techType)
        {
            __instance.GetComponent<VehicleUpgrader>().UpgradeVehicle(techType);
        }
    }

    [HarmonyPatch(typeof(Exosuit))]
    [HarmonyPatch("Awake")]
    internal class Exosuit_Awake_Patcher
    {
        [HarmonyPostfix]
        internal static void Postfix(ref Exosuit __instance)
        {
            if (__instance.GetComponent<VehicleUpgrader>() == null)
            {
                var vUpgrader = __instance.gameObject.AddComponent<VehicleUpgrader>();
                vUpgrader.Initialize(__instance);
            }
        }
    }

}
