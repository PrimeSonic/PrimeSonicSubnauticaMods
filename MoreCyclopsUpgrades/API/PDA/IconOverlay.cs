namespace MoreCyclopsUpgrades.API.PDA
{
    using UnityEngine;

    /// <summary>
    /// The base class that is used to provide simple text overlays on top of Cyclops Upgrade Console Equipment icons when viewed in the PDA.
    /// </summary>
    public abstract class IconOverlay
    {
        /// <summary>
        /// The text element anchored to <see cref="TextAnchor.UpperCenter"/>.
        /// </summary>
        public readonly IconOverlayText upperText;

        /// <summary>
        /// The text element anchored to <see cref="TextAnchor.MiddleCenter"/>.
        /// </summary>
        public readonly IconOverlayText middleText;

        /// <summary>
        /// The text element anchored to <see cref="TextAnchor.LowerCenter"/>.
        /// </summary>
        public readonly IconOverlayText lowerText;

        /// <summary>
        /// The actual UI gameobject that acts as the root to all these new UI elements.
        /// </summary>
        public readonly uGUI_ItemIcon uGuiIcon;

        /// <summary>
        /// The upgrade module's tech type.
        /// </summary>
        public readonly TechType techType;

        /// <summary>
        /// The upgrade module item itself.
        /// </summary>
        public readonly InventoryItem module;

        /// <summary>
        /// The cyclops sub where this is happening.
        /// </summary>
        public readonly SubRoot cyclops;

        /// <summary>
        /// Initializes a new instance of the <see cref="IconOverlay"/> class.
        /// </summary>
        /// <param name="icon">The PDA icon.</param>
        /// <param name="upgradeModule">The upgrade module item.</param>
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

        internal void Clear()
        {
            upperText.Clear();
            middleText.Clear();
            lowerText.Clear();
        }
    }
}
