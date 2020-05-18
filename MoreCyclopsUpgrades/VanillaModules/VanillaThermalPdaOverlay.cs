namespace MoreCyclopsUpgrades.VanillaModules
{
    using MoreCyclopsUpgrades.API;
    using MoreCyclopsUpgrades.API.PDA;

    internal class VanillaThermalPdaOverlay : IconOverlay
    {
        private readonly VanillaThermalChargeManager thermalCharger;

        public VanillaThermalPdaOverlay(uGUI_ItemIcon icon, InventoryItem upgradeModule)
            : base(icon, upgradeModule)
        {
            thermalCharger = MCUServices.Find.CyclopsCharger<VanillaThermalChargeManager>(base.Cyclops);
        }

        public override void UpdateText()
        {
            if (thermalCharger.ThermalEnergyAvailable)
            {
                base.MiddleText.FontSize = 16;
                base.MiddleText.TextColor = thermalCharger.StatusTextColor();
                base.MiddleText.TextString = thermalCharger.StatusText();
            }
            else
            {
                base.MiddleText.TextString = string.Empty;
            }
        }
    }
}
