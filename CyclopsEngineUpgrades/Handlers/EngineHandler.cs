namespace CyclopsEngineUpgrades.Handlers
{
    using CyclopsEngineUpgrades.Craftables;
    using MoreCyclopsUpgrades.API;

    internal class EngineHandler : TieredGroupHandler<int>
    {
        private const int MaxAllowedPerTier = 1;
        private const int BaseValue = 0;
        private readonly EngineManager powerManager;

        public EngineHandler(PowerUpgradeModuleMk2 upgradeMk2, PowerUpgradeModuleMk3 upgradeMk3, SubRoot cyclops)
            : base(BaseValue, cyclops)
        {
            powerManager = EngineManager.GetManager(cyclops);
            powerManager.EngineEfficientyUpgrades = this;          

            OnFinishedWithUpgrades = () =>
            {
                powerManager.UpdatePowerSpeedRating();
            };

            TieredUpgradeHandler<int> tier1 = CreateTier(TechType.PowerUpgradeModule, 1);
            tier1.MaxCount = MaxAllowedPerTier;

            TieredUpgradeHandler<int> tier2 = CreateTier(upgradeMk2.TechType, 2);
            tier2.MaxCount = MaxAllowedPerTier;

            TieredUpgradeHandler<int> tier3 = CreateTier(upgradeMk3.TechType, 3);
            tier3.MaxCount = MaxAllowedPerTier;
        }
    }
}
