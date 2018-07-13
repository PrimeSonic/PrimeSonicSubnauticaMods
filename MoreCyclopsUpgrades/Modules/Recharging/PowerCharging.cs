namespace MoreCyclopsUpgrades
{
    using Common;
    using UnityEngine;

    internal static class PowerCharging
    {
        private const float Mk2ChargeRateModifier = 1.15f;        
        internal const float MaxMk2Charge = 100f;
        private const float ZeroCharge = 0f;

        internal static int GetTotalReservePower(Equipment modules)
        {
            float availableReservePower = ZeroCharge;

            foreach (string slotName in SlotHelper.SlotNames)
            {
                TechType techTypeInSlot = modules.GetTechTypeInSlot(slotName);

                if (techTypeInSlot == CyclopsModule.SolarChargerMk2ID ||
                    techTypeInSlot == CyclopsModule.ThermalChargerMk2ID ||
                    techTypeInSlot == CyclopsModule.NuclearChargerID)
                {
                    Battery battery = GetBatteryInSlot(modules, slotName);
                    availableReservePower += battery.charge;
                }
            }

            return Mathf.FloorToInt(availableReservePower);
        }

        internal static int GetLastPowerPercentage(ref CyclopsHelmHUDManager cyclopsHUD)
        {
            return (int)cyclopsHUD.GetPrivateField("lastPowerPctUsedForString");
        }

        internal static Battery GetBatteryInSlot(Equipment modules, string slotName)
        {
            // Get the battery component
            InventoryItem item = modules.GetItemInSlot(slotName);
            Battery batteryInSlot = item.item.GetComponent<Battery>();
            return batteryInSlot;
        }

        internal static void ChargeCyclopsFromBattery(ref SubRoot cyclops, Battery batteryInSlot, float drainingRate, ref float powerDeficit)
        {
            if (Mathf.Approximately(powerDeficit, ZeroCharge)) // No power deficit left to charge
                return; // Exit

            if (Mathf.Approximately(batteryInSlot.charge, ZeroCharge)) // The battery has no charge left
                return; // Skip this battery

            // Mathf.Min is to prevent accidentally taking too much power from the battery
            float chargeAmt = Mathf.Min(powerDeficit, drainingRate);

            if (batteryInSlot.charge > chargeAmt)
            {
                batteryInSlot.charge -= chargeAmt;
            }
            else // Battery about to be fully drained
            {
                chargeAmt = batteryInSlot.charge; // Take what's left
                batteryInSlot.charge = ZeroCharge; // Set battery to empty                
            }

            powerDeficit -= chargeAmt; // This is to prevent draining more than needed if the power cells were topped up mid-loop

            cyclops.powerRelay.AddEnergy(chargeAmt, out float amtStored);
        }

        internal static float ChargeFromModule(ref SubRoot cyclops, float chargeAmount, ref float powerDeficit)
        {
            if (Mathf.Approximately(powerDeficit, ZeroCharge))
                return chargeAmount; // Surplus power

            if (Mathf.Approximately(chargeAmount, ZeroCharge))
                return ZeroCharge;

            cyclops.powerRelay.AddEnergy(chargeAmount, out float amtStored);
            powerDeficit = Mathf.Max(ZeroCharge, powerDeficit - chargeAmount);

            return Mathf.Max(ZeroCharge, chargeAmount - powerDeficit); // Surplus power
        }

        internal static float ChargeFromModulelMk2(ref SubRoot cyclops, Battery batteryInSlot, float chargeAmount, float batteryDrainRate, ref float powerDeficit)
        {
            if (Mathf.Approximately(chargeAmount, ZeroCharge))
            {
                ChargeCyclopsFromBattery(ref cyclops, batteryInSlot, batteryDrainRate, ref powerDeficit);
                return ZeroCharge;
            }
            else
            {
                return ChargeCyclopsAndBattery(cyclops, batteryInSlot, ref chargeAmount, ref powerDeficit);
            }
        }

        private static float ChargeCyclopsAndBattery(SubRoot cyclops, Battery batteryInSlot, ref float chargeAmount, ref float powerDeficit)
        {
            chargeAmount *= Mk2ChargeRateModifier;

            cyclops.powerRelay.AddEnergy(chargeAmount, out float amtStored);
            powerDeficit = Mathf.Max(ZeroCharge, powerDeficit - chargeAmount);

            batteryInSlot.charge = Mathf.Min(batteryInSlot.capacity, batteryInSlot.charge + chargeAmount);

            return Mathf.Max(ZeroCharge, chargeAmount - powerDeficit); // Surplus power
        }
    }
}
