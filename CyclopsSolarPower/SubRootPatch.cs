namespace CyclopsSolarPower
{
    using Harmony;
    using UnityEngine;

    [HarmonyPatch(typeof(SubRoot))]
    [HarmonyPatch("UpdateThermalReactorCharge")]
    public class SubRootPatches
    {
        private static void Postfix(SubRoot instance)
        {
            // The code here mostly replicates the UpdateSolarRecharge() method found on the SeaMoth class
            DayNightCycle main = DayNightCycle.main;
            if (main == null)
            {
                return; // This was probably put here for safety
            }

            if (instance.thermalReactorUpgrade)
            {
                int numberOfSolarChargeModules = 2;
                float proximityToSurface = Mathf.Clamp01((200f + instance.transform.position.y) / 200f);
                float localLightScalar = main.GetLocalLightScalar();
                float amount = 1f * localLightScalar * proximityToSurface * numberOfSolarChargeModules;
                instance.powerRelay.AddEnergy(amount, out float amtStored);
            }
        }
    }
}
