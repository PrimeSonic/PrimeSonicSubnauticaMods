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
            VehicleUpgrader.GetUpgrader(__instance)?.UpgradeVehicle(techType);
        }
    }
}
