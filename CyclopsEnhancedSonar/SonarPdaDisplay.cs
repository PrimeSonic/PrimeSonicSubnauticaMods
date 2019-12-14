namespace CyclopsEnhancedSonar
{
    using MoreCyclopsUpgrades.API;
    using MoreCyclopsUpgrades.API.PDA;

    internal class SonarPdaDisplay : IconOverlay
    {
        public SonarPdaDisplay(uGUI_ItemIcon icon, InventoryItem upgradeModule)
            : base(icon, upgradeModule)
        {
        }

        public override void UpdateText()
        {
            int upgradeCount = MCUServices.CrossMod.GetUpgradeCount(base.Cyclops, TechType.CyclopsSonarModule);

            base.LowerText.FontSize = 14 + (3 * upgradeCount);
            base.LowerText.TextString = $"{upgradeCount}/{SonarUpgradeHandler.MaxUpgrades}";
        }
    }
}
