namespace MoreCyclopsUpgrades
{
    using UnityEngine;

    /// <summary>
    /// This class handles the solar power charging.
    /// </summary>
    internal static class SolarChargingManager
    {
        private const float MaxDepth = 200f;
        private const float SolarChargingFactor = 0.03f;
        internal const float BatteryDrainRate = 0.01f;

        public static float GetSolarChargeAmount(ref SubRoot cyclops)
        {
            // The code here mostly replicates what the UpdateSolarRecharge() method does from the SeaMoth class.
            // Consessions were made for the differences between the Seamoth and Cyclops upgrade modules.
            DayNightCycle main = DayNightCycle.main;

            if (main == null)
                return 0f; // Safety check

            // This is 1-to-1 the same way the Seamoth calculates its solar charging rate.
            float proximityToSurface = Mathf.Clamp01((MaxDepth + cyclops.transform.position.y) / MaxDepth);
            float localLightScalar = main.GetLocalLightScalar();

            return SolarChargingFactor * localLightScalar * proximityToSurface;
        }
    }
}
