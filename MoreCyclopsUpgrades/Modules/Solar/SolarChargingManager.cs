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
        private const float maxDepth = 200f;
        public const float MaxMk2Charge = 300f;

        public static void AddSolarCharge(ref SubRoot cyclops, float chargeAmt, ref float powerDeficit)
        {
            cyclops.powerRelay.AddEnergy(chargeAmt, out float amtStored);
            powerDeficit = Mathf.Max(0f, powerDeficit - chargeAmt);
        }

        public static void AddSolarChargeMk2(ref SubRoot cyclops, float chargeAmt, ref float powerDeficit)
        {
            chargeAmt *= Mk2ChargeRateModifier;
            cyclops.powerRelay.AddEnergy(chargeAmt, out float amtStored);
            powerDeficit = Mathf.Max(0f, powerDeficit - chargeAmt);
        }

        public static float GetSolarChargeAmount(ref SubRoot cyclops)
        {
            // The code here mostly replicates what the UpdateSolarRecharge() method does from the SeaMoth class.
            // Consessions were made for the differences between the Seamoth and Cyclops upgrade modules.
            DayNightCycle main = DayNightCycle.main;
            if (main == null)
            {
                return 0f; // This was probably put here for safety
            }

            // This is 1-to-1 the same way the Seamoth calculates its solar charging rate.
            float proximityToSurface = Mathf.Clamp01((maxDepth + cyclops.transform.position.y) / maxDepth);
            float localLightScalar = main.GetLocalLightScalar();

            return BaseChargingFactor * localLightScalar * proximityToSurface;
        }

        public static void ChargeSolarBattery(Equipment modules, string slotName, float addedCharge)
        {
            // Get the battery component
            InventoryItem item = modules.GetItemInSlot(slotName);
            Battery batteryInSlot = item.item.GetComponent<Battery>();

            if (batteryInSlot.charge + addedCharge > batteryInSlot.capacity)
            {
                batteryInSlot.charge = batteryInSlot.capacity;
            }
            else
            {
                batteryInSlot.charge += addedCharge;
            }
        }
    }
}
