namespace UpgradedVehicles.Patchers
{
    using Harmony;

    [HarmonyPatch(typeof(Vehicle))]
    [HarmonyPatch("OnUpgradeModuleChange")]
    internal class Vehicle_OnUpgradeModuleChange_Patcher
    {
        [HarmonyPostfix]
        internal static void Postfix(Vehicle __instance, TechType techType)
        {
            VehicleUpgrader.UpgradeVehicle(__instance, techType);
        }
    }
}
