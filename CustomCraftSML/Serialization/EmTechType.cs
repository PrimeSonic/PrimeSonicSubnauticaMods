namespace CustomCraftSML.Serialization
{
    using System;
    using Common;
    using EasyMarkup;

    public class EmTechType : EmProperty<TechType>
    {
        public TechType ItemID => Value;

        public EmTechType(string key) : base(key)
        {
        }

        public EmTechType(string key, TechType value) : base(key, value)
        {
        }

        public override TechType ConvertFromSerial(string value)
        {
            TechType val = (TechType)Enum.Parse(typeof(TechType), value.WithFirstUpper());

            return val;
        }
    }
}
