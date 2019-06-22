namespace CyclopsSolarUpgrades.Management
{
    using CyclopsSolarUpgrades.Craftables;
    using MoreCyclopsUpgrades.API.Upgrades;

    internal class SolarUpgrade : AmbientEnergyUpgradeHandler
    {
        public SolarUpgrade(TechType tier1Id, TechType tier2Id, SubRoot cyclops)
            : base(tier1Id, tier2Id, CyclopsSolarCharger.MaxSolarReached(), cyclops)
        {
        }
    }
}
