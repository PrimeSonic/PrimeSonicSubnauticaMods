namespace CustomCraftSML.Serialization.EasyMarkup
{
    public class EmPropertyTechType : EmProperty<TechType>
    {
        public EmPropertyTechType(string key) : base(key)
        {
        }

        public EmPropertyTechType(string key, TechType value) : base(key, value)
        {
        }

        public override TechType ConvertFromSerial(string value)
        {
            if (TechTypeExtensions.FromString(value, out var tType, true))
                return tType;
            else
                return TechType.None;
        }

        internal override EmProperty Copy()
        {
            if (ObjValue == default(TechType))
                return new EmPropertyTechType(Key);
            else
                return new EmPropertyTechType(Key, Value);
        }
    }
}
