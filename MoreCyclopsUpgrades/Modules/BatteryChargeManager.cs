namespace MoreCyclopsUpgrades
{
    using UnityEngine;

    internal enum BatteryState : byte
    {
        Undetermined,
        Empty,
        Charged,
        Full        
    }

    internal static class BatteryChargeManager
    {
        internal const float NoCharge = 0f;

        internal static BatteryState ChargeBattery(Equipment modules, string slotName, float addedCharge)
        {
            // Get the battery component
            InventoryItem item = modules.GetItemInSlot(slotName);
            Battery batteryInSlot = item.item.GetComponent<Battery>();

            batteryInSlot.charge = Mathf.Min(batteryInSlot.capacity, batteryInSlot.charge + addedCharge);

            if (batteryInSlot.charge == batteryInSlot.capacity)
                return BatteryState.Full;
            else
                return BatteryState.Charged;
        }

        internal static BatteryState DrainBattery(ref SubRoot cyclops, Equipment modules, string slotName, float drainingRate, ref float powerDeficit)
        {
            if (powerDeficit <= 0f) // No power deficit left to charge
                return BatteryState.Undetermined; // Exit

            // Get the battery component
            InventoryItem item = modules.GetItemInSlot(slotName);
            Battery batteryInSlot = item.item.GetComponent<Battery>();

            if (batteryInSlot.charge <= NoCharge) // The battery has no charge left
                return BatteryState.Empty; // Skip this battery

            // Mathf.Min is to prevent accidentally taking too much power from the battery
            float chargeAmt = Mathf.Min(powerDeficit, drainingRate);

            BatteryState batteryState;

            if (batteryInSlot.charge > chargeAmt)
            {
                batteryInSlot.charge -= chargeAmt;
                batteryState = BatteryState.Charged;
            }
            else // Battery about to be fully drained
            {
                chargeAmt = batteryInSlot.charge; // Take what's left
                batteryInSlot.charge = NoCharge; // Set battery to empty
                batteryState = BatteryState.Empty;
            }

            powerDeficit -= chargeAmt; // This is to prevent draining more than needed if the power cells were topped up mid-loop

            cyclops.powerRelay.AddEnergy(chargeAmt, out float amtStored);

            return batteryState;
        }
    }
}
