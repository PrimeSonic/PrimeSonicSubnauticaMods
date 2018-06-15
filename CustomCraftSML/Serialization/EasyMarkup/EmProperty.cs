namespace CustomCraftSML.Serialization.EasyMarkup
{
    using System.Text.RegularExpressions;
    using NUnit.Framework;

    public abstract class EmProperty
    {
        protected static readonly Regex WhiteSpace = new Regex(@"[\s\r\n\t]+", RegexOptions.Compiled | RegexOptions.Multiline);

        public string Key;
        internal string SerializedValue;

        public override string ToString()
        {
            return $"{Key}:{SerializedValue};";
        }

        public void FromString(string rawValue)
        {
            string flatValue = WhiteSpace.Replace(rawValue, string.Empty);

            char[] cleanValue = flatValue.ToCharArray();

            var fullString = new StringBuffer(cleanValue);

            var key = ExtractKey(fullString);
            if (string.IsNullOrEmpty(Key))
                Key = key;
            else
                Assert.AreEqual(Key, key);

            SerializedValue = ExtractValue(fullString);
        }

        protected virtual string ExtractKey(StringBuffer fullString)
        {
            var key = new StringBuffer();
            while (fullString.Count > 0 && fullString.PeekStart() != ':')
                key.PushToEnd(fullString.PopFromStart());

            fullString.PopFromStart(); // Skip : separator

            return key.ToString();
        }

        protected virtual string ExtractValue(StringBuffer fullString)
        {
            var value = new StringBuffer();
            while (fullString.Count > 0 && fullString.PeekStart() != ';')
                value.PushToEnd(fullString.PopFromStart());

            fullString.PopFromStart(); // Skip ; separator

            return value.ToString();
        }

        internal abstract EmProperty Copy();

        public string PrintyPrint()
        {
            var originalString = new StringBuffer(this.ToString());

            var prettyString = new StringBuffer();

            int indentLevel = 0;
            const int indentSize = 4;

            do
            {
                switch (originalString.PeekStart())
                {
                    case '(':
                        prettyString.PushToEnd('\r', '\n');
                        prettyString.PushToEnd(' ', indentLevel * indentSize);
                        indentLevel++;
                        prettyString.PushToEnd(originalString.PopFromStart());
                        prettyString.PushToEnd('\r', '\n');
                        prettyString.PushToEnd(' ', indentLevel * indentSize);
                        prettyString.PushToEnd(originalString.PopFromStart());
                        break;
                    case ';':
                        prettyString.PushToEnd(originalString.PopFromStart());

                        if (originalString.IsEmpty || originalString.PeekStart() == ')')
                            indentLevel--;

                        prettyString.PushToEnd('\r', '\n');
                        prettyString.PushToEnd(' ', indentLevel * indentSize);
                        
                        break;
                    default:
                        prettyString.PushToEnd(originalString.PopFromStart());
                        break;
                }

            } while (!originalString.IsEmpty);

            return prettyString.ToString();
        }
    }

}