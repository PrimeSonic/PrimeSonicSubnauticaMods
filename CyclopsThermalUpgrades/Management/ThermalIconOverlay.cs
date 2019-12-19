namespace CyclopsThermalUpgrades.Management
{
    using MoreCyclopsUpgrades.API.AmbientEnergy;

    internal class ThermalIconOverlay : AmbientEnergyIconOverlay<ThermalUpgradeHandler, ThermalCharger>
    {
        public ThermalIconOverlay(uGUI_ItemIcon icon, InventoryItem upgradeModule) : base(icon, upgradeModule)
        {
        }
    }
}
