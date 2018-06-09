namespace MoreCyclopsUpgrades
{
    using System.Reflection;
    using UnityEngine;

    internal struct ReservePower
    {
        internal int Current;
        internal int Capacity;
    }

    internal static class PowerCharging
    {
        private const float Mk2ChargeRateModifier = 1.15f;
        internal const float NoCharge = 0f;
        internal const float MaxMk2Charge = 100f;

        internal static int GetTotalReservePower(Equipment modules)
        {
            float availableReservePower = 0f;

            foreach (string slotName in SlotHelper.SlotNames)
            {
                TechType techTypeInSlot = modules.GetTechTypeInSlot(slotName);

                if (techTypeInSlot == SolarChargerMk2.SolarMk2TechType ||
                    techTypeInSlot == ThermalChargerMk2.ThermalMk2TechType ||
                    techTypeInSlot == NuclearCharger.CyNukBatteryType)
                {
                    Battery battery = GetBatteryInSlot(modules, slotName);
                    availableReservePower += battery.charge;                    
                }
            }

            return Mathf.FloorToInt(availableReservePower);
        }

        internal static int GetLastPowerPercentage(ref CyclopsHelmHUDManager cyclopsHUD)
        {
            FieldInfo fieldInfo = typeof(CyclopsHelmHUDManager).GetField("lastPowerPctUsedForString", BindingFlags.NonPublic | BindingFlags.Instance);
            return (int)fieldInfo.GetValue(cyclopsHUD);
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
            if (powerDeficit <= 0f) // No power deficit left to charge
                return; // Exit

            if (batteryInSlot.charge <= NoCharge) // The battery has no charge left
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
                batteryInSlot.charge = NoCharge; // Set battery to empty                
            }

            powerDeficit -= chargeAmt; // This is to prevent draining more than needed if the power cells were topped up mid-loop

            cyclops.powerRelay.AddEnergy(chargeAmt, out float amtStored);
        }

        internal static float ChargeFromModule(ref SubRoot cyclops, float chargeAmount, ref float powerDeficit)
        {
            if (powerDeficit <= 0f)
                return chargeAmount; // Surplus power

            if (chargeAmount <= 0f)
                return 0f;

            cyclops.powerRelay.AddEnergy(chargeAmount, out float amtStored);
            powerDeficit = Mathf.Max(0f, powerDeficit - chargeAmount);
                        
            return Mathf.Max(0f, chargeAmount - powerDeficit); // Surplus power
        }

        internal static float ChargeFromModulelMk2(ref SubRoot cyclops, Battery batteryInSlot, float chargeAmount, float batteryDrainRate, ref float powerDeficit)
        {
            if (chargeAmount <= 0f)
            {
                ChargeCyclopsFromBattery(ref cyclops, batteryInSlot, batteryDrainRate, ref powerDeficit);
                return 0f;
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
            powerDeficit = Mathf.Max(0f, powerDeficit - chargeAmount);
            
            batteryInSlot.charge = Mathf.Min(batteryInSlot.capacity, batteryInSlot.charge + chargeAmount);

            return Mathf.Max(0f, chargeAmount - powerDeficit); // Surplus power
        }
    }
}
