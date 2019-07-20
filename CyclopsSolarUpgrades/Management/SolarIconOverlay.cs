namespace CyclopsSolarUpgrades.Management
{
    using CommonCyclopsUpgrades;

    internal class SolarIconOverlay : AmbientEnergyIconOverlay<SolarUpgradeHandler, SolarCharger>
    {
        public SolarIconOverlay(uGUI_ItemIcon icon, InventoryItem upgradeModule) : base(icon, upgradeModule)
        {
        }
    }
}
