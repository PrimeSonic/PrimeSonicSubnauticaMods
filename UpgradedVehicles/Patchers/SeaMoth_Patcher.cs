namespace UpgradedVehicles.Patchers
{
    using Harmony;

    [HarmonyPatch(typeof(SeaMoth))]
    [HarmonyPatch("OnUpgradeModuleChange")]
    internal class SeaMoth_OnUpgradeModuleChange_Patcher
    {
        [HarmonyPostfix]
        internal static void Postfix(ref SeaMoth __instance, TechType techType)
        {
            __instance.GetComponent<VehicleUpgrader>().UpgradeVehicle(techType);
        }
    }

    [HarmonyPatch(typeof(SeaMoth))]
    [HarmonyPatch("Awake")]
    internal class SeaMoth_Awake_Patcher
    {
        [HarmonyPostfix]
        internal static void Postfix(ref SeaMoth __instance)
        {
            if (__instance.GetComponent<VehicleUpgrader>() == null)
            {
                var vUpgrader = __instance.gameObject.AddComponent<VehicleUpgrader>();
                vUpgrader.Initialize(__instance);
            }
        }
    }

}
