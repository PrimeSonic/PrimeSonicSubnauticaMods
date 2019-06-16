namespace CyclopsSolarUpgrades.Management
{
    using System.Collections.Generic;
    using MoreCyclopsUpgrades.API;
    using UnityEngine;


    internal partial class Solar : StackingGroupHandler
    {
        internal const int MaxSolarChargers = 6;

        private readonly TechType solarTier1;
        private readonly TechType solarTier2;
        private readonly Atlas.Sprite solar1Sprite;
        private readonly Atlas.Sprite solar2Sprite;
        private readonly IList<BatteryDetails> batteries = new List<BatteryDetails>();

        private float totalBatteryCharge = 0f;
        private float totalBatteryCapacity = 0f;

        internal float TotalBatteryCapacity { get; private set; }
        internal float TotalBatteryCharge { get; private set; }

        public Solar(SubRoot cyclops) : base(cyclops)
        {
            solarTier1 = CyclopsSolarCharger;
            solarTier2 = CyclopsSolarChargerMk2;
            solar1Sprite = SpriteManager.Get(solarTier1);
            solar2Sprite = SpriteManager.Get(solarTier2);

            OnClearUpgrades += () =>
            {
                totalBatteryCharge = 0f;
                totalBatteryCapacity = 0f;
                batteries.Clear();
            };

            StackingUpgradeHandler tier1 = CreateStackingTier(CyclopsSolarCharger);
            tier1.IsAllowedToAdd += (Pickupable item, bool verbose) =>
            {
                return this.TotalCount < MaxSolarChargers;
            };

            StackingUpgradeHandler tier2 = CreateStackingTier(CyclopsSolarChargerMk2);
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
        }
    }
}
