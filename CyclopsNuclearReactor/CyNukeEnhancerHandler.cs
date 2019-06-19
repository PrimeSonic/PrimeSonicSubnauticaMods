namespace CyclopsNuclearReactor
{
    using Common;
    using MoreCyclopsUpgrades.API;
    using MoreCyclopsUpgrades.API.Upgrades;
    using System.Collections.Generic;

    internal class CyNukeEnhancerHandler : TieredGroupHandler<int>
    {
        private const int NoUpgradesValue = 0;
        private const int Mk1UpgradeValue = 1;
        private const int Mk2UpgradeValue = 2;

        private readonly TieredUpgradeHandler<int> tier1;
        private readonly TieredUpgradeHandler<int> tier2;

        private readonly CyNukeChargeManager manager;

        public CyNukeEnhancerHandler(SubRoot cyclops) : base(NoUpgradesValue, cyclops)
        {
            manager = MCUServices.Find.AuxCyclopsManager<CyNukeChargeManager>(cyclops, CyNukeChargeManager.ChargerName);

            // CyNukeEnhancerMk1
            tier1 = CreateTier(CyNukeEnhancerMk1.TechTypeID, Mk1UpgradeValue);
            tier1.MaxCount = 1;

            // CyNukeEnhancerMk2
            tier2 = CreateTier(CyNukeEnhancerMk2.TechTypeID, Mk2UpgradeValue);
            tier2.MaxCount = 1;

            OnUpgradeCounted = (Equipment modules, string slot) =>
            {
                manager.UpgradeHandler = this; // Link this to the upgrade manager
                OnUpgradeCounted = null; // This method only needs to be called once
            };

            // Collection
            OnFinishedWithUpgrades += () =>
            {
                List<CyNukeReactorMono> reactors = manager.CyNukeReactors;

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
