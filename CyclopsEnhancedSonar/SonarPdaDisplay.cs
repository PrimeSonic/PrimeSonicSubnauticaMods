namespace CyclopsEnhancedSonar
{
    using MoreCyclopsUpgrades.API;
    using MoreCyclopsUpgrades.API.PDA;

    internal class SonarPdaDisplay : IconOverlay
    {
        public const string SpeedUpKey = "CySnrSpdUp";
        public const string SpeedUpText = "Sonar Speed Up";

        private readonly string langSpeedUpText;

        public SonarPdaDisplay(uGUI_ItemIcon icon, InventoryItem upgradeModule)
            : base(icon, upgradeModule)
        {
            base.UpperText.FontSize = 12;
            langSpeedUpText = Language.main.Get(SpeedUpKey);
        }

        public override void UpdateText()
        {
            int upgradeCount = MCUServices.CrossMod.GetUpgradeCount(base.Cyclops, TechType.CyclopsSonarModule);

            base.LowerText.FontSize = 14 + (3 * upgradeCount);
            base.LowerText.TextString = $"{upgradeCount}/{SonarUpgradeHandler.MaxUpgrades}";

            base.UpperText.TextString = upgradeCount == SonarUpgradeHandler.MaxUpgrades 
                ? $"[{langSpeedUpText}]" 
                : string.Empty;
        }
    }
}
