namespace MoreCyclopsUpgrades.CyclopsUpgrades
{
    using MoreCyclopsUpgrades.API;
    using System.Collections.Generic;
    using UnityEngine;

    internal class BatteryUpgradeHandler : ChargingUpgradeHandler
    {
        internal const float MinimalPowerValue = MCUServices.MinimalPowerValue;
        public delegate void BatteryDrainedEvent(BatteryDetails details);
        public BatteryDrainedEvent OnBatteryDrained;

        internal IList<BatteryDetails> Batteries { get; } = new List<BatteryDetails>();

        internal readonly bool BatteryRecharges;
        public float TotalBatteryCharge = 0f;
        public float TotalBatteryCapacity = 0f;

        internal bool BatteryHasCharge => this.Count > 0 && TotalBatteryCharge > MinimalPowerValue;

        public BatteryUpgradeHandler(TechType techType, bool canRecharge, SubRoot cyclops) : base(techType, cyclops)
        {
            BatteryRecharges = canRecharge;
            OnClearUpgrades += () =>
            {
                TotalBatteryCharge = 0f;
                TotalBatteryCapacity = 0f;
                this.Batteries.Clear();
            };

            OnUpgradeCounted += (Equipment modules, string slot) =>
            {
                var details = new BatteryDetails(modules, slot, modules.GetItemInSlot(slot).item.GetComponent<Battery>());
                this.Batteries.Add(details);
                TotalBatteryCharge += details.BatteryRef._charge;
                TotalBatteryCapacity += details.BatteryRef._capacity;
            };
        }

        public float GetBatteryPower(float drainingRate, float requestedPower)
        {
            if (requestedPower < MinimalPowerValue) // No power deficit left to charge
                return 0f; // Exit
            float totalDrainedAmt = 0f;
            foreach (BatteryDetails details in this.Batteries)
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

                    OnBatteryDrained?.Invoke(details);
                }

                TotalBatteryCharge -= amtToDrain;
                requestedPower -= amtToDrain; // This is to prevent draining more than needed if the power cells were topped up mid-loop

                totalDrainedAmt += amtToDrain;
            }

            return totalDrainedAmt;
        }

        public void RechargeBatteries(float surplusPower)
        {
            if (!BatteryRecharges)
                return;

            bool batteryCharged = false;
            foreach (BatteryDetails details in this.Batteries)
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

            TotalBatteryCharge = Mathf.Min(TotalBatteryCharge + surplusPower, TotalBatteryCapacity);
        }
    }
}
