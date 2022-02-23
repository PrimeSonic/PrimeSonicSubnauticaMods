namespace MoreCyclopsUpgrades.API.StatusIcons
{
    using UnityEngine;

    /// <summary>
    /// Parent class for creating your own status indicator icon.
    /// </summary>
    public abstract class CyclopsStatusIcon
    {
        /// <summary>
        /// A reference to the the cyclops <see cref="SubRoot"/> instance.
        /// </summary>
        public readonly SubRoot Cyclops;

        /// <summary>
        /// Initializes a new instance of the <see cref="CyclopsStatusIcon"/> class.
        /// </summary>
        /// <param name="cyclops">The cyclops.</param>
        protected CyclopsStatusIcon(SubRoot cyclops)
        {
            Cyclops = cyclops;
        }

        /// <summary>
        /// Get a value indicating whether the status icon should to be visible.
        /// </summary>
        /// <value>
        ///   <c>true</c> if can be visible in the Cyclops displays; otherwise, <c>false</c>.
        /// </value>
        public abstract bool ShowStatusIcon { get; }

        /// <summary>
        /// Gets the sprite to use for status icon. This will only be called when <see cref="ShowStatusIcon"/> returns <c>true</c>.
        /// </summary>
        /// <returns>A new <see cref="Atlas.Sprite"/> to be used in the Cyclops Helm and Holographic HUDs.</returns>
        public abstract Atlas.Sprite StatusSprite();

        /// <summary>
        /// Returns the text to use under the status icon. This will only be called when <see cref="ShowStatusIcon"/> returns <c>true</c>.
        /// </summary>
        /// <returns>A <see cref="string"/>, ready to use for in-game text. Should be limited to numeric values if possible.</returns>
        public abstract string StatusText();

        /// <summary>
        /// Returns the color of the text used under the status icon. This will only be called when <see cref="ShowStatusIcon"/> returns <c>true</c>.
        /// </summary>
        /// <returns>A Unity <see cref="Color"/> value for the text. When in doubt, just set this to white.</returns>
        public abstract Color StatusTextColor();
    }
}
