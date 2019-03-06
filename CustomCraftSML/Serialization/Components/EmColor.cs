namespace CustomCraft2SML.Serialization.Components
{
    using Common.EasyMarkup;
    using System.Collections.Generic;
    using UnityEngine;

    internal class EmColorRGB : EmPropertyList<float>
    {
        public const string MainKey = "ColorRGB";

        public EmColorRGB() : this(MainKey, new float[3] { 0, 0, 0 })
        {
        }

        public EmColorRGB(string key, IEnumerable<float> values) : base(key, values)
        {
        }

        public EmColorRGB(string key) : base(key)
        {
        }

        internal Color GetColor()
        {
            if (this.HasValidColor)
            {
                return new Color(this.Values[0], this.Values[1], this.Values[2]);
            }

            return Color.black; // Fallback
        }

        internal bool HasValidColor => this.HasValue && this.Values.Count == 3;

        internal override EmProperty Copy()
        {
            return new EmColorRGB(this.Key, this.Values);
        }
    }
}
