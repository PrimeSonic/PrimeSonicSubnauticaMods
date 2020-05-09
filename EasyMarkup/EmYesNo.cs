namespace EasyMarkup
{
    internal class EmYesNo : EmProperty<bool>
    {
        public EmYesNo(string key, bool defaultValue = false) : base(key, defaultValue)
        {
        }

        public override bool ConvertFromSerial(string value)
        {
            bool retValue;

            switch (value.ToUpperInvariant())
            {
                case "YES":
                case "TRUE":
                    retValue = true;
                    break;
                case "NO":
                case "FALSE":
                    retValue = false;
                    break;
                default:
                    retValue = this.DefaultValue;
                    break;
            }

            SerializedValue = retValue ? "YES" : "NO";

            return retValue;
        }

        public override string ToString()
        {
            SerializedValue = this.Value ? "YES" : "NO";

            return base.ToString();
        }

        internal override EmProperty Copy()
        {
            if (this.HasValue)
                return new EmYesNo(this.Key, this.Value) { Optional = this.Optional };

            return new EmYesNo(this.Key, this.DefaultValue) { Optional = this.Optional };
        }
    }
}
