namespace CyclopsNuclearUpgrades.Management
{
    using System.Collections.Generic;
    using MoreCyclopsUpgrades.API;
    using UnityEngine;

    internal partial class NuclearModule : UpgradeHandler
    {
        private const float MinimalPowerValue = MCUServices.MinimalPowerValue;
        private const float MaxNuclearChargeRate = 0.16f;
        private const float MinNuclearChargeRate = MinimalPowerValue * 2;
        private const float CooldownRate = MaxNuclearChargeRate * 6f;
        private const float MaxHeat = 1500f;

        private readonly IList<BatteryDetails> batteries = new List<BatteryDetails>();
        private readonly DepletedNuclearModule depletedModule;

        private float totalBatteryCharge = 0f;
        internal float TotalBatteryCharge { get; private set; }

        public NuclearModule(TechType nuclearModule, DepletedNuclearModule module, SubRoot cyclops)
            : base(nuclearModule, cyclops)
        {
            depletedModule = module;
            sprite = SpriteManager.Get(nuclearModule);
            this.MaxCount = 3;

            OnClearUpgrades += () =>
            {
                totalBatteryCharge = 0f;
                batteries.Clear();
            };

            OnUpgradeCounted += (Equipment modules, string slot) =>
            {
                var details = new BatteryDetails(modules, slot, modules.GetItemInSlot(slot).item.GetComponent<Battery>());
                batteries.Add(details);
                totalBatteryCharge += details.BatteryRef._charge;
            };

            OnFinishedWithoutUpgrades += () =>
            {
                this.TotalBatteryCharge = 0f;
            };

            OnFinishedWithUpgrades += () =>
            {
                this.TotalBatteryCharge = totalBatteryCharge;
            };

            OnFirstTimeMaxCountReached += () =>
            {
                ErrorMessage.AddMessage(CyclopsNuclearModule.MaxNuclearReachedMsg);
            };
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
                    depletedModule.DepleteNuclearModule(details.ParentEquipment, details.SlotName);
                }

                totalBatteryCharge -= amtToDrain;
                requestedPower -= amtToDrain; // This is to prevent draining more than needed if the power cells were topped up mid-loop

                totalDrainedAmt += amtToDrain;
            }

            return totalDrainedAmt;
        }
    }
}
