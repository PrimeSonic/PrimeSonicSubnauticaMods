namespace Common.EasyMarkup
{
    using Common;
    using System;
    using UnityEngine.Assertions;

    public abstract class EmProperty
    {
        protected const char SpChar_KeyDelimiter = ':';
        protected const char SpChar_ValueDelimiter = ';';
        protected const char SpChar_BeginComplexValue = '(';
        protected const char SpChar_FinishComplexValue = ')';
        protected const char SpChar_ListItemSplitter = ',';
        protected const char SpChar_CommentBlock = '#';

        protected delegate void OnValueExtracted();
        protected OnValueExtracted OnValueExtractedEvent;

        public string Key { get; protected set; }

        internal string SerializedValue;

        public override string ToString()
        {
            return $"{Key}{SpChar_KeyDelimiter}{SerializedValue}{SpChar_ValueDelimiter}";
        }

        public bool FromString(string rawValue)
        {
            StringBuffer cleanValue = CleanValue(new StringBuffer(rawValue));

            if (cleanValue.IsEmpty)
                return false;

            var key = ExtractKey(cleanValue);
            if (string.IsNullOrEmpty(Key))
                Key = key;
            else
                Assert.AreEqual(Key, key);

            SerializedValue = ExtractValue(cleanValue);
            OnValueExtractedEvent?.Invoke();

            return true;
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
                    case SpChar_BeginComplexValue:
                        prettyString.PushToEnd('\r', '\n');
                        prettyString.PushToEnd(' ', indentLevel * indentSize);
                        indentLevel++;
                        prettyString.PushToEnd(originalString.PopFromStart());
                        prettyString.PushToEnd('\r', '\n');
                        prettyString.PushToEnd(' ', indentLevel * indentSize);
                        prettyString.PushToEnd(originalString.PopFromStart());
                        break;
                    case SpChar_ValueDelimiter:
                        prettyString.PushToEnd(originalString.PopFromStart());

                        if (originalString.IsEmpty || originalString.PeekStart() == SpChar_FinishComplexValue)
                            indentLevel--;

                        prettyString.PushToEnd('\r', '\n');
                        prettyString.PushToEnd(' ', indentLevel * indentSize);

                        break;
                    case SpChar_CommentBlock:
                        prettyString.PushToEnd(originalString.PopFromStart());

                        do
                        {
                            prettyString.PushToEnd(originalString.PopFromStart());

                        } while (!originalString.IsEmpty && prettyString.PeekEnd() != SpChar_CommentBlock);

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

        private static StringBuffer CleanValue(StringBuffer rawValue)
        {
            var cleanValue = new StringBuffer();

            while (!rawValue.IsEmpty)
            {
                switch (rawValue.PeekStart())
                {
                    case ' ': // spaces
                        rawValue.TrimStart(' ');
                        break;
                    case '\t': // tabs
                        rawValue.TrimStart('\t');
                        break;
                    case '\r': // line breaks
                    case '\n':
                        rawValue.TrimStart('\r', '\n');
                        break;
                    case SpChar_CommentBlock: // comments
                        rawValue.PopFromStart(); // Pop first #

                        char poppedChar;
                        do
                        {
                            poppedChar = rawValue.PopFromStart();

                        } while (!rawValue.IsEmpty && poppedChar != SpChar_CommentBlock);

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