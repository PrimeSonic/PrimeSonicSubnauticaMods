namespace CyclopsSolarUpgrades.Management
{
    using MoreCyclopsUpgrades.API.AmbientEnergy;
    using CyclopsSolarUpgrades.Craftables;

    internal class SolarUpgradeHandler : AmbientEnergyUpgradeHandler
    {
        public SolarUpgradeHandler(TechType tier1Id, TechType tier2Id, SubRoot cyclops)
            : base(tier1Id, tier2Id, CyclopsSolarCharger.MaxSolarReached(), cyclops)
        {
        }
    }
}
