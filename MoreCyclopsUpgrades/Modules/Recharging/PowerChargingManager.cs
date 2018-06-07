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

    internal static class PowerChargingManager
    {
        private const float Mk2ChargeRateModifier = 1.15f;
        internal const float NoCharge = 0f;
        internal const float MaxMk2Charge = 100f;

        internal static Battery GetBatteryInSlot(Equipment modules, string slotName)
        {
            // Get the battery component
            InventoryItem item = modules.GetItemInSlot(slotName);
            Battery batteryInSlot = item.item.GetComponent<Battery>();
            return batteryInSlot;
        }

        internal static BatteryState ChargeBattery(Battery batteryInSlot, float addedCharge)
        {
            batteryInSlot.charge = Mathf.Min(batteryInSlot.capacity, batteryInSlot.charge + addedCharge);

            if (batteryInSlot.charge == batteryInSlot.capacity)
                return BatteryState.Full;
            else
                return BatteryState.Charged;
        }

        internal static BatteryState DrainBattery(ref SubRoot cyclops, Battery batteryInSlot, float drainingRate, ref float powerDeficit)
        {
            if (powerDeficit <= 0f) // No power deficit left to charge
                return BatteryState.Undetermined; // Exit

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

        public static void ChargeFromModule(ref SubRoot cyclops, float chargeAmount, ref float powerDeficit)
        {
            if (chargeAmount <= 0 || powerDeficit <= 0)
                return;

            cyclops.powerRelay.AddEnergy(chargeAmount, out float amtStored);
            powerDeficit = Mathf.Max(0f, powerDeficit - chargeAmount);
        }

        public static BatteryState ChargeFromModulelMk2(ref SubRoot cyclops, Battery batteryInSlot, float thermalChargeAmount, float batteryDrainRate, ref float powerDeficit)
        {
            if (thermalChargeAmount <= 0)
            {
                return DrainBattery(ref cyclops, batteryInSlot, batteryDrainRate, ref powerDeficit);
            }
            else
            {
                thermalChargeAmount *= Mk2ChargeRateModifier;

                cyclops.powerRelay.AddEnergy(thermalChargeAmount, out float amtStored);
                powerDeficit = Mathf.Max(0f, powerDeficit - thermalChargeAmount);

                return ChargeBattery(batteryInSlot, thermalChargeAmount);
            }
        }
    }
}
