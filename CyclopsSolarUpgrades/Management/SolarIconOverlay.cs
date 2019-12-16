namespace CyclopsSolarUpgrades.Management
{
    using MoreCyclopsUpgrades.API.AmbientEnergy;

    internal class SolarIconOverlay : AmbientEnergyIconOverlay<SolarUpgradeHandler, SolarCharger>
    {
        public SolarIconOverlay(uGUI_ItemIcon icon, InventoryItem upgradeModule) : base(icon, upgradeModule)
        {
        }
    }
}
