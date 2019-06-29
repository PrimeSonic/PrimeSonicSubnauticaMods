namespace CyclopsNuclearReactor
{
    using Common;
    using MoreCyclopsUpgrades.API;
    using MoreCyclopsUpgrades.API.Upgrades;

    internal class CyNukeEnhancerHandler : TieredGroupHandler<int>
    {
        private const int NoUpgradesValue = 0;
        private const int Mk1UpgradeValue = 1;
        private const int Mk2UpgradeValue = 2;

        private readonly TieredUpgradeHandler<int> tier1;
        private readonly TieredUpgradeHandler<int> tier2;

        private CyNukeChargeManager manager;
        private CyNukeChargeManager ChargeManager => manager ?? 
            (manager = MCUServices.Find.CyclopsCharger<CyNukeChargeManager>(cyclops, CyNukeChargeManager.ChargerName));

        public CyNukeEnhancerHandler(SubRoot cyclops) : base(NoUpgradesValue, cyclops)
        {
            // CyNukeEnhancerMk1
            tier1 = CreateTier(CyNukeEnhancerMk1.TechTypeID, Mk1UpgradeValue);
            tier1.MaxCount = 1;

            // CyNukeEnhancerMk2
            tier2 = CreateTier(CyNukeEnhancerMk2.TechTypeID, Mk2UpgradeValue);
            tier2.MaxCount = 1;

            OnUpgradeCounted = () => // Doing the final sync during HandleUpgrades may be unorthodox, but it somehow doesn't want to work any other way.
            {
                if (manager == null)
                    manager = MCUServices.Find.CyclopsCharger<CyNukeChargeManager>(cyclops, CyNukeChargeManager.ChargerName);

                OnUpgradeCounted = null; // This method only needs to be called once
            };
            OnFinishedUpgrades = EnhanceCyNukeReactors;
        }

        private void EnhanceCyNukeReactors()
        {
            QuickLogger.Debug($"Handling CyNukeEnhancer at {this.HighestValue}");

            if (this.ChargeManager != null)
            {
                foreach (CyNukeReactorMono reactor in this.ChargeManager.CyNukeReactors)
                    reactor?.UpdateUpgradeLevel(this.HighestValue);
            }
            else
            {
                QuickLogger.Warning("CyNukeChargeManager still not found!");
            }
        }
    }
}
