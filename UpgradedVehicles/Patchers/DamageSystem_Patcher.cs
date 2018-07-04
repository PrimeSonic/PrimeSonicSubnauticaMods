namespace UpgradedVehicles.Patchers
{
    using Harmony;
    using UnityEngine;

    [HarmonyPatch(typeof(DamageSystem))]
    [HarmonyPatch("CalculateDamage")]
    internal class DamageSystem_CalculateDamage_Patch
    {
        [HarmonyPostfix]
        internal static void Postfix(ref float __result, GameObject target)
        {
            Vehicle vehicle = target.GetComponent<Vehicle>();

            if (vehicle != null) // Target is vehicle
            {
                __result = VehicleUpgrader.ReduceIncomingDamage(vehicle, __result);
            }
        }
    }
}
