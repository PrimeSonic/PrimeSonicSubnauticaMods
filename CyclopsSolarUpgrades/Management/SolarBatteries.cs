namespace CyclopsSolarUpgrades.Management
{
    using UnityEngine;

    internal partial class Solar
    {
        /// <summary>
        /// Gets the amount of available energy provided by the currently available sunlight.
        /// </summary>
        /// <returns>The currently available solar energy.</returns>
        private static float GetSolarChargeAmount(SubRoot cyclops)
        {
            // The code here mostly replicates what the UpdateSolarRecharge() method does from the SeaMoth class.
            // Consessions were made for the differences between the Seamoth and Cyclops upgrade modules.

            if (DayNightCycle.main == null)
                return 0f; // Safety check

            // This is 1-to-1 the same way the Seamoth calculates its solar charging rate.

            return SolarChargingFactor *
                   DayNightCycle.main.GetLocalLightScalar() *
                   Mathf.Clamp01((MaxSolarDepth + cyclops.transform.position.y) / MaxSolarDepth); // Distance to surfuce
        }

        private static float GetSolarStatus(SubRoot cyclops)
        {
            if (DayNightCycle.main == null)
                return 0f; // Safety check

            return DayNightCycle.main.GetLocalLightScalar() *
                   Mathf.Clamp01((MaxSolarDepth + cyclops.transform.position.y) / MaxSolarDepth);
        }

        private float GetBatteryPower(float drainingRate, float requestedPower)
        {
            if (requestedPower < MinimalPowerValue) // No power deficit left to charge
                return 0f; // Exit

            float totalDrainedAmt = 0f;
            foreach (BatteryDetails details in batteries)
            {
                if (requestedPower <= 0f)
                    continue; // No more power requested

                Battery battery = details.BatteryRef;

                if (battery._charge < MinimalPowerValue) // The battery has no charge left
                    continue; // Skip this battery

                // Mathf.Min is to prevent accidentally taking too much power from the battery
                float amtToDrain = Mathf.Min(requestedPower, drainingRate);

                if (battery._charge > amtToDrain)
                {
                    battery._charge -= amtToDrain;
                }
                else // Battery about to be fully drained
                {
                    amtToDrain = battery._charge; // Take what's left
                    battery._charge = 0f; // Set battery to empty
                }

                totalBatteryCharge -= amtToDrain;
                requestedPower -= amtToDrain; // This is to prevent draining more than needed if the power cells were topped up mid-loop

                totalDrainedAmt += amtToDrain;
            }

            return totalDrainedAmt;
        }

        private void RechargeBatteries(float surplusPower)
        {
            bool batteryCharged = false;
            foreach (BatteryDetails details in batteries)
            {
                if (batteryCharged)
                    continue;

                if (surplusPower < MinimalPowerValue)
                    continue;

                if (details.IsFull)
                    continue;

                Battery batteryToCharge = details.BatteryRef;
                batteryToCharge._charge = Mathf.Min(batteryToCharge._capacity, batteryToCharge._charge + surplusPower);
                surplusPower -= (batteryToCharge._capacity - batteryToCharge._charge);
                batteryCharged = true;
            }

            totalBatteryCharge = Mathf.Min(totalBatteryCharge + surplusPower, totalBatteryCapacity);
        }
    }
}
