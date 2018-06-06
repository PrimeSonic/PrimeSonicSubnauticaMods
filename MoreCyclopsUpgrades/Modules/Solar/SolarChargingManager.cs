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
                DrainSolarBattery(ref cyclops, modules, slotName, ref powerDeficit);
            }
            else
            {
                solarChargeAmount *= Mk2ChargeRateModifier;

                cyclops.powerRelay.AddEnergy(solarChargeAmount, out float amtStored);
                powerDeficit = Mathf.Max(0f, powerDeficit - solarChargeAmount);

                ChargeSolarBattery(modules, slotName, solarChargeAmount);
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

        private static void ChargeSolarBattery(Equipment modules, string slotName, float addedCharge)
        {
            // Get the battery component
            InventoryItem item = modules.GetItemInSlot(slotName);
            Battery batteryInSlot = item.item.GetComponent<Battery>();

            batteryInSlot.charge = Mathf.Min(batteryInSlot.capacity, batteryInSlot.charge + addedCharge);
        }

        private static void DrainSolarBattery(ref SubRoot cyclops, Equipment modules, string slotName, ref float powerDeficit)
        {
            if (powerDeficit <= 0f) // No power deficit left to charge
                return; // Exit

            // Get the battery component
            InventoryItem item = modules.GetItemInSlot(slotName);
            Battery batteryInSlot = item.item.GetComponent<Battery>();

            if (batteryInSlot.charge == NoCharge) // The battery has no charge left
                return; // Skip this battery

            // Mathf.Min is to prevent accidentally taking too much power from the battery
            float chargeAmt = Mathf.Min(powerDeficit, BaseChargingFactor);

            if (batteryInSlot.charge > chargeAmt)
            {
                batteryInSlot.charge -= chargeAmt;
            }
            else
            {
                chargeAmt = batteryInSlot.charge;
                batteryInSlot.charge = NoCharge;
            }

            powerDeficit -= chargeAmt; // This is to prevent draining more than needed if the power cells were topped up mid-loop

            cyclops.powerRelay.AddEnergy(chargeAmt, out float amtStored);
        }
    }
}
