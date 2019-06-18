namespace CyclopsSolarUpgrades.Management
{
    using System.Collections.Generic;
    using CyclopsSolarUpgrades.Craftables;
    using MoreCyclopsUpgrades.API;
    using UnityEngine;

    internal class SolarUpgrade : StackingGroupHandler
    {
        private const float MinimalPowerValue = MCUServices.MinimalPowerValue;
        internal const int MaxSolarChargers = 6;

        private readonly IList<BatteryDetails> batteries = new List<BatteryDetails>();
        private float totalBatteryCharge = 0f;
        private float totalBatteryCapacity = 0f;

        internal float TotalBatteryCapacity { get; private set; }
        internal float TotalBatteryCharge { get; private set; }

        public SolarUpgrade(TechType solarUpgradeMk1, TechType solarUpgradeMk2, SubRoot cyclops) : base(cyclops)
        {
            OnClearUpgrades += () =>
            {
                totalBatteryCharge = 0f;
                totalBatteryCapacity = 0f;
                batteries.Clear();
            };

            StackingUpgradeHandler tier1 = CreateStackingTier(solarUpgradeMk1);
            tier1.IsAllowedToAdd += (Pickupable item, bool verbose) =>
            {
                return this.TotalCount < MaxSolarChargers;
            };

            StackingUpgradeHandler tier2 = CreateStackingTier(solarUpgradeMk2);
            tier2.IsAllowedToAdd += (Pickupable item, bool verbose) =>
            {
                return this.TotalCount < MaxSolarChargers;
            };

            tier2.OnUpgradeCounted += (Equipment modules, string slot) =>
            {
                var details = new BatteryDetails(modules, slot, modules.GetItemInSlot(slot).item.GetComponent<Battery>());
                batteries.Add(details);
                totalBatteryCharge += details.BatteryRef._charge;
                totalBatteryCapacity += details.BatteryRef._capacity;
            };

            OnFinishedWithoutUpgrades += () =>
            {
                this.TotalBatteryCapacity = 0f;
                this.TotalBatteryCharge = 0f;
            };

            OnFinishedWithUpgrades += () =>
            {
                this.TotalBatteryCapacity = totalBatteryCapacity;
                this.TotalBatteryCharge = totalBatteryCharge;
            };

            OnFirstTimeMaxCountReached += () =>
            {
                ErrorMessage.AddMessage(CyclopsSolarCharger.MaxSolarReached());
            };
        }

        internal float GetBatteryPower(float drainingRate, float requestedPower)
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

        internal void RechargeBatteries(float surplusPower)
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
