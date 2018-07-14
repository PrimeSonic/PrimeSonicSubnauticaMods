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
