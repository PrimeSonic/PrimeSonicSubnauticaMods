namespace Common.EasyMarkup
{
    internal class EmYesNo : EmProperty<bool>
    {
        internal bool ValidData = true;

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
