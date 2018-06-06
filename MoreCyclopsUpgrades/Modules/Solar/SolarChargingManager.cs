namespace MoreCyclopsUpgrades
{
    using UnityEngine;

    /// <summary>
    /// This class handles the solar power charging.
    /// </summary>
    internal static class SolarChargingManager
    {
        private const float BaseChargingFactor = 0.03f;
        private const float Mk2ChargeRateModifier = 1.3f;
        private const float BaseDrainRate = 0.01f;
        private const float maxDepth = 200f;
        public const float MaxMk2Charge = 100f;
        private const float NoCharge = 0f;

        public static void ChargeFromSolar(ref SubRoot cyclops, float solarChargeAmount, ref float powerDeficit)
        {
            if (solarChargeAmount > 0)
            {
                cyclops.powerRelay.AddEnergy(solarChargeAmount, out float amtStored);
                powerDeficit = Mathf.Max(0f, powerDeficit - solarChargeAmount);
            }
        }

        public static void ChargeFromSolarMk2(ref SubRoot cyclops, Equipment modules, float solarChargeAmount, string slotName, ref float powerDeficit)
        {   
            if (solarChargeAmount <= 0)
            {
                BatteryChargeManager.DrainBattery(ref cyclops, modules, slotName, BaseDrainRate, ref powerDeficit);
            }
            else
            {
                solarChargeAmount *= Mk2ChargeRateModifier; // The Mk2 Solar Charger gets a bonus to it's charge rate

                cyclops.powerRelay.AddEnergy(solarChargeAmount, out float amtStored);
                powerDeficit = Mathf.Max(0f, powerDeficit - solarChargeAmount);

                BatteryChargeManager.ChargeBattery(modules, slotName, solarChargeAmount);
            }
        }
        public static float GetSolarChargeAmount(ref SubRoot cyclops)
        {
            // The code here mostly replicates what the UpdateSolarRecharge() method does from the SeaMoth class.
            // Consessions were made for the differences between the Seamoth and Cyclops upgrade modules.
            DayNightCycle main = DayNightCycle.main;

            if (main == null)
                return 0f; // Safety check

            // This is 1-to-1 the same way the Seamoth calculates its solar charging rate.
            float proximityToSurface = Mathf.Clamp01((maxDepth + cyclops.transform.position.y) / maxDepth);
            float localLightScalar = main.GetLocalLightScalar();

            return BaseChargingFactor * localLightScalar * proximityToSurface;
        }
    }
}
