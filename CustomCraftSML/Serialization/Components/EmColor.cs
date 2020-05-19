namespace CustomCraft2SML.Serialization.Components
{
    using System.Collections.Generic;
    using EasyMarkup;
    using UnityEngine;

    internal class EmColorRGB : EmPropertyList<float>
    {
        public const string MainKey = "ColorRGB";

        public EmColorRGB() : this(MainKey, new float[0])
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
            if (this.HasValue)
            {
                if (this.Values.Count == 3)
                    return new Color(this.Values[0], this.Values[1], this.Values[2]);
                else // 4
                    return new Color(this.Values[0], this.Values[1], this.Values[2], this.Values[3]);
            }

            return new Color(0, 0, 0, 0); // Fallback. Should be invisible.
        }

        public override bool HasValue
        {
            get
            {
                if (this.Values.Count == 3 || this.Values.Count == 4)
                {
                    foreach (float value in this.Values)
                    {
                        if (value < 0f || value > 1f)
                            return false;
                    }

                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public override string ToString()
        {
            if (this.Values.Count != 3)
                return string.Empty;

            return base.ToString();
        }

        internal override EmProperty Copy()
        {
            return new EmColorRGB(this.Key, this.Values);
        }
    }
}
