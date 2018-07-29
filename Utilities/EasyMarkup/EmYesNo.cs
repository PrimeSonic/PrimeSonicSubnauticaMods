namespace Common.EasyMarkup
{
    public class EmYesNo : EmProperty<bool>
    {
        public bool ForceWrite { get; set; } = false;

        public EmYesNo(string key, bool defaultValue = false) : base(key, defaultValue)
        {
        }

        public override bool ConvertFromSerial(string value)
        {
            bool retValue;

            switch (value)
            {
                case "YES":
                case "yes":
                case "Yes":
                case "TRUE":
                case "True":
                case "true":
                    retValue = true;
                    break;
                case "NO":
                case "no":
                case "No":
                case "FALSE":
                case "False":
                case "false":
                    retValue = false;
                    break;
                default:
                    retValue = DefaultValue;
                    break;
            }

            this.SerializedValue = retValue ? "YES" : "NO";

            return retValue;
        }

        public override string ToString()
        {
            if (this.Value == DefaultValue && !ForceWrite)
                return string.Empty;

            this.SerializedValue = Value ? "YES" : "NO";

            return base.ToString();
        }

        internal override EmProperty Copy()
        {
            if (HasValue)
                return new EmYesNo(this.Key, this.Value);

            return new EmYesNo(this.Key, this.DefaultValue);
        }
    }
}
