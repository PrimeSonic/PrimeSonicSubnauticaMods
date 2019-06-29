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

        /// <summary>
        /// Gets or sets the font used for this <see cref="Text"/> element.<para/>
        /// Defaults to Arial.
        /// </summary>
        /// <value>
        /// The font.
        /// </value>
        public Font Font
        {
            get => text.font;
            set
            {
                text.material = value.material;
                text.font = value;
            }
        }

        /// <summary>
        /// Gets or sets the text string used for this <see cref="Text"/> element.
        /// </summary>
        /// <value>
        /// The text string.
        /// </value>
        public string TextString
        {
            get => text.text;
            set => text.text = value;
        }

        /// <summary>
        /// Gets or sets the size of the text font used for this <see cref="Text"/> element.<para/>
        /// Defaults to 18.
        /// </summary>
        /// <value>
        /// The size of the text font.
        /// </value>
        public int FontSize
        {
            get => text.fontSize;
            set => text.fontSize = value;
        }

        /// <summary>
        /// Gets or sets the font style used for this <see cref="Text"/> element.<para/>
        /// Defaults to <see cref="FontStyle.Normal"/>.
        /// </summary>
        /// <value>
        /// The font style.
        /// </value>
        public FontStyle FontStyle
        {
            get => text.fontStyle;
            set => text.fontStyle = value;
        }

        /// <summary>
        /// Gets or sets the color of the text used for this <see cref="Text"/> element.<para/>
        /// Defaults to <see cref="Color.white"/>.
        /// </summary>
        /// <value>
        /// The color of the text.
        /// </value>
        public Color TextColor
        {
            get => text.color;
            set => text.color = value;
        }

        /// <summary>
        /// Gets or sets the text outline used for this <see cref="Text"/> element.<para/>
        /// Defaults to <see cref="Color.black"/>.
        /// </summary>
        /// <value>
        /// The text outline.
        /// </value>
        public Color TextOutline
        {
            get => outline.effectColor;
            set => outline.effectColor = value;
        }

        internal IconOverlayText(uGUI_ItemIcon icon, TextAnchor anchor)
        {
            textGO = new GameObject("PdaIconOverlay");
            textGO.transform.SetParent(icon.transform, false);

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
