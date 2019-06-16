namespace CyclopsEngineUpgrades.Handlers
{
    using CyclopsEngineUpgrades.Craftables;
    using MoreCyclopsUpgrades.API;

    internal class EngineHandler : TieredUpgradesHandlerCollection<int>
    {
        private const int BaseValue = 0;
        private readonly PowerManager powerManager;
        private readonly PowerUpgradeModuleMk2 moduleMk2;
        private readonly PowerUpgradeModuleMk3 moduleMk3;

        public EngineHandler(PowerUpgradeModuleMk2 upgradeMk2, PowerUpgradeModuleMk3 upgradeMk3, SubRoot cyclops)
            : base(BaseValue, cyclops)
        {
            powerManager = PowerManager.GetManager(cyclops);
            powerManager.EngineEfficientyUpgrades = this;
            moduleMk2 = upgradeMk2;
            moduleMk3 = upgradeMk3;            

            OnFinishedWithUpgrades = () =>
            {
                powerManager.UpdatePowerSpeedRating();
            };

            TieredUpgradeHandler<int> tier1 = CreateTier(TechType.PowerUpgradeModule, 1);
            tier1.MaxCount = 1;

            TieredUpgradeHandler<int> tier2 = CreateTier(moduleMk2.TechType, 2);
            tier2.MaxCount = 1;

            TieredUpgradeHandler<int> tier3 = CreateTier(moduleMk3.TechType, 3);
            tier3.MaxCount = 1;
        }
    }
}
