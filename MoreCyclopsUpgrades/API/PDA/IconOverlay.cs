namespace MoreCyclopsUpgrades.API.PDA
{
    using UnityEngine;

    /// <summary>
    /// The base class that is used to provide simple text overlays on top of Cyclops Upgrade Console Equipment icons when viewed in the PDA.
    /// </summary>
    public abstract class IconOverlay
    {
        private IconOverlayText upper;
        private IconOverlayText middle;
        private IconOverlayText lower;

        /// <summary>
        /// The text element anchored to <see cref="TextAnchor.UpperCenter"/>.
        /// </summary>
        public readonly IIconOverlayText UpperText;

        /// <summary>
        /// The text element anchored to <see cref="TextAnchor.MiddleCenter"/>.
        /// </summary>
        public readonly IIconOverlayText MiddleText;

        /// <summary>
        /// The text element anchored to <see cref="TextAnchor.LowerCenter"/>.
        /// </summary>
        public readonly IIconOverlayText LowerText;

        /// <summary>
        /// The actual UI gameobject that acts as the root to all these new UI elements.
        /// </summary>
        public readonly uGUI_ItemIcon Icon;

        /// <summary>
        /// The upgrade module's tech type.
        /// </summary>
        public readonly TechType TechType;

        /// <summary>
        /// The upgrade module item itself.
        /// </summary>
        public readonly InventoryItem Item;

        /// <summary>
        /// The cyclops sub where this is happening.
        /// </summary>
        public readonly SubRoot Cyclops;

        /// <summary>
        /// Initializes a new instance of the <see cref="IconOverlay"/> class.
        /// </summary>
        /// <param name="icon">The PDA icon.</param>
        /// <param name="upgradeModule">The upgrade module item.</param>
        protected IconOverlay(uGUI_ItemIcon icon, InventoryItem upgradeModule)
        {
            Item = upgradeModule;
            TechType = upgradeModule.item.GetTechType();
            Icon = icon;
            Cyclops = Player.main.currentSub;

            UpperText = upper = new IconOverlayText(icon, TextAnchor.UpperCenter);
            MiddleText = middle = new IconOverlayText(icon, TextAnchor.MiddleCenter);
            LowerText = lower = new IconOverlayText(icon, TextAnchor.LowerCenter);
        }

        /// <summary>
        /// Update the values in <see cref="UpperText"/>, <see cref="MiddleText"/>, and <see cref="LowerText"/> in this method.
        /// </summary>
        public abstract void UpdateText();

        internal void Clear()
        {
            upper.Clear();
            middle.Clear();
            lower.Clear();
        }
    }
}
