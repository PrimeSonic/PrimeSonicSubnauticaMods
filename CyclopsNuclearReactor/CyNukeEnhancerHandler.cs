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
        private CyNukeManager Manager => manager ?? (manager = MCUServices.Find.AuxCyclopsManager<CyNukeManager>(base.Cyclops));

        public CyNukeEnhancerHandler(SubRoot cyclops) : base(NoUpgradesValue, cyclops)
        {
            tier1 = CreateTier(CyNukeEnhancerMk1.TechTypeID, Mk1UpgradeValue);
            tier1.MaxCount = 1;
            tier2 = CreateTier(CyNukeEnhancerMk2.TechTypeID, Mk2UpgradeValue);
            tier2.MaxCount = 1;

            OnFinishedUpgrades = () =>
            {
                MCUServices.Logger.Debug($"Handling all CyNukeEnhancers at {this.HighestValue}");

                this.Manager?.ApplyToAll((reactor) => reactor.UpdateUpgradeLevel(this.HighestValue));
            };
        }
    }
}
