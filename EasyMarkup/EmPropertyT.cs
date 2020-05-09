namespace EasyMarkup
{
    using System;
    using System.Globalization;

    internal class EmProperty<T> : EmProperty, IValueConfirmation where T : IConvertible
    {
        private bool hasValue = false;
        public string InLineComment { get; set; } = null;

        public override bool HasValue => hasValue;

        public T DefaultValue { get; set; } = default;

        private T ObjValue;

        private readonly Type DataType;

        public T Value
        {
            get => ObjValue;
            set
            {
                ObjValue = value;
                hasValue = value != null;

                SerializedValue = ObjValue?.ToString(CultureInfo.InvariantCulture);
            }
        }

        public EmProperty(string key, T defaultValue = default)
        {
            DataType = typeof(T);
            this.Key = key;
            ObjValue = defaultValue;
            this.DefaultValue = defaultValue;
            SerializedValue = ObjValue?.ToString(CultureInfo.InvariantCulture);
        }

        public override string ToString()
        {
            if (!this.HasValue && this.Optional)
                return string.Empty;

            return $"{base.ToString()}{EmUtils.CommentText(this.InLineComment)}";
        }

        protected override string ExtractValue(StringBuffer fullString)
        {
            string serialValue = base.ExtractValue(fullString);

            this.Value = ConvertFromSerial(serialValue);

            hasValue = true;

            return serialValue;
        }

        public virtual T ConvertFromSerial(string value)
        {
            try
            {
                return DataType.IsEnum
                    ? (T)Enum.Parse(DataType, value, true)
                    : (T)Convert.ChangeType(value, typeof(T));
            }
            catch
            {
                hasValue = false;
                return this.DefaultValue;
            }
        }

        internal override EmProperty Copy()
        {
            if (this.HasValue)
                return new EmProperty<T>(this.Key, ObjValue) { Optional = this.Optional };

            return new EmProperty<T>(this.Key, this.DefaultValue) { Optional = this.Optional };
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

