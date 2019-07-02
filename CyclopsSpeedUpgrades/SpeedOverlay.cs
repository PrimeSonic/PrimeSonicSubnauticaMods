namespace CyclopsSpeedUpgrades
{
    using MoreCyclopsUpgrades.API;
    using MoreCyclopsUpgrades.API.PDA;
    using UnityEngine;

    internal class SpeedOverlay : IconOverlay
    {
        private readonly SpeedHandler speedHandler;

        public SpeedOverlay(uGUI_ItemIcon icon, InventoryItem upgradeModule, CyclopsSpeedModule speedBooster) : base(icon, upgradeModule)
        {
            speedHandler = MCUServices.Find.CyclopsUpgradeHandler<SpeedHandler>(base.Cyclops, speedBooster.TechType);
        }

        public override void UpdateText()
        {
            base.UpperText.TextString = $"{Mathf.RoundToInt(speedHandler.CurrentModifier * 100f)}";

            base.LowerText.TextString = $"{speedHandler.Count}/{speedHandler.MaxCount}";
        }
    }
}
