namespace CyclopsSimpleSolar
{
    using MoreCyclopsUpgrades.API;
    using MoreCyclopsUpgrades.API.PDA;

    internal class SolarPdaOverlay : IconOverlay
    {
        private readonly CySolarChargeManager cySolarChargeManager;

        public SolarPdaOverlay(uGUI_ItemIcon icon, InventoryItem upgradeModule)
            : base(icon, upgradeModule)
        {
            cySolarChargeManager = MCUServices.Find.CyclopsCharger<CySolarChargeManager>(base.Cyclops);
        }

        public override void UpdateText()
        {
            if (cySolarChargeManager.SolarEnergyAvailable)
            {
                base.MiddleText.FontSize = 16;
                base.MiddleText.TextColor = cySolarChargeManager.StatusTextColor();
                base.MiddleText.TextString = cySolarChargeManager.StatusText();
            }
            else
            {
                base.MiddleText.TextString = string.Empty;
            }
        }
    }
}
