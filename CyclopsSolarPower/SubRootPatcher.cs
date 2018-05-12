namespace CyclopsSolarPower
{
    using Harmony;
    using UnityEngine;

    [HarmonyPatch(typeof(SubRoot))]
    [HarmonyPatch("UpdateThermalReactorCharge")]    
    internal class SubRootPatcher
    {
        public static void Postfix(ref SubRoot __instance)
        {
            // The code here mostly replicates the UpdateSolarRecharge() method from the SeaMoth class,
            // with consessions made for the differences between the Seamoth and Cyclops upgrade modules.
            DayNightCycle main = DayNightCycle.main;
            if (main == null)
            {
                return; // This was probably put here for safety
            }

            if (__instance.thermalReactorUpgrade)
            {
                float proximityToSurface = Mathf.Clamp01((200f + __instance.transform.position.y) / 200f);
                float localLightScalar = main.GetLocalLightScalar();

                // It seems small, but this charges quite fast, especially in shallow water.
                const float baseChargingFactor = 0.01f; 
                // And that was without the engine efficiency module!

                float amount = baseChargingFactor * localLightScalar * proximityToSurface;
                __instance.powerRelay.AddEnergy(amount, out float amtStored);
            }

            // This is almost too powerful on even as it is right now.
            // A separate module might be more balanced, but in the end would only be a nusance to the player having to switch them back and forth.
            // Find out how much power the Cyclops drains under normal operation and set the optimal recharge rate to just under that.            
        }
    }
}
