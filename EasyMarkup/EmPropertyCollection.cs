namespace EasyMarkup
{
    using System;
    using System.Collections.Generic;

    internal abstract class EmPropertyCollection : EmProperty
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

        protected EmPropertyCollection(string key, ICollection<EmProperty> definitions)
        {
            this.Key = key;
            Definitions = definitions;
            Properties = new Dictionary<string, EmProperty>(definitions.Count, StringComparer.InvariantCultureIgnoreCase);

            foreach (EmProperty property in definitions)
                Properties[property.Key] = property;
        }

        public override string ToString()
        {
            string val = $"{this.Key}{SpChar_KeyDelimiter}{SpChar_BeginComplexValue}";
            foreach (EmProperty property in Properties.Values)
            {
                if (!property.HasValue && property.Optional)
                    continue;

                val += $"{property}";
            }

            return val + $"{SpChar_FinishComplexValue}{SpChar_ValueDelimiter}";
        }

        public override bool HasValue
        {
            get
            {
                foreach (EmProperty property in Properties.Values)
                {
                    if (property.HasValue)
                        return true;
                }

                return false;
            }
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
                        if (!Properties.TryGetValue(subKey, out var property))
                            throw new EmException($"Unknown key value: '{subKey}'", buffer);

                        property.FromString(buffer.ToString());
                        buffer.Clear();
                        serialValues += property.ToString();
                        subKey = null;
                        goto default;
                    case SpChar_KeyDelimiter when openParens == 1: // Key to a nested property belonging to this collection
                        buffer.PopFromStartIfEquals(SpChar_BeginComplexValue);
                        buffer.PopFromStartIfEquals(SpChar_ListItemSplitter);
                        subKey = buffer.ToString();
                        goto default;
                    case SpChar_EscapeChar:
                        buffer.PushToEnd(fullString.PopFromStart()); // Include escape char to be handled by EmProperty
                        goto default;
                    case SpChar_BeginComplexValue:
                        openParens++;
                        goto default;
                    case SpChar_FinishComplexValue:
                        openParens--;
                        if (openParens < 0)
                            throw new EmException(UnbalancedContainersError, buffer);
                        goto default;
                    case SpChar_LiteralStringBlock:
                        buffer.PushToEnd(fullString.PopFromStart()); // add first "

                        char popped;
                        do
                        {
                            popped = fullString.PopFromStart();
                            buffer.PushToEnd(popped); // until the last "
                        } while (popped != SpChar_LiteralStringBlock && fullString.Count > 0);

                        break;
                    default:
                        buffer.PushToEnd(fullString.PopFromStart());
                        break;
                }

            } while (fullString.Count > 0 && !exit);

            if (openParens != 1)
                throw new EmException(UnbalancedContainersError, buffer);

            return serialValues + SpChar_FinishComplexValue;
        }

        internal override bool ValueEquals(EmProperty other)
        {
            if (other is EmPropertyCollection otherTyped)
            {
                if (Properties.Count != otherTyped.Properties.Count)
                    return false;

                foreach (KeyValuePair<string, EmProperty> property in Properties)
                {
                    if (!otherTyped.Properties.TryGetValue(property.Key, out var otherValue))
                        return false;

                    if (!property.Value.Equals(otherValue))
                        return false;
                }

                return true;
            }

            return false;
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