namespace CustomCraft2SML.Serialization
{
    using Common.EasyMarkup;

    public class EmPropertyTechType : EmProperty<TechType>
    {
        public EmPropertyTechType(string key) : base(key)
        {
        }

        public EmPropertyTechType(string key, TechType value) : base(key, value)
        {
            //SetNameAsComment(value.ToString(), value);
        }

        public override TechType ConvertFromSerial(string value)
        {
            if (TechTypeExtensions.FromString(value, out var tType, true))
            {
                //SetNameAsComment(value, tType);

                return tType;
            }
            else
                return TechType.None;
        }

        //private void SetNameAsComment(string value, TechType tType)
        //{
        //    string friendlyName = Language.main?.Get(tType);

        //    if (!string.IsNullOrEmpty(friendlyName) && friendlyName != value)
        //        this.InLineComment = friendlyName;
        //}

        internal override EmProperty Copy()
        {
            if (HasValue)
                return new EmPropertyTechType(Key, Value);

            return new EmPropertyTechType(Key);
        }
    }
}
