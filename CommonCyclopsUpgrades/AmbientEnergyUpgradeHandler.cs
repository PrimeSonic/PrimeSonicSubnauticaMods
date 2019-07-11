namespace CommonCyclopsUpgrades
{
    using System.Collections.Generic;
    using MoreCyclopsUpgrades.API;
    using MoreCyclopsUpgrades.API.Upgrades;
    using UnityEngine;

    internal class AmbientEnergyUpgradeHandler : StackingGroupHandler
    {
        private const float MinimalPowerValue = MCUServices.MinimalPowerValue;
        public const int MaxChargers = 6;

        private readonly IList<BatteryDetails> batteries = new List<BatteryDetails>();
        private float totalBatteryCharge = 0f;
        private float totalBatteryCapacity = 0f;
        private bool updating = false;

        public float TotalBatteryCapacity
        {
            get
            {
                if (!updating)
                {
                    totalBatteryCapacity = 0f;
                    foreach (BatteryDetails battery in batteries)
                        totalBatteryCapacity += battery.BatteryRef._capacity;
                }

                return totalBatteryCapacity;
            }
        }

        public float TotalBatteryCharge
        {
            get
            {
                if (!updating)
                {
                    totalBatteryCharge = 0f;
                    foreach (BatteryDetails battery in batteries)
                        totalBatteryCharge += battery.BatteryRef._charge;
                }

                return totalBatteryCharge;
            }
        }

        public float ChargeMultiplier { get; private set; } = 1f;

        public bool HasBatteriesToCharge => tier2.Count > 0;

        public readonly TechType Tier1ID;
        public readonly TechType Tier2ID;

        private readonly StackingUpgradeHandler tier1;
        private readonly StackingUpgradeHandler tier2;
        private readonly string maxCountReachedMsg;

        public AmbientEnergyUpgradeHandler(TechType tier1Id, TechType tier2Id, string maxedOutMsg, SubRoot cyclops)
            : base(cyclops)
        {
            Tier1ID = tier1Id;
            Tier2ID = tier2Id;

            this.MaxCount = MaxChargers;
            maxCountReachedMsg = maxedOutMsg;

            OnClearUpgrades = () =>
            {
                updating = true;
                batteries.Clear();
            };

            tier1 = CreateStackingTier(tier1Id);
            tier2 = CreateStackingTier(tier2Id);

            tier1.MaxCount = MaxChargers;
            tier2.MaxCount = MaxChargers;

            tier1.IsAllowedToAdd += CheckCombinedTotal;
            tier2.IsAllowedToAdd += CheckCombinedTotal;

            OnUpgradeCountedDetailed += AddBatteryDetails;

            OnFinishedUpgrades = () =>
            {
                updating = false;

                if (this.Count > 1)
                {
                    // Stacking multiple solar/thermal chargers has diminishing returns on how much extra energy you can get after the first.
                    // The diminishing returns are themselves also variable.
                    // Heavy diminishing returns for tier 1 modules.
                    // Better returns and multiplier for tier 2 modules.

                    // The diminishing returns follow a geometric sequence with a factor always less than 1.
                    // You can check the math on this over here https://www.purplemath.com/modules/series5.htm

                    float diminishingReturnFactor = 0.45f + (0.045f * tier2.Count);
                    float chargeMultiplier = (1 - Mathf.Pow(diminishingReturnFactor, this.Count)) /
                                                (1 - diminishingReturnFactor);
                    chargeMultiplier += 0.015f * tier2.Count;

                    this.ChargeMultiplier = Mathf.Max(1f, chargeMultiplier);
                }
            };

            OnFirstTimeMaxCountReached = () =>
            {
                ErrorMessage.AddMessage(maxCountReachedMsg);
            };
        }

        private bool CheckCombinedTotal(Pickupable item, bool verbose)
        {
            return this.Count < MaxChargers;
        }

        private void AddBatteryDetails(Equipment modules, string slot, InventoryItem inventoryItem)
        {
            if (inventoryItem.item.GetTechType() == tier2.TechType)
            {
                var details = new BatteryDetails(modules, slot, inventoryItem.item.GetComponent<Battery>());
                batteries.Add(details);
            }
        }

        public float GetBatteryPower(float drainingRate, float requestedPower)
        {
            float totalDrainedAmt = 0f;
            foreach (BatteryDetails details in batteries)
            {
                if (requestedPower < MinimalPowerValue) // No power deficit left to charge
                    break; // Exit

                Battery battery = details.BatteryRef;

                if (battery._charge < MinimalPowerValue) // The battery has no charge left
                    continue; // Skip this battery

                // Mathf.Min is to prevent accidentally taking too much power from the battery
                float amtToDrain = Mathf.Min(requestedPower, drainingRate * DayNightCycle.main.deltaTime);

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

        public void RechargeBatteries(float surplusPower)
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

