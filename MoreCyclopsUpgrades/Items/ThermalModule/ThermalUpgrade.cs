namespace MoreCyclopsUpgrades.Items.ThermalModule
{
    using MoreCyclopsUpgrades.API.Upgrades;

    internal class ThermalUpgrade : AmbientEnergyUpgradeHandler
    {
        public ThermalUpgrade(TechType tier1Id, TechType tier2Id, SubRoot cyclops)
            : base(tier1Id, tier2Id, CyclopsThermalChargerMk2.MaxThermalReached(), cyclops)
        {
        }
    }
}
