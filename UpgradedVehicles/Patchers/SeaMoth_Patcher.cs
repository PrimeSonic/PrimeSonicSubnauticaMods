namespace UpgradedVehicles.Patchers
{
    using System;
    using System.Collections.Generic;
    using System.Reflection.Emit;
    using Harmony;

    [HarmonyPatch(typeof(SeaMoth))]
    [HarmonyPatch("OnUpgradeModuleChange")]
    internal class SeaMoth_OnUpgradeModuleChange_Patcher
    {
        internal static void Postfix(ref SeaMoth __instance)
        {
            VehicleUpgrader.UpgradeSeaMoth(__instance);
        }
    }

    [HarmonyPatch(typeof(SeaMoth))]
    [HarmonyPatch("Awake")]
    internal class SeaMoth_Awake_Patcher
    {
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
        internal static bool Prefix(ref SeaMoth __instance, ref string __result)
        {
            bool isSeamothMk2 = __instance.GetComponentInChildren<PrefabIdentifier>().ClassId == SeaMothMk2.NameID;
            Console.WriteLine($"[UpgradedVehicles] vehicleDefaultName : isSeamothMk2:{isSeamothMk2}");

            if (isSeamothMk2)
            {
                __result = "SEAMOTH MK2";
                return false;
            }

            return true;
        }

    }

}
