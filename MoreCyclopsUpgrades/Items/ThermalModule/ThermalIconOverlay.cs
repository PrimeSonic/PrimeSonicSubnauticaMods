namespace MoreCyclopsUpgrades.Items.ThermalModule
{
    using CommonCyclopsUpgrades;

    internal class ThermalIconOverlay : AmbientEnergyIconOverlay<ThermalUpgradeHandler>
    {
        public ThermalIconOverlay(uGUI_ItemIcon icon, InventoryItem upgradeModule)
            : base(ThermalCharger.ChargerName, icon, upgradeModule)
        {
        }
    }
}
