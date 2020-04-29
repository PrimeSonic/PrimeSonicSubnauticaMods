namespace MoreCyclopsUpgrades.AuxConsole
{
    using System.Collections.Generic;
    using UnityEngine;

    internal class ModuleIconDisplay
    {
        public readonly Dictionary<string, Canvas> IconDisplays;

        public ModuleIconDisplay(Canvas canvas1, Canvas canvas2, Canvas canvas3,
                                  Canvas canvas4, Canvas canvas5, Canvas canvas6)
        {
            IconDisplays = new Dictionary<string, Canvas>(6)
            {
               { "Module1", canvas1 },
               { "Module2", canvas2 },
               { "Module3", canvas3 },
               { "Module4", canvas4 },
               { "Module5", canvas5 },
               { "Module6", canvas6 }
            };
        }

        public Canvas this[string slot] => IconDisplays[slot];

        public void EnableIcon(string slot, TechType techType)
        {
            GameObject canvasObject = this[slot].gameObject;
            uGUI_Icon icon = canvasObject.GetComponent<uGUI_Icon>();

            if (techType != TechType.None)
            {
                icon.sprite = SpriteManager.Get(techType);
                icon.enabled = true;
                canvasObject.SetActive(true);
            }
        }

        public void DisableIcon(string slot)
        {
            GameObject canvasObject = this[slot].gameObject;
            uGUI_Icon icon = canvasObject.GetComponent<uGUI_Icon>();

            canvasObject.SetActive(false);
            icon.enabled = false;
            icon.sprite = null; // Clear the sprite when empty
        }
    }
}
