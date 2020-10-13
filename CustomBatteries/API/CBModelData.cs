
using UnityEngine;

namespace CustomBatteries.API
{
    public abstract class CBModelData
    {
        /// <summary>
        /// The custom skin for the item.<br/>
        /// This property is optional and will default to the standard texture for batteries or power cells.
        /// </summary>
        public virtual Texture2D CustomTexture => null;

        /// <summary>
        /// The custom bump texture for the item.<br/>
        /// This property is optional and will default to the standard bump texture for batteries or power cells.
        /// </summary>
        public virtual Texture2D CustomNormalMap => null;

        /// <summary>
        /// The custom Spec Texture for the item.<br/>
        /// This property is optional and will default to the standard spec texture for batteries or power cells.
        /// </summary>
        public virtual Texture2D CustomSpecMap => null;

        /// <summary>
        /// The custom lighting texture for the item.<br/>
        /// This property is optional and will default to the standard illum texture for batteries or power cells.
        /// </summary>
        public virtual Texture2D CustomIllumMap => null;

        /// <summary>
        /// The custom lighting strength for the item.<br/>
        /// This property is will default to 1.0f if the <see cref="CustomIllumMap"/> is set but will use the default value for batteries or power cells if no <see cref="CustomIllumMap"/> is set.
        /// </summary>
        public virtual float CustomIllumStrength => 1.0f;

        /// <summary>
        /// Override this value if you want your item to use the Ion battery or powercell model as its base.
        /// </summary>
        public virtual bool UseIonModelsAsBase => false;

    }
}
