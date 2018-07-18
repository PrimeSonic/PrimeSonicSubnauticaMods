namespace Common.EasyMarkup
{
    using System.Collections.Generic;

    public class EmPropertyTechTypeList : EmPropertyList<TechType>
    {
        public EmPropertyTechTypeList(string key) : base(key)
        {
        }

        public EmPropertyTechTypeList(string key, ICollection<TechType> values) : base(key, values)
        {
        }

        public override TechType ConvertFromSerial(string value)
        {
            if (TechTypeExtensions.FromString(value, out var tType, true))
                return tType;
            else
                return TechType.None;
        }

        internal override EmProperty Copy() => new EmPropertyTechTypeList(Key);

        public override string ToString()
        {
            if (Count == 0)
                return string.Empty;//$" {SpChar_CommentBlock} The {Key} list was empty {SpChar_CommentBlock} ";            

            return base.ToString();
        }
    }
}
