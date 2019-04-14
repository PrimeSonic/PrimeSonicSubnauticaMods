namespace CyclopsNuclearReactor
{
    using UnityEngine;
    using UnityEngine.UI;

    internal partial class CyNukeReactorMono
    {
        internal class SlotData
        {
            internal TechType techType;
            internal float charge;
            internal Pickupable pickupable;
            internal Text text;

            public SlotData()
            {
                techType = TechType.None;
                charge = -1f;
                pickupable = null;
                text = null;
            }

            public SlotData(float charge, Pickupable pickupable)
            {
                techType = pickupable.GetTechType();
                this.charge = charge;
                this.pickupable = pickupable;
                text = null;
            }

            public void AddDisplayText(uGUI_EquipmentSlot icon)
            {
                // This code was made possible with the help of Waisie Milliams Hah
                var arial = (Font)Resources.GetBuiltinResource(typeof(Font), "Arial.ttf");

                var textGO = new GameObject("EnergyLabel");

                textGO.transform.parent = icon.transform;
                textGO.AddComponent<Text>();

                Text text = textGO.GetComponent<Text>();
                text.font = arial;
                text.material = arial.material;
                text.text = string.Empty;
                text.fontSize = 16;
                text.alignment = TextAnchor.MiddleCenter;
                text.color = Color.white;

                Outline outline = textGO.AddComponent<Outline>();
                outline.effectColor = Color.black;

                RectTransform rectTransform = text.GetComponent<RectTransform>();
                rectTransform.localScale = Vector3.one;
                rectTransform.anchoredPosition3D = Vector3.zero;

                this.text = text;
            }
        }
    }
}
