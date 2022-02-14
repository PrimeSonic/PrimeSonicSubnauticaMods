namespace UpgradedVehicles
{
    using System.Collections;
    using Common;
    using HarmonyLib;
    using UnityEngine;
    using UWE;

    [HarmonyPatch(typeof(DamageSystem))]
    [HarmonyPatch(nameof(DamageSystem.CalculateDamage))]
    internal class DamageSystem_CalculateDamage_Patch
    {
        [HarmonyPostfix]
        internal static void Postfix(ref float __result, GameObject target)
        {
            MonoBehaviour vehicle = target.GetComponent<Vehicle>();
#if BELOWZERO
            if (vehicle == null)
                vehicle = target.GetComponent<SeaTruckMotor>();
#endif
            if (vehicle != null) // Target is vehicle
            {
                VehicleUpgrader vehicleUpgrader = vehicle.gameObject.EnsureComponent<VehicleUpgrader>();

                __result = vehicleUpgrader.GeneralDamageReduction * __result;
            }
        }
    }

    [HarmonyPatch]
    internal class VehiclePatches
    {
#if SUBNAUTICA
        [HarmonyPatch(typeof(SeaMoth))]
        [HarmonyPatch(nameof(SeaMoth.Awake))]
        [HarmonyPrefix]
        internal static void PreSeamothAwake(ref SeaMoth __instance)
        {
            QuickLogger.Debug(nameof(PreSeamothAwake));
            VehicleUpgrader vehicleUpgrader = __instance.gameObject.EnsureComponent<VehicleUpgrader>();
            vehicleUpgrader.Initialize(ref __instance);
        }

#elif BELOWZERO
        [HarmonyPatch(typeof(SeaTruckMotor))]
        [HarmonyPatch(nameof(SeaTruckMotor.Start))]
        [HarmonyPostfix]
        internal static void PostSeatruckStart(ref SeaTruckMotor __instance)
        {
            QuickLogger.Debug(nameof(PostSeatruckStart));
            VehicleUpgrader vehicleUpgrader = __instance.gameObject.EnsureComponent<VehicleUpgrader>();
            vehicleUpgrader.Initialize(ref __instance);
        }
#endif

        [HarmonyPatch(typeof(Exosuit))]
        [HarmonyPatch(nameof(Exosuit.Awake))]
        [HarmonyPrefix]
        internal static void PreExosuitAwake(ref Exosuit __instance)
        {
            QuickLogger.Debug(nameof(PreExosuitAwake));
            VehicleUpgrader vehicleUpgrader = __instance.gameObject.EnsureComponent<VehicleUpgrader>();
            vehicleUpgrader.Initialize(ref __instance);
        }

#if SUBNAUTICA
        [HarmonyPatch(typeof(SeaMoth))]
        [HarmonyPatch(nameof(SeaMoth.OnUpgradeModuleChange))]
        [HarmonyPriority(Priority.Last)]
        [HarmonyPostfix]
        internal static void PostSeamothUpgradeChange(ref SeaMoth __instance, TechType techType)
        {
            QuickLogger.Debug($"{nameof(PostSeamothUpgradeChange)} {techType.AsString()}");
            //__instance.gameObject.EnsureComponent<VehicleUpgrader>().UpgradeVehicle(techType, ref __instance);
            CoroutineHost.StartCoroutine(DeferUpgrade(__instance, techType));
        }

#elif BELOWZERO
        [HarmonyPatch(typeof(SeaTruckUpgrades))]
        [HarmonyPatch(nameof(SeaTruckUpgrades.OnUpgradeModuleChange))]
        [HarmonyPriority(Priority.Last)]
        [HarmonyPostfix]
        internal static void PostSeaTruckUpgradesModuleChange(ref SeaTruckUpgrades __instance, TechType techType)
        {
            QuickLogger.Debug($"{nameof(PostSeaTruckUpgradesModuleChange)} {techType.AsString()}", true);
            SeaTruckMotor cab = __instance.motor;
            //cab.gameObject.EnsureComponent<VehicleUpgrader>().UpgradeVehicle(techType, ref cab);
            CoroutineHost.StartCoroutine(DeferUpgrade(cab, techType));
        }

#endif

        [HarmonyPatch(typeof(Exosuit))]
        [HarmonyPatch(nameof(Exosuit.OnUpgradeModuleChange))]
        [HarmonyPriority(Priority.Last)]
        [HarmonyPostfix]
        internal static void PostExosuitOnUpgradeModuleChange(ref Exosuit __instance, TechType techType)
        {
            QuickLogger.Debug($"{nameof(PostExosuitOnUpgradeModuleChange)} {techType.AsString()}", true);
            //__instance.gameObject.EnsureComponent<VehicleUpgrader>().UpgradeVehicle(techType, ref __instance);
            CoroutineHost.StartCoroutine(DeferUpgrade(__instance, techType));
        }

        private static IEnumerator DeferUpgrade(MonoBehaviour vehicleInstance, TechType techType)
        {
            yield return new WaitForSecondsRealtime(0.1f);

            vehicleInstance.gameObject.EnsureComponent<VehicleUpgrader>().UpgradeVehicle(techType, ref vehicleInstance);
            yield break;
        }
    }
}
