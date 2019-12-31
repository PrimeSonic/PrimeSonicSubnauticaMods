namespace UpgradedVehicles
{
    using Common;
    using Harmony;
    using UnityEngine;

    [HarmonyPatch(typeof(DamageSystem))]
    [HarmonyPatch(nameof(DamageSystem.CalculateDamage))]
    internal class DamageSystem_CalculateDamage_Patch
    {
        [HarmonyPostfix]
        internal static void Postfix(ref float __result, GameObject target)
        {
            Vehicle vehicle = target.GetComponent<Vehicle>();

            if (vehicle != null) // Target is vehicle
            {
                VehicleUpgrader vehicleUpgrader = vehicle.gameObject.EnsureComponent<VehicleUpgrader>();

                __result = vehicleUpgrader.GeneralDamageReduction * __result;
            }
        }
    }

    [HarmonyPatch(typeof(SeaMoth))]
    [HarmonyPatch(nameof(SeaMoth.Start))]
    internal class SeaMoth_Start_Patcher
    {
        [HarmonyPrefix]
        internal static void Prefix(ref SeaMoth __instance)
        {
            QuickLogger.Debug("SeaMoth_Start_Patcher");
            VehicleUpgrader vehicleUpgrader = __instance.gameObject.EnsureComponent<VehicleUpgrader>();
            vehicleUpgrader.Initialize(ref __instance);
        }
    }

    [HarmonyPatch(typeof(Exosuit))]
    [HarmonyPatch(nameof(Exosuit.Start))]
    internal class Exosuit_Start_Patcher
    {
        [HarmonyPrefix]
        internal static void Prefix(ref Exosuit __instance)
        {
            QuickLogger.Debug("Exosuit_Start_Patcher");
            VehicleUpgrader vehicleUpgrader = __instance.gameObject.EnsureComponent<VehicleUpgrader>();
            vehicleUpgrader.Initialize(ref __instance);
        }
    }

    [HarmonyPatch(typeof(SeaMoth))]
    [HarmonyPatch(nameof(SeaMoth.OnUpgradeModuleChange))]
    internal class SeaMoth_OnUpgradeModuleChange_Patcher
    {
        [HarmonyPostfix]
        internal static void Postfix(ref SeaMoth __instance, TechType techType)
        {
            QuickLogger.Debug("SeaMoth_OnUpgradeModuleChange_Patcher");
            __instance.gameObject.EnsureComponent<VehicleUpgrader>().UpgradeVehicle(techType, ref __instance);
        }
    }

    [HarmonyPatch(typeof(Exosuit))]
    [HarmonyPatch(nameof(Exosuit.OnUpgradeModuleChange))]
    internal class Exosuit_OnUpgradeModuleChange_Patcher
    {
        [HarmonyPostfix]
        internal static void Postfix(ref Exosuit __instance, TechType techType)
        {
            QuickLogger.Debug("Exosuit_OnUpgradeModuleChange_Patcher");
            __instance.gameObject.EnsureComponent<VehicleUpgrader>().UpgradeVehicle(techType, ref __instance);
        }
    }
}
