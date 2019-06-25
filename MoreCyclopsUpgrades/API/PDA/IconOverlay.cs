namespace MoreCyclopsUpgrades.API.PDA
{
    using UnityEngine;

    public abstract class IconOverlay
    {
        public readonly IconOverlayText upperText;
        public readonly IconOverlayText middleText;
        public readonly IconOverlayText lowerText;

        public readonly uGUI_ItemIcon uGuiIcon;
        public readonly TechType techType;
        public readonly InventoryItem module;
        public readonly SubRoot cyclops;

        protected IconOverlay(uGUI_ItemIcon icon, InventoryItem upgradeModule)
        {
            module = upgradeModule;
            techType = upgradeModule.item.GetTechType();
            uGuiIcon = icon;
            cyclops = Player.main.currentSub;

            upperText = new IconOverlayText(icon, TextAnchor.UpperCenter);
            middleText = new IconOverlayText(icon, TextAnchor.MiddleCenter);
            lowerText = new IconOverlayText(icon, TextAnchor.LowerCenter);
        }

        /// <summary>
        /// Update the values in <see cref="upperText"/>, <see cref="middleText"/>, and <see cref="lowerText"/> in this method.
        /// </summary>
        public abstract void UpdateText();
    }
}
