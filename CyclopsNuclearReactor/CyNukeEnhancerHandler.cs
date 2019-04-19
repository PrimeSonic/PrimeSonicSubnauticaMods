namespace CyclopsNuclearReactor
{
    using Common;
    using MoreCyclopsUpgrades.CyclopsUpgrades;
    using System.Collections.Generic;
    using UnityEngine;

    internal class CyNukeEnhancerHandler : TieredUpgradesHandlerCollection<int>
    {
        private static float errorDelay = 0f;
        private const float delayInterval = 10f;

        private readonly TieredUpgradeHandler<int> tier1;
        private readonly TieredUpgradeHandler<int> tier2;

        public CyNukeEnhancerHandler() : base(0)
        {
            // CyNukeEnhancerMk1
            tier1 = CreateTier(CyNukeEnhancerMk1.TechTypeID, 1);
            tier1.MaxCount = 1;
            tier1.IsAllowedToRemove += (SubRoot cyclops, Pickupable item, bool verbose) =>
            {
                if (tier2.HasUpgrade)
                    return true;

                int availableSlots = CyNukeReactorMono.CalculateTotalSlots(0);

                return HasRoomToStrink(cyclops, availableSlots);
            };

            // CyNukeEnhancerMk2
            tier2 = CreateTier(CyNukeEnhancerMk2.TechTypeID, 2);
            tier2.MaxCount = 1;
            IsAllowedToRemove += (SubRoot cyclops, Pickupable item, bool verbose) =>
            {
                int availableSlots = CyNukeReactorMono.CalculateTotalSlots(tier1.HasUpgrade ? 1 : 0);

                return HasRoomToStrink(cyclops, availableSlots);
            };

            // Collection
            OnFinishedUpgrades += (SubRoot cyclops) =>
            {
                List<CyNukeReactorMono> reactors = CyNukeChargeManager.GetReactors(cyclops);

                if (reactors == null)
                    return;

                QuickLogger.Debug($"Handling CyNukeEnhancer at {this.HighestValue}");
                foreach (CyNukeReactorMono reactor in reactors)
                {
                    reactor.UpdateUpgradeLevel(this.HighestValue);
                }
            };
        }

        private static bool HasRoomToStrink(SubRoot cyclops, int availableSlots)
        {
            List<CyNukeReactorMono> reactors = CyNukeChargeManager.GetReactors(cyclops);

            if (reactors == null)
                return true;

            foreach (CyNukeReactorMono reactor in reactors)
            {
                if (reactor.TotalItemCount > availableSlots)
                {
                    if (Time.time > errorDelay)
                    {
                        errorDelay = Time.time + delayInterval;
                        ErrorMessage.AddMessage(CyNukReactorBuildable.CannotRemoveMsg());
                    }

                    return false;
                }
            }

            return true;
        }
    }
}
