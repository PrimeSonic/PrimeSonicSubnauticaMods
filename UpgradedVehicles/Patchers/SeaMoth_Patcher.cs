namespace UpgradedVehicles.Patchers
{
    using System;
    using System.Collections.Generic;
    using System.Reflection.Emit;
    using Harmony;
    using UnityEngine;

    [HarmonyPatch(typeof(SeaMoth))]
    [HarmonyPatch("OnUpgradeModuleChange")]
    internal class SeaMoth_OnUpgradeModuleChange_Patcher
    {
        [HarmonyPostfix]
        internal static void Postfix(ref SeaMoth __instance)
        {
            VehicleUpgrader.UpgradeSeaMoth(__instance);
        }
    }

    [HarmonyPatch(typeof(SeaMoth))]
    [HarmonyPatch("Awake")]
    internal class SeaMoth_Awake_Patcher
    {
        [HarmonyPostfix]
        internal static void Postfix(ref SeaMoth __instance)
        {
            VehicleUpgrader.UpgradeSeaMoth(__instance);
            VehicleUpgrader.UpgradeVehicle(__instance);
        }
    }

    [HarmonyPatch(typeof(SeaMoth))]
    [HarmonyPatch("Start")]
    internal class SeaMoth_Start_Patcher
    {
        [HarmonyPostfix]
        internal static void Postfix(ref SeaMoth __instance)
        {
            VehicleUpgrader.UpgradeSeaMoth(__instance);
            VehicleUpgrader.UpgradeVehicle(__instance);
        }
    }

    [HarmonyPatch(typeof(SeaMoth))]
    [HarmonyPatch("vehicleDefaultName", PropertyMethod.Getter)]
    internal class SeaMoth_vehicleDefaultName_Patcher
    {
        [HarmonyPrefix]
        internal static bool Prefix(ref SeaMoth __instance, ref string __result)
        {
            bool isSeamothMk2 = __instance.GetComponentInChildren<PrefabIdentifier>().ClassId == SeaMothMk2.NameID;
#if DEBUG
            Console.WriteLine($"[UpgradedVehicles] vehicleDefaultName : isSeamothMk2:{isSeamothMk2}");
#endif
            if (isSeamothMk2)
            {
                __result = "SEAMOTH MK2";
                return false;
            }

            return true;
        }

    }

    [HarmonyPatch(typeof(SeaMoth))]
    [HarmonyPatch("vehicleDefaultColors", PropertyMethod.Getter)]
    internal class SeaMoth_vehicleDefaultColors_Patcher
    {
        [HarmonyPrefix]
        internal static bool Prefix(ref SeaMoth __instance, ref Vector3[] __result)
        {
            bool isSeamothMk2 = __instance.GetComponentInChildren<PrefabIdentifier>().ClassId == SeaMothMk2.NameID;
            Console.WriteLine($"[UpgradedVehicles] vehicleDefaultName : isSeamothMk2:{isSeamothMk2}");

            if (isSeamothMk2)
            {
                __result = new Vector3[]
                            {
                                new Vector3(1f, 1f, 1f),
                                new Vector3(0f, 0f, 1f),
                                new Vector3(1f, 1f, 1f),
                                new Vector3(0.577f, 0.447f, 0.604f),
                                new Vector3(0.114f, 0.729f, 0.965f)
                            };
                return false;
            }

            return true;
        }

    }

}
