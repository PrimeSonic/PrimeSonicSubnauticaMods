namespace CyclopsNuclearUpgrades.Management
{
    using System.Collections.Generic;
    using MoreCyclopsUpgrades.API;
    using MoreCyclopsUpgrades.API.Upgrades;
    using UnityEngine;

    internal class NuclearUpgradeHandler : UpgradeHandler
    {
        private const float MinimalPowerValue = MCUServices.MinimalPowerValue;

        private readonly List<BatteryDetails> batteries = new List<BatteryDetails>();

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

        public NuclearUpgradeHandler(TechType nuclearModule, SubRoot cyclops)
            : base(nuclearModule, cyclops)
        {
            this.MaxCount = 4;

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
            for (int i = 0; i < batteries.Count; i++)
            {
                if (requestedPower <= 0f)
                    continue; // No more power requested

                BatteryDetails details = batteries[i];

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
                    DepleteNuclearModule(details.ParentEquipment, details.SlotName);
                }

                totalBatteryCharge -= amtToDrain;
                requestedPower -= amtToDrain; // This is to prevent draining more than needed if the power cells were topped up mid-loop
                totalDrainedAmt += amtToDrain;
            }

            return totalDrainedAmt;
        }

        private void DepleteNuclearModule(Equipment modules, string slotName)
        {
            MCUServices.Logger.Debug("Nuclear module depleting");

            InventoryItem inventoryItem = modules.RemoveItem(slotName, true, false);

            if (inventoryItem != null)
                GameObject.Destroy(inventoryItem.item.gameObject);
            
            if (modules.AddItem(slotName, CyclopsUpgrade.SpawnCyclopsModule(TechType.DepletedReactorRod), true))
                ErrorMessage.AddMessage(CyclopsNuclearModule.DepletedEventMsg);
        }
    }
}
