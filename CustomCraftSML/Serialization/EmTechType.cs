namespace CustomCraftSML.Serialization
{
    using System;
    using Common;
    using EasyMarkup;

    public class EmTechType : EmProperty
    {
        protected TechType ObjValue;

        public TechType ItemID => Value;

        public TechType Value
        {
            get => ObjValue;
            set
            {
                ObjValue = value;
                SerializedValue = ObjValue.ToString();
            }
        }

        public EmTechType(string key)
        {
            Key = key;
        }

        public EmTechType(string key, TechType value)
        {
            Key = key;
            Value = value;
        }

        protected override string ExtractValue(StringBuffer fullString)
        {
            string serialValue = base.ExtractValue(fullString);

            Value = ConvertFromSerial(serialValue);

            return serialValue;
        }

        public TechType ConvertFromSerial(string value)
        {
            return (TechType)Enum.Parse(typeof(TechType), value.WithFirstUpper());
        }

        internal override EmProperty Copy() => new EmTechType(Key);
    }
}
