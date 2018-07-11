namespace Common.EasyMarkup
{
    internal class EmYesNo : EmProperty<bool>
    {
        public EmYesNo(string key) : base(key)
        {
        }

        public EmYesNo(string key, bool value) : base(key, value)
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
                default:
                    return false;
            }
        }

        public override string ToString()
        {
            this.SerializedValue = this.Value ? "YES" : "NO";
            return base.ToString();
        }
    }
}
