namespace UpgradedVehicles.Patchers
{
    using Harmony;
    using UnityEngine;

    [HarmonyPatch(typeof(SeaMoth))]
    [HarmonyPatch("OnUpgradeModuleChange")]
    internal class SeaMoth_OnUpgradeModuleChange_Patcher
    {
        [HarmonyPostfix]
        internal static void Postfix(ref SeaMoth __instance, TechType techType)
        {
            VehicleUpgrader.UpgradeSeaMoth(__instance, techType);
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
        }
    }

    [HarmonyPatch(typeof(SeaMoth))]
    [HarmonyPatch("vehicleDefaultName", PropertyMethod.Getter)]
    internal class SeaMoth_vehicleDefaultName_Patcher
    {
        [HarmonyPrefix]
        internal static bool Prefix(ref SeaMoth __instance, ref string __result)
        {
            var techType = __instance.GetComponent<TechTag>().type;

            if (techType == SeaMothMk2.TechTypeID)
            {
                __result = "SEAMOTH MK2";
                return false;
            }
            else if (techType == SeaMothMk3.TechTypeID)
            {
                __result = "SEAMOTH MK3";
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
            TechType seamothType = __instance.GetComponent<TechTag>().type;

            bool isUpgradedSeamoth = 
                seamothType == SeaMothMk2.TechTypeID ||
                seamothType == SeaMothMk3.TechTypeID;

            if (isUpgradedSeamoth)
            {
                __result = VehicleUpgrader.UpgradedVehicleColors;
                return false;
            }

            return true;
        }

    }

}
