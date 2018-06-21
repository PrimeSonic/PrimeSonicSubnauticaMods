namespace UpgradedVehicles.Patchers
{
    using Harmony;

    [HarmonyPatch(typeof(Vehicle))]
    [HarmonyPatch("OnUpgradeModuleChange")]
    internal class Vehicle_OnUpgradeModuleChange_Patcher
    {
        public static void Postfix(Vehicle __instance)
        {
            VehicleUpgrader.UpgradeVehicle(__instance);
        }        
    }

}
