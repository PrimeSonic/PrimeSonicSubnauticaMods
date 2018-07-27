namespace UpgradedVehicles.Patchers
{
    using Harmony;
    using UnityEngine;

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

    [HarmonyPatch(typeof(Vehicle))]
    [HarmonyPatch("Awake")]
    internal class Vehicle_Awake_Patcher
    {
        [HarmonyPostfix]
        internal static void Postfix(ref Vehicle __instance)
        {
            VehicleUpgrader.UpgradeVehicle(__instance);
        }
    }  
}
