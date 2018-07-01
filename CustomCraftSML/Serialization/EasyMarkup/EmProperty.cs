namespace CustomCraftSML.Serialization.EasyMarkup
{
    using NUnit.Framework;
    using Common;

    public abstract class EmProperty : IEmProperty
    {
        protected const char SpChar_KeyDelimiter = ':';
        protected const char SpChar_ValueDelimiter = ';';
        protected const char SpChar_BeginComplexValue = '(';
        protected const char SpChar_FinishComplexValue = ')';
        protected const char SpChar_ListItemSplitter = ',';        

        protected delegate void OnValueExtracted();
        protected OnValueExtracted OnValueExtractedEvent;

        public string Key { get; protected set; }

        internal string SerializedValue;

        public override string ToString()
        {
            return $"{Key}{SpChar_KeyDelimiter}{SerializedValue}{SpChar_ValueDelimiter}";
        }

        public void FromString(string rawValue)
        {            
            var cleanValue = CleanValue(new StringBuffer(rawValue));

            var key = ExtractKey(cleanValue);
            if (string.IsNullOrEmpty(Key))
                Key = key;
            else
                Assert.AreEqual(Key, key);

            SerializedValue = ExtractValue(cleanValue);
            OnValueExtractedEvent?.Invoke();
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


        private StringBuffer CleanValue(StringBuffer rawValue)
        {
            var cleanValue = new StringBuffer();

            while (!rawValue.IsEmpty)
            {
                switch (rawValue.PeekStart())
                {
                    case ' ':
                        rawValue.TrimStart(' ');                        
                        break;
                    case '\r':
                        rawValue.TrimStart('\r', '\n');
                        break;
                    default:
                        cleanValue.PushToEnd(rawValue.PopFromStart());
                        break;
                }
            }

            return cleanValue;
        }
    }

}