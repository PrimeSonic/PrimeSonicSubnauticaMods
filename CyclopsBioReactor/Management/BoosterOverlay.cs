namespace CyclopsBioReactor.Management
{
    using MoreCyclopsUpgrades.API;
    using MoreCyclopsUpgrades.API.PDA;
    using UnityEngine;

    internal class BoosterOverlay : IconOverlay
    {
        private readonly BioBoosterUpgradeHandler upgradeHandler;

        public BoosterOverlay(uGUI_ItemIcon icon, InventoryItem upgradeModule)
            : base(icon, upgradeModule)
        {
            upgradeHandler = MCUServices.Find.CyclopsUpgradeHandler<BioBoosterUpgradeHandler>(cyclops, this.techType);
        }

        public override void UpdateText()
        {
            int currentCount = upgradeHandler.Count;
            int maxCount = upgradeHandler.MaxCount;
            lowerText.TextString = $"{currentCount}/{maxCount}";
            lowerText.TextColor = Color.green;
        }
    }
}
