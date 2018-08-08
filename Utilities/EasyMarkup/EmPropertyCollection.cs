namespace Common.EasyMarkup
{
    using System;
    using System.Collections.Generic;
    using Common;

    public abstract class EmPropertyCollection : EmProperty
    {
        public EmProperty this[string key]
        {
            get => Properties[key];
            internal set => Properties[key] = value;
        }

        protected readonly Dictionary<string, EmProperty> Properties;

        public Dictionary<string, EmProperty>.KeyCollection Keys => Properties.Keys;
        public Dictionary<string, EmProperty>.ValueCollection Values => Properties.Values;

        public readonly ICollection<EmProperty> Definitions;

        public EmPropertyCollection(string key, ICollection<EmProperty> definitions)
        {
            Key = key;
            Definitions = definitions;
            Properties = new Dictionary<string, EmProperty>(definitions.Count);

            foreach (EmProperty property in definitions)
                Properties[property.Key] = property;
        }

        public override string ToString()
        {
            var val = $"{Key}{SpChar_KeyDelimiter}{SpChar_BeginComplexValue}";
            foreach (string key in Properties.Keys)
            {
                val += $"{Properties[key]}";
            }

            return val + $"{SpChar_FinishComplexValue}{SpChar_ValueDelimiter}";
        }

        protected override string ExtractValue(StringBuffer fullString)
        {
            string serialValues = $"{SpChar_BeginComplexValue}";

            int openParens = 0;

            string subKey = null;

            var buffer = new StringBuffer();

            bool exit = false;

            do
            {
                switch (fullString.PeekStart())
                {
                    case SpChar_ValueDelimiter when openParens == 0 && buffer.PeekEnd() == SpChar_FinishComplexValue && fullString.Count == 1: // End of ComplexList
                        exit = true;
                        goto default;
                    case SpChar_ValueDelimiter when openParens == 1 && subKey != null: // End of a nested property belonging to this collection
                        buffer.PushToEnd(fullString.PopFromStart());
                        if (!Properties.ContainsKey(subKey))
                            Console.WriteLine($"Key Not Found: {subKey} - Current Buffer:{buffer}");

                        Properties[subKey].FromString(buffer.ToString());
                        buffer.Clear();
                        serialValues += Properties[subKey].ToString();
                        subKey = null;
                        goto default;
                    case SpChar_KeyDelimiter when openParens == 1: // Key to a nested property belonging to this collection
                        buffer.PopFromStartIfEquals(SpChar_BeginComplexValue);
                        buffer.PopFromStartIfEquals(SpChar_ListItemSplitter);
                        subKey = buffer.ToString();
                        goto default;
                    case SpChar_BeginComplexValue:
                        openParens++;
                        goto default;
                    case SpChar_FinishComplexValue:
                        openParens--;
                        goto default;
                    default:
                        buffer.PushToEnd(fullString.PopFromStart());
                        break;
                }

            } while (fullString.Count > 0 && !exit);

            return serialValues + SpChar_FinishComplexValue;
        }

        protected ICollection<EmProperty> CopyDefinitions
        {
            get
            {
                var definitions = new List<EmProperty>(Properties.Count);
                foreach (EmProperty item in Properties.Values)
                    definitions.Add(item.Copy());

                return definitions;
            }
        }
    }

}