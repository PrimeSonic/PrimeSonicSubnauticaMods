namespace CyclopsNuclearReactor
{
    using Common;
    using MoreCyclopsUpgrades.CyclopsUpgrades;
    using System.Collections.Generic;

    internal class CyNukeEnhancerHandler : TieredUpgradesHandlerCollection<int>
    {
        private const int NoUpgradesValue = 0;
        private const int Mk1UpgradeValue = 1;
        private const int Mk2UpgradeValue = 2;

        private readonly TieredUpgradeHandler<int> tier1;
        private readonly TieredUpgradeHandler<int> tier2;

        public CyNukeEnhancerHandler() : base(NoUpgradesValue)
        {
            // CyNukeEnhancerMk1
            tier1 = CreateTier(CyNukeEnhancerMk1.TechTypeID, Mk1UpgradeValue);
            tier1.MaxCount = 1;

            // CyNukeEnhancerMk2
            tier2 = CreateTier(CyNukeEnhancerMk2.TechTypeID, Mk2UpgradeValue);
            tier2.MaxCount = 1;

            OnUpgradeCounted = (SubRoot cyclops, Equipment modules, string slot) =>
            {
                var mgr = CyNukeChargeManager.GetManager(cyclops);
                mgr.UpgradeHandler = this; // Link this to the upgrade manager
                OnUpgradeCounted = null; // This method only needs to be called once
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
    }
}
