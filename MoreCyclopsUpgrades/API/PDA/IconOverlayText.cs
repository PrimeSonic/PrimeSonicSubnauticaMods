namespace MoreCyclopsUpgrades.API.PDA
{
    using UnityEngine;
    using UnityEngine.UI;

    /// <summary>
    /// A class that exposes additional UI elements that MoreCyclopsUpgrades will overlay on top of an equipment icon in the PDA screen.
    /// </summary>
    public class IconOverlayText
    {
        private static readonly Font ArialFont = (Font)Resources.GetBuiltinResource(typeof(Font), "Arial.ttf");

        private readonly GameObject textGO;
        private readonly Text text;
        private readonly Outline outline;
        internal readonly uGUI_ItemIcon Icon;

        public Font Font
        {
            get => text.font;
            set
            {
                text.material = value.material;
                text.font = value;
            }
        }

        public string TextString
        {
            get => text.text;
            set => text.text = value;
        }

        public int FontSize
        {
            get => text.fontSize;
            set => text.fontSize = value;
        }

        public FontStyle FontStyle
        {
            get => text.fontStyle;
            set => text.fontStyle = value;
        }

        public Color TextColor
        {
            get => text.color;
            set => text.color = value;
        }

        public Color TextOutline
        {
            get => outline.effectColor;
            set => outline.effectColor = value;
        }

        internal IconOverlayText(uGUI_ItemIcon icon, TextAnchor anchor)
        {
            Icon = icon;

            textGO = new GameObject("PdaIconOverlay");
            textGO.transform.SetParent(icon.transform, false);

            text = textGO.AddComponent<Text>();
            text.font = ArialFont;
            text.material = ArialFont.material;
            text.text = string.Empty;
            text.fontSize = 16;
            text.fontStyle = FontStyle.Normal;
            text.alignment = anchor;
            text.color = Color.white;

            outline = textGO.AddComponent<Outline>();
            outline.effectColor = Color.black;

            RectTransform rectTransform = text.GetComponent<RectTransform>();
            rectTransform.localScale = Vector3.one;
            rectTransform.anchoredPosition3D = Vector3.zero;
        }
    }
}
