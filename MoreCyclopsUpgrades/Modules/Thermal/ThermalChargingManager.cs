namespace MoreCyclopsUpgrades
{
    using UnityEngine;

    internal static class ThermalChargingManager
    {
        public const float MaxMk2Charge = 100f;
        private const float NoCharge = 0f;
        private const float BaseChargingFactor = 1.5f;

        public static void ChargeFromThermalMk2(ref SubRoot cyclops, Equipment modules, float thermalChargeAmount, string slotName, ref float powerDeficit)
        {
            if (thermalChargeAmount <= 0)
            {
                DrainThermalBattery(ref cyclops, modules, slotName, ref powerDeficit);
            }
            else
            {
                thermalChargeAmount *= BaseChargingFactor; // The Mk2 Solar Charger gets a bonus to it's charge rate

                cyclops.powerRelay.AddEnergy(thermalChargeAmount, out float amtStored);
                powerDeficit = Mathf.Max(0f, powerDeficit - thermalChargeAmount);

                ChargeThermalBattery(modules, slotName, thermalChargeAmount);
            }
        }

        private static void ChargeThermalBattery(Equipment modules, string slotName, float addedCharge)
        {
            // Get the battery component
            InventoryItem item = modules.GetItemInSlot(slotName);
            Battery batteryInSlot = item.item.GetComponent<Battery>();

            batteryInSlot.charge = Mathf.Min(batteryInSlot.capacity, batteryInSlot.charge + addedCharge);
        }

        private static void DrainThermalBattery(ref SubRoot cyclops, Equipment modules, string slotName, ref float powerDeficit)
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
            else // Battery about to be fully drained
            {
                chargeAmt = batteryInSlot.charge; // Take what's left
                batteryInSlot.charge = NoCharge; // Set battery to empty
            }

            powerDeficit -= chargeAmt; // This is to prevent draining more than needed if the power cells were topped up mid-loop

            cyclops.powerRelay.AddEnergy(chargeAmt, out float amtStored);
        }
    }
}
