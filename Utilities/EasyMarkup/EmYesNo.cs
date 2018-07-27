namespace Common.EasyMarkup
{
    public class EmYesNo : EmProperty<bool>
    {
        internal bool ValidData = true;

        private readonly bool DefaultValue;

        public EmYesNo(string key, bool defaultValue = false) : base(key)
        {
            DefaultValue = defaultValue;
        }

        public EmYesNo(string key, bool value, bool defaultValue = false) : base(key, value)
        {
            DefaultValue = defaultValue;
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
                    ValidData = true;
                    return true;
                case "NO":
                case "no":
                case "No":
                case "FALSE":
                case "False":
                case "false":
                    ValidData = true;
                    return false;
                default: 
                    ValidData = false;
                    return DefaultValue;
            }
        }

        public override string ToString()
        {
            this.SerializedValue = this.Value ? "YES" : "NO";
            return base.ToString();
        }

        internal override EmProperty Copy() => new EmYesNo(this.Key);
    }
}
