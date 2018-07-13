namespace UpgradedVehicles.Patchers
{
    using Harmony;
    using UnityEngine;

    [HarmonyPatch(typeof(Exosuit))]
    [HarmonyPatch("OnUpgradeModuleChange")]
    internal class Exosuit_OnUpgradeModuleChange_Patcher
    {
        [HarmonyPostfix]
        internal static void Postfix(ref Exosuit __instance, TechType techType)
        {
            VehicleUpgrader.UpgradeExosuit(__instance, techType);
        }
    }

    [HarmonyPatch(typeof(Exosuit))]
    [HarmonyPatch("Awake")]
    internal class Exosuit_Awake_Patcher
    {
        [HarmonyPostfix]
        internal static void Postfix(ref Exosuit __instance)
        {
            VehicleUpgrader.UpgradeExosuit(__instance);
        }
    }

    [HarmonyPatch(typeof(Exosuit))]
    [HarmonyPatch("vehicleDefaultName", PropertyMethod.Getter)]
    internal class Exosuit_vehicleDefaultName_Patcher
    {
        [HarmonyPrefix]
        internal static bool Prefix(ref Exosuit __instance, ref string __result)
        {
            var techType = __instance.GetComponent<TechTag>().type;

            if (techType == ExosuitMk2.TechTypeID)
            {
                __result = "PRAWN SUIT MK2";
                return false;
            }

            return true;
        }

    }

    [HarmonyPatch(typeof(Exosuit))]
    [HarmonyPatch("vehicleDefaultColors", PropertyMethod.Getter)]
    internal class Exosuit_vehicleDefaultColors_Patcher
    {
        [HarmonyPrefix]
        internal static bool Prefix(ref Exosuit __instance, ref Vector3[] __result)
        {
            bool isUpgradedSeamoth = __instance.GetComponent<TechTag>().type == ExosuitMk2.TechTypeID;

            if (isUpgradedSeamoth)
            {
                __result = VehicleUpgrader.UpgradedVehicleColors;
                return false;
            }

            return true;
        }

    }

}
