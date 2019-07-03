namespace CyclopsThermalUpgrades.Management
{
    using CommonCyclopsUpgrades;

    internal class ThermalIconOverlay : AmbientEnergyIconOverlay<ThermalUpgradeHandler>
    {
        public ThermalIconOverlay(uGUI_ItemIcon icon, InventoryItem upgradeModule) : base(icon, upgradeModule)
        {
        }
    }
}
