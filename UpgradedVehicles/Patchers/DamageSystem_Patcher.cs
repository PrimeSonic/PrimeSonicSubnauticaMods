namespace UpgradedVehicles.Patchers
{
    using Common;
    using Harmony;
    using UnityEngine;

    [HarmonyPatch(typeof(DamageSystem))]
    [HarmonyPatch("CalculateDamage")]
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

                __result = vehicleUpgrader.ReduceIncomingDamage(__result);
            }
        }
    }
}
