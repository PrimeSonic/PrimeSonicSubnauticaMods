namespace CyclopsBioReactor.Management
{
    using MoreCyclopsUpgrades.API;
    using MoreCyclopsUpgrades.API.PDA;

    internal class BoosterOverlay : IconOverlay
    {
        private readonly BioBoosterUpgradeHandler upgradeHandler;
        private readonly BioChargeHandler chargeHandler;

        public BoosterOverlay(uGUI_ItemIcon icon, InventoryItem upgradeModule)
            : base(icon, upgradeModule)
        {
            chargeHandler = MCUServices.Find.CyclopsCharger<BioChargeHandler>(base.Cyclops, BioChargeHandler.ChargerName);
            upgradeHandler = MCUServices.Find.CyclopsUpgradeHandler<BioBoosterUpgradeHandler>(base.Cyclops, base.TechType);
        }

        public override void UpdateText()
        {
            MiddleText.TextString = chargeHandler.GetIndicatorText();
            LowerText.TextString = $"{upgradeHandler.Count}/{upgradeHandler.MaxCount}";
        }
    }
}
