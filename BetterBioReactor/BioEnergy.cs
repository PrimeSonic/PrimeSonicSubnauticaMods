namespace BetterBioReactor
{
    using UnityEngine;
    using UnityEngine.UI;

    internal class BioEnergy
    {
        public bool FullyConsumed => RemainingEnergy <= 0f;
        public string EnergyString => $"{Mathf.RoundToInt(RemainingEnergy)}/{MaxEnergy}";

        public Pickupable Pickupable;
        public float RemainingEnergy;
        public readonly float MaxEnergy;
        public int Size = 1;
        public Text DisplayText { get; set; }

        public BioEnergy(Pickupable pickupable, float currentEnergy, float originalEnergy)
        {
            Pickupable = pickupable;
            RemainingEnergy = currentEnergy;
            MaxEnergy = originalEnergy;
        }

        internal BioEnergy(Pickupable pickupable, float energy)
        {
            Pickupable = pickupable;
            RemainingEnergy = energy;
            MaxEnergy = BaseBioReactor.GetCharge(pickupable.GetTechType());
        }

        public void UpdateInventoryText()
        {
            if (this.DisplayText is null)
                return;

            this.DisplayText.text = this.EnergyString;
        }

        public void AddDisplayText(uGUI_ItemIcon icon)
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
            text.fontSize = 14 + Size;
            text.alignment = TextAnchor.MiddleCenter;
            text.color = Color.yellow;

            Outline outline = textGO.AddComponent<Outline>();
            outline.effectColor = Color.black;

            RectTransform rectTransform = text.GetComponent<RectTransform>();
            rectTransform.localScale = Vector3.one;
            rectTransform.anchoredPosition3D = Vector3.zero;

            this.DisplayText = text;
        }
    }
}