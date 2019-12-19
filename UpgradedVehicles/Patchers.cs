namespace UpgradedVehicles
{
    using Harmony;
    using UnityEngine;

    [HarmonyPatch(typeof(DamageSystem))]
    [HarmonyPatch(nameof(DamageSystem.CalculateDamage))]
    internal class DamageSystem_CalculateDamage_Patch
    {
        [HarmonyPostfix]
        internal static void Postfix(ref float __result, GameObject target)
        {
            var vehicle = target.GetComponent<Vehicle>();

            if (vehicle != null) // Target is vehicle
            {
                var vehicleUpgrader = VehicleUpgrader.GetUpgrader(vehicle);

                if (vehicleUpgrader == null)
                {
                    return;
                }

                __result = vehicleUpgrader.GeneralDamageReduction * __result;
            }
        }
    }

    [HarmonyPatch(typeof(SeaMoth))]
    [HarmonyPatch(nameof(SeaMoth.OnUpgradeModuleChange))]
    internal class SeaMoth_OnUpgradeModuleChange_Patcher
    {
        [HarmonyPostfix]
        internal static void Postfix(ref SeaMoth __instance, TechType techType)
        {
            VehicleUpgrader.GetUpgrader(__instance)?.UpgradeVehicle(techType);
        }
    }

    [HarmonyPatch(typeof(Exosuit))]
    [HarmonyPatch(nameof(Exosuit.OnUpgradeModuleChange))]
    internal class Exosuit_OnUpgradeModuleChange_Patcher
    {
        [HarmonyPostfix]
        internal static void Postfix(ref Exosuit __instance, TechType techType)
        {
            VehicleUpgrader.GetUpgrader(__instance)?.UpgradeVehicle(techType);
        }
    }
}
