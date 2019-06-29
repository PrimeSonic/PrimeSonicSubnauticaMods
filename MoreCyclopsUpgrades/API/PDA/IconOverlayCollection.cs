namespace MoreCyclopsUpgrades.API.PDA
{
    using System.Collections.Generic;

    internal class IconOverlayCollection
    {
        private readonly List<IconOverlay> overlays = new List<IconOverlay>(6);

        public void Deactivate()
        {
            foreach (IconOverlay overlay in overlays)
                overlay.Clear();

            overlays.Clear();
        }

        public void UpdateText()
        {
            foreach (IconOverlay overlay in overlays)
                overlay.UpdateText();
        }

        public void Add(IconOverlay iconOverlay)
        {
            overlays.Add(iconOverlay);
        }
    }
}
