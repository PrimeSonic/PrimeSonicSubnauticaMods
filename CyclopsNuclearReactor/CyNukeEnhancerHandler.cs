namespace CyclopsNuclearReactor
{
    using Common;
    using MoreCyclopsUpgrades.CyclopsUpgrades;
    using System.Collections.Generic;

    internal class CyNukeEnhancerHandler : TieredUpgradesHandlerCollection<int>
    {
        private static readonly float errorDelay = 0f;
        private const float delayInterval = 10f;

        private readonly TieredUpgradeHandler<int> tier1;
        private readonly TieredUpgradeHandler<int> tier2;

        public CyNukeEnhancerHandler() : base(0)
        {
            // CyNukeEnhancerMk1
            tier1 = CreateTier(CyNukeEnhancerMk1.TechTypeID, 1);
            tier1.MaxCount = 1;

            // CyNukeEnhancerMk2
            tier2 = CreateTier(CyNukeEnhancerMk2.TechTypeID, 2);
            tier2.MaxCount = 1;

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
    }
}
