namespace CustomCraftSML.Serialization
{
    using System;
    using System.Collections.Generic;
    using Common;
    using EasyMarkup;

    public class EmTechTypeList : EmPropertyList<TechType>
    {
        public EmTechTypeList(string key) : base(key)
        {
        }

        public EmTechTypeList(string key, ICollection<TechType> values) : base(key, values)
        {
        }

        public override TechType ConvertFromSerial(string value)
        {
            TechType val = (TechType)Enum.Parse(typeof(TechType), value.WithFirstUpper());

            return val;
        }
    }
}
