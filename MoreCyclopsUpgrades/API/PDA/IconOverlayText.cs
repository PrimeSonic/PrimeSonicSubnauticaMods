namespace MoreCyclopsUpgrades.API.PDA
{
    using UnityEngine;
    using UnityEngine.UI;
    
    internal class IconOverlayText : IIconOverlayText
    {
        private static readonly Font ArialFont = (Font)Resources.GetBuiltinResource(typeof(Font), "Arial.ttf");

        private readonly GameObject textGO;
        private readonly Text text;
        private readonly Outline outline;

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
            textGO = new GameObject("PdaIconOverlay");
            textGO.transform.SetParent(icon.transform, false);

            CanvasGroup group = textGO.AddComponent<CanvasGroup>();
            group.blocksRaycasts = false;

            text = textGO.AddComponent<Text>();
            text.font = ArialFont;
            text.material = ArialFont.material;
            text.text = string.Empty;
            text.fontSize = 18;
            text.fontStyle = FontStyle.Normal;
            text.alignment = anchor;
            text.color = Color.white;

            outline = textGO.AddComponent<Outline>();
            outline.effectColor = Color.black;

            RectTransform rectTransform = text.GetComponent<RectTransform>();
            rectTransform.localScale = Vector3.one;
            rectTransform.anchoredPosition3D = Vector3.zero;
        }

        internal void Clear()
        {
            GameObject.Destroy(textGO);
            GameObject.Destroy(text);
            GameObject.Destroy(outline);
        }
    }
}
