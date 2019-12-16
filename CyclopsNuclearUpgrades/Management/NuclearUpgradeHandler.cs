namespace CyclopsNuclearUpgrades.Management
{
    using System.Collections.Generic;
    using MoreCyclopsUpgrades.API.AmbientEnergy;
    using MoreCyclopsUpgrades.API;
    using MoreCyclopsUpgrades.API.Upgrades;
    using UnityEngine;

    internal interface INuclearModuleDepleter
    {
        void DepleteNuclearModule(Equipment modules, string slotName);
    }

    internal class NuclearUpgradeHandler : UpgradeHandler
    {
        internal delegate void DepleteModule(Equipment modules, string slotName);

        private const float MinimalPowerValue = MCUServices.MinimalPowerValue;
        private readonly IList<BatteryDetails> batteries = new List<BatteryDetails>();
        private readonly INuclearModuleDepleter moduleDepleter;
        private bool updating = false;
        private float totalBatteryCharge = 0f;
        internal float TotalBatteryCharge
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
        internal bool TooHotToHandle { get; set; } = false;

        public NuclearUpgradeHandler(TechType nuclearModule, INuclearModuleDepleter depleter, SubRoot cyclops)
            : base(nuclearModule, cyclops)
        {
            moduleDepleter = depleter;

            this.MaxCount = 3;

            OnClearUpgrades += () =>
            {
                totalBatteryCharge = this.TotalBatteryCharge;
                updating = true;
                batteries.Clear();
            };

            OnUpgradeCountedDetailed += (Equipment modules, string slot, InventoryItem inventoryItem) =>
            {
                var details = new BatteryDetails(modules, slot, inventoryItem.item.GetComponent<Battery>());
                batteries.Add(details);
            };

            OnFinishedUpgrades = () =>
            {
                updating = false;
            };

            OnFirstTimeMaxCountReached += () =>
            {
                ErrorMessage.AddMessage(CyclopsNuclearModule.MaxNuclearReachedMsg);
            };

            IsAllowedToRemove += (Pickupable item, bool verbose) =>
            {
                return !this.TooHotToHandle;
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
                float amtToDrain = Mathf.Min(requestedPower, drainingRate * DayNightCycle.main.deltaTime);

                if (battery._charge > amtToDrain)
                {
                    battery._charge -= amtToDrain;
                }
                else // Battery about to be fully drained
                {
                    amtToDrain = battery._charge; // Take what's left
                    battery._charge = 0f; // Set battery to empty
                    moduleDepleter.DepleteNuclearModule(details.ParentEquipment, details.SlotName);
                }

                totalBatteryCharge -= amtToDrain;
                requestedPower -= amtToDrain; // This is to prevent draining more than needed if the power cells were topped up mid-loop

                totalDrainedAmt += amtToDrain;
            }

            return totalDrainedAmt;
        }
    }
}
