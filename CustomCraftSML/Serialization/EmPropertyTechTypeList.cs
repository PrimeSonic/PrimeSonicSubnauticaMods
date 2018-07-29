namespace CustomCraft2SML.Serialization
{
    using System.Collections.Generic;
    using Common.EasyMarkup;

    public class EmPropertyTechTypeList : EmPropertyList<TechType>
    {
        public EmPropertyTechTypeList(string key) : base(key)
        {
        }

        public EmPropertyTechTypeList(string key, IEnumerable<TechType> values) : base(key, values)
        {
        }

        public override TechType ConvertFromSerial(string value)
        {
            if (TechTypeExtensions.FromString(value, out var tType, true))
                return tType;
            else
                return TechType.None;
        }

        internal override EmProperty Copy() => new EmPropertyTechTypeList(Key, this.Values);

        public override string ToString()
        {
            if (Count == 0)
                return string.Empty;//$" {SpChar_CommentBlock} The {Key} list was empty {SpChar_CommentBlock} ";            

            return base.ToString();
        }
    }
}
