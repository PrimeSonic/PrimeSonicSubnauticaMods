namespace CustomBatteries.API
{
    using UnityEngine;

    public interface IModPluginPackCustomSkin : IModPluginPack
    {
        /// <summary>
        /// Gets the custom texture for batteries.
        /// </summary>
        /// <value>The battery skin.</value>
        Texture2D BatterySkin { get; }

        /// <summary>
        /// Gets the custom texture for power cells.
        /// </summary>
        /// <value>The power cell skin.</value>
        Texture2D PowerCellSkin { get; }
    }
}
