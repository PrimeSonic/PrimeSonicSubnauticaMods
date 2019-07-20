namespace MoreCyclopsUpgrades.API.PDA
{
    using UnityEngine;
    using UnityEngine.UI;

    /// <summary>
    /// A class that exposes additional <see cref="Text"/> elements that MoreCyclopsUpgrades will overlay on top of an equipment icon in the PDA screen.
    /// </summary>
    public interface IIconOverlayText
    {
        /// <summary>
        /// Gets or sets the font used for this <see cref="Text"/> element.<para/>
        /// Defaults to Arial.
        /// </summary>
        /// <value>
        /// The font.
        /// </value>
        Font Font { get; set; }

        /// <summary>
        /// Gets or sets the size of the text font used for this <see cref="Text"/> element.<para/>
        /// Defaults to 18.
        /// </summary>
        /// <value>
        /// The size of the text font.
        /// </value>
        int FontSize { get; set; }

        /// <summary>
        /// Gets or sets the font style used for this <see cref="Text"/> element.<para/>
        /// Defaults to <see cref="FontStyle.Normal"/>.
        /// </summary>
        /// <value>
        /// The font style.
        /// </value>
        FontStyle FontStyle { get; set; }

        /// <summary>
        /// Gets or sets the color of the text used for this <see cref="Text"/> element.<para/>
        /// Defaults to <see cref="Color.white"/>.
        /// </summary>
        /// <value>
        /// The color of the text.
        /// </value>
        Color TextColor { get; set; }

        /// <summary>
        /// Gets or sets the text outline used for this <see cref="Text"/> element.<para/>
        /// Defaults to <see cref="Color.black"/>.
        /// </summary>
        /// <value>
        /// The text outline.
        /// </value>
        Color TextOutline { get; set; }

        /// <summary>
        /// Gets or sets the text string used for this <see cref="Text"/> element.
        /// </summary>
        /// <value>
        /// The text string.
        /// </value>
        string TextString { get; set; }
    }
}