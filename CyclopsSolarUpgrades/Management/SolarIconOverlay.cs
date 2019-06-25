namespace CyclopsSolarUpgrades.Management
{
    using CommonCyclopsUpgrades;

    internal class SolarIconOverlay : AmbientEnergyIconOverlay<SolarUpgradeHandler>
    {
        public SolarIconOverlay(uGUI_ItemIcon icon, InventoryItem upgradeModule)
            : base(SolarCharger.ChargerName, icon, upgradeModule)
        {
        }
    }
}
