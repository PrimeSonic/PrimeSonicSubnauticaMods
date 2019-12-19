namespace CyclopsNuclearReactor
{
    using MoreCyclopsUpgrades.API;
    using UnityEngine;
    using UnityEngine.UI;

    internal class SlotData
    {
        private static readonly Font arialFont = (Font)Resources.GetBuiltinResource(typeof(Font), "Arial.ttf");

        internal const float EmptySlotCharge = -1f;

        internal TechType TechTypeID;
        internal float Charge;
        internal Pickupable Item;
        internal Text InfoDisplay;


        public SlotData()
        {
            TechTypeID = TechType.None;
            Charge = EmptySlotCharge;
            Item = null;
            InfoDisplay = null;
        }

        public SlotData(float charge, Pickupable pickupable)
        {
            TechTypeID = pickupable.GetTechType();
            Charge = charge;
            Item = pickupable;
            InfoDisplay = null;            
        }

        public bool HasPower()
        {
            return TechTypeID == TechType.ReactorRod &&
                   Charge > MCUServices.MinimalPowerValue;
        }

        public void AddDisplayText(uGUI_ItemIcon icon)
        {
            // This code was made possible with the help of Waisie Milliams Hah

            var textGO = new GameObject("EnergyLabel");

            textGO.transform.parent = icon.transform;
            textGO.AddComponent<Text>();

            Text text = textGO.GetComponent<Text>();
            text.font = arialFont;
            text.material = arialFont.material;
            text.text = string.Empty;
            text.fontSize = 16;
            text.alignment = TextAnchor.MiddleCenter;
            text.color = Color.white;

            Outline outline = textGO.AddComponent<Outline>();
            outline.effectColor = Color.black;

            RectTransform rectTransform = text.GetComponent<RectTransform>();
            rectTransform.localScale = Vector3.one;
            rectTransform.anchoredPosition3D = Vector3.zero;

            InfoDisplay = text;
        }
    }
}
