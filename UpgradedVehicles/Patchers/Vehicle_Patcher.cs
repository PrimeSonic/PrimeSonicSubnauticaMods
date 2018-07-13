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

    [HarmonyPatch(typeof(Vehicle))]
    [HarmonyPatch("vehicleDefaultColors", PropertyMethod.Getter)]
    internal class Vehicle_vehicleDefaultColors_Patcher
    {
        [HarmonyPrefix]
        internal static bool Prefix(ref Vehicle __instance, ref Vector3[] __result)
        {            
            if (VehicleUpgrader.IsUpgradedVehicle(__instance))
            {
                __result = VehicleUpgrader.UpgradedVehicleColors;
                return false;
            }

            return true;
        }

    }

    [HarmonyPatch(typeof(Vehicle))]
    [HarmonyPatch("EnterVehicle")]
    internal class Vehicle_EnterVehicle_Patcher
    {
        [HarmonyPostfix]
        internal static void Postfix(ref Vehicle __instance)
        {
            var seamoth = __instance.GetComponentInParent<SeaMoth>();
            var exosuit = __instance.GetComponentInParent<Exosuit>();

            if (seamoth != null)
            {
                VehicleUpgrader.UpgradeSeaMoth(seamoth);
            }
            else if (exosuit != null)
            {
                VehicleUpgrader.UpgradeExosuit(exosuit);
            }
        }
    }
}
