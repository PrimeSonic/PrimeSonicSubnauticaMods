namespace MoreCyclopsUpgrades.API.AmbientEnergy
{
    using System.Collections.Generic;
    using MoreCyclopsUpgrades.API;
    using MoreCyclopsUpgrades.API.Upgrades;
    using UnityEngine;

    /// <summary>
    /// A standardized <see cref="StackingGroupHandler"/> implementation for <see cref="AmbientEnergyCharger{T}"/>s.
    /// </summary>
    /// <seealso cref="StackingGroupHandler" />
    public class AmbientEnergyUpgradeHandler : StackingGroupHandler
    {
        private const float MinimalPowerValue = MCUServices.MinimalPowerValue;

        /// <summary>
        /// Gets the maximum number of chargers that can work together to increase the charging rate.<para/>
        /// Defaults to <c>6</c>.
        /// </summary>
        /// <value>
        /// The maximum number of same-type chargers that can provide power together.
        /// </value>
        public virtual int MaxChargers => 6;

        private readonly IList<BatteryDetails> batteries = new List<BatteryDetails>();
        private float totalBatteryCharge = 0f;
        private float totalBatteryCapacity = 0f;
        private bool updating = false;
        private float totalDrainedAmt = 0f;

        /// <summary>
        /// Gets the total battery capacity.
        /// </summary>
        /// <value>
        /// The total battery capacity.
        /// </value>
        /// <seealso cref="AmbientEnergyCharger{T}.TotalReserveEnergy"/>
        /// <seealso cref="AmbientEnergyCharger{T}.StatusTextColor"/>
        public float TotalBatteryCapacity
        {
            get
            {
                if (!updating)
                {
                    totalBatteryCapacity = 0f;
                    for (int i = 0; i < batteries.Count; i++)
                        totalBatteryCapacity += batteries[i].BatteryRef._capacity;
                }

                return totalBatteryCapacity;
            }
        }

        /// <summary>
        /// Gets the total battery charge.
        /// </summary>
        /// <value>
        /// The total battery charge.
        /// </value>
        /// <seealso cref="AmbientEnergyCharger{T}.TotalReserveEnergy"/>
        /// <seealso cref="AmbientEnergyCharger{T}.StatusTextColor"/>
        public float TotalBatteryCharge
        {
            get
            {
                if (!updating)
                {
                    totalBatteryCharge = 0f;
                    for (int i = 0; i < batteries.Count; i++)
                        totalBatteryCharge += batteries[i].BatteryRef._charge;
                }

                return totalBatteryCharge;
            }
        }

        /// <summary>
        /// Gets the charge multiplier. This updates depending on how many instances of this upgrade module (and at what tier) are currently installed in the Cyclops.
        /// </summary>
        /// <value>
        /// The charge multiplier.
        /// </value>
        public float ChargeMultiplier { get; private set; } = 1f;

        private readonly StackingUpgradeHandler tier1;
        private readonly StackingUpgradeHandler tier2;
        private readonly string maxCountReachedMsg;

        /// <summary>
        /// Initializes a new instance of the <see cref="AmbientEnergyUpgradeHandler"/> class.
        /// </summary>
        /// <param name="tier1Id">The tier1 identifier.</param>
        /// <param name="tier2Id">The tier2 identifier.</param>
        /// <param name="maxedOutMsg">The maxed out MSG.</param>
        /// <param name="cyclops">The cyclops.</param>
        public AmbientEnergyUpgradeHandler(TechType tier1Id, TechType tier2Id, string maxedOutMsg, SubRoot cyclops)
            : base(cyclops)
        {
            this.MaxCount = this.MaxChargers;
            maxCountReachedMsg = maxedOutMsg;

            OnClearUpgrades = () =>
            {
                updating = true;
                batteries.Clear();
            };

            tier1 = CreateStackingTier(tier1Id);
            tier2 = CreateStackingTier(tier2Id);

            tier1.MaxCount = this.MaxChargers;
            tier2.MaxCount = this.MaxChargers;

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
            return this.Count < this.MaxChargers;
        }

        private void AddBatteryDetails(Equipment modules, string slot, InventoryItem inventoryItem)
        {
            if (inventoryItem.item.GetTechType() == tier2.TechType)
            {
                var details = new BatteryDetails(modules, slot, inventoryItem.item.GetComponent<Battery>());
                batteries.Add(details);
            }
        }

        /// <summary>
        /// Gets power from the reserve battery.
        /// </summary>
        /// <param name="drainingRate">The rate at which power can be pulled from the battery.</param>
        /// <param name="requestedPower">The amonut of requested power.</param>
        /// <returns></returns>
        public float GetBatteryPower(float drainingRate, float requestedPower)
        {
            totalDrainedAmt = 0f;
            for (int i = 0; i < batteries.Count; i++)
            {
                if (requestedPower < MinimalPowerValue) // No power deficit left to charge
                    break; // Exit

                Battery battery = batteries[i].BatteryRef;

                if (battery._charge < MinimalPowerValue) // The battery has no charge left
                    continue; // Skip this battery

                // Mathf.Min is to prevent accidentally taking too much power from the battery
                float amtToDrain = Mathf.Min(requestedPower, drainingRate * DayNightCycle.main.deltaTime);

                // As a hidden treat, the last "blip" of battery power will be more than it otherwise would have been
                battery._charge = Mathf.Max(0f, battery._charge - amtToDrain);

                totalBatteryCharge -= amtToDrain;
                requestedPower -= amtToDrain; // This is to prevent draining more than needed if the power cells were topped up mid-loop

                totalDrainedAmt += amtToDrain;
            }

            return totalDrainedAmt;
        }

        /// <summary>
        /// Recharges the reserve batteries with the provided surplus power.
        /// </summary>
        /// <param name="surplusPower">The surplus power.</param>
        public void RechargeBatteries(float surplusPower)
        {
            for (int i = 0; i < batteries.Count; i++)
            {
                BatteryDetails details = batteries[i];

                if (details.IsFull)
                    continue;

                Battery batteryToCharge = details.BatteryRef;
                batteryToCharge._charge = Mathf.Min(batteryToCharge._capacity, batteryToCharge._charge + surplusPower);
                surplusPower -= (batteryToCharge._capacity - batteryToCharge._charge);
                break;
            }
        }
    }
}

