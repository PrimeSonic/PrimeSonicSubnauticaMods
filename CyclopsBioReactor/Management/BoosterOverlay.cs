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
            chargeHandler = MCUServices.Find.CyclopsCharger<BioChargeHandler>(base.cyclops, BioChargeHandler.ChargerName);
            upgradeHandler = MCUServices.Find.CyclopsUpgradeHandler<BioBoosterUpgradeHandler>(base.cyclops, base.techType);
        }

        public override void UpdateText()
        {
            middleText.TextString = chargeHandler.GetIndicatorText();
            lowerText.TextString = $"{upgradeHandler.Count}/{upgradeHandler.MaxCount}";
        }
    }
}
