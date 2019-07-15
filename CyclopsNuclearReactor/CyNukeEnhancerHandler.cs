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

        private CyNukeManager manager;
        private CyNukeManager ChargeManager => manager ??
            (manager = MCUServices.Find.AuxCyclopsManager<CyNukeManager>(Cyclops));

        public CyNukeEnhancerHandler(SubRoot cyclops) : base(NoUpgradesValue, cyclops)
        {
            tier1 = CreateTier(CyNukeEnhancerMk1.TechTypeID, Mk1UpgradeValue);
            tier1.MaxCount = 1;
            tier2 = CreateTier(CyNukeEnhancerMk2.TechTypeID, Mk2UpgradeValue);
            tier2.MaxCount = 1;

            OnUpgradeCounted = () => // Doing the final sync during HandleUpgrades may be unorthodox, but it somehow doesn't want to work any other way.
            {
                if (manager == null)
                    manager = MCUServices.Find.AuxCyclopsManager<CyNukeManager>(cyclops);

                OnUpgradeCounted = null; // This method only needs to be called once
            };

            OnFinishedUpgrades = EnhanceCyNukeReactors;
        }

        private void EnhanceCyNukeReactors()
        {
            QuickLogger.Debug($"Handling CyNukeEnhancer at {this.HighestValue}");

            if (this.ChargeManager != null)
            {
                for (int r = 0; r < this.ChargeManager.CyNukeReactors.Count; r++)
                    this.ChargeManager.CyNukeReactors[r]?.UpdateUpgradeLevel(this.HighestValue);
            }
            else
            {
                QuickLogger.Warning("CyNukeChargeManager still not found!");
            }
        }
    }
}
