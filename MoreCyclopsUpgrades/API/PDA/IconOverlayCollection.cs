namespace MoreCyclopsUpgrades.API.PDA
{
    using System.Collections.Generic;

    internal class IconOverlayCollection
    {
        private readonly List<IconOverlay> overlays = new List<IconOverlay>(6);

        public void Deactivate()
        {
            for (int i = 0; i < overlays.Count; i++)
                overlays[i].Clear();

            overlays.Clear();
        }

        public void UpdateText()
        {
            for (int i = 0; i < overlays.Count; i++)
                overlays[i].UpdateText();
        }

        public void Add(IconOverlay iconOverlay)
        {
            overlays.Add(iconOverlay);
        }
    }
}
