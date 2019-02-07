namespace Common.EasyMarkup
{
    using System;
    using System.Globalization;
    using Common;

    public class EmProperty<T> : EmProperty, IValueConfirmation where T : IConvertible
    {
        public string InLineComment { get; set; } = null;

        public bool Optional { get; set; } = false;

        public bool HasValue { get; set; } = false;

        protected T DefaultValue { get; private set; } = default(T);

        private T ObjValue;

        public T Value
        {
            get => ObjValue;
            set
            {
                ObjValue = value;
                HasValue = true;
                SerializedValue = ObjValue?.ToString(CultureInfo.InvariantCulture);
            }
        }

        public EmProperty(string key, T defaultValue = default(T))
        {
            Key = key;
            ObjValue = defaultValue;
            DefaultValue = defaultValue;
            SerializedValue = ObjValue?.ToString(CultureInfo.InvariantCulture);
        }

        public override string ToString()
        {
            if (!HasValue && Optional)
                return string.Empty;

            return $"{base.ToString()}{EmUtils.CommentText(InLineComment)}";
        }

        protected override string ExtractValue(StringBuffer fullString)
        {
            string serialValue = base.ExtractValue(fullString);

            Value = ConvertFromSerial(serialValue);

            HasValue = true;

            return serialValue;
        }

        public virtual T ConvertFromSerial(string value)
        {
            var type = typeof(T);

            if (type.IsEnum)
                return (T)Enum.Parse(type, value, true);
            else
                return (T)Convert.ChangeType(value, typeof(T));
        }

        internal override EmProperty Copy()
        {
            if (HasValue)
                return new EmProperty<T>(Key, ObjValue);

            return new EmProperty<T>(Key, DefaultValue);
        }

        internal override bool ValueEquals(EmProperty other)
        {
            if (other is EmProperty<T> otherTyped)
            {
                return this.Value.Equals(otherTyped.Value);
            }

            return false;
        }
    }

}

