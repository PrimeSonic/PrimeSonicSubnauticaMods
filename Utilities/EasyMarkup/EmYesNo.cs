namespace Common.EasyMarkup
{
    public class EmYesNo : EmProperty<bool>
    {
        public EmYesNo(string key, bool defaultValue = false) : base(key, defaultValue)
        {
        }

        public override bool ConvertFromSerial(string value)
        {
            switch (value)
            {
                case "YES":
                case "yes":
                case "Yes":
                case "TRUE":
                case "True":
                case "true":
                    return true;
                case "NO":
                case "no":
                case "No":
                case "FALSE":
                case "False":
                case "false":
                    return false;
                default:
                    return DefaultValue;
            }
        }

        public override string ToString()
        {
            this.SerializedValue = this.Value ? "YES" : "NO";
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
