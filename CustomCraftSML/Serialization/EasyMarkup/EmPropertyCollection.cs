namespace CustomCraftSML.Serialization.EasyMarkup
{

    using System.Collections.Generic;

    public class EmPropertyCollection : EmProperty
    {
        public EmProperty this[string key]
        {
            get => Properties[key];
            internal set => Properties[key] = value;
        }

        protected readonly Dictionary<string, EmProperty> Properties;

        public Dictionary<string, EmProperty>.KeyCollection Keys => Properties.Keys;
        public Dictionary<string, EmProperty>.ValueCollection Values => Properties.Values;

        public EmPropertyCollection(string key, ICollection<EmProperty> definitions)
        {
            Key = key;

            Properties = new Dictionary<string, EmProperty>(definitions.Count);

            foreach (EmProperty property in definitions)
                Properties[property.Key] = property;
        }

        public override string ToString()
        {
            var val = $"{Key}:(";
            foreach (string key in Properties.Keys)
            {
                val += $"{Properties[key]}";
            }

            return val + ");";
        }

        protected override string ExtractValue(StringBuffer fullString)
        {
            string serialValues = "(";

            int openParens = 0;

            string subKey = null;

            var buffer = new StringBuffer();

            bool exit = false;

            do
            {
                switch (fullString.PeekStart())
                {
                    case ';' when openParens == 0 && buffer.PeekEnd() == ')' && fullString.Count == 1: // End of ComplexList
                        exit = true;
                        goto default;
                    case ';' when openParens == 1 && subKey != null: // End of a nested property belonging to this collection
                        buffer.AddFromEnd(fullString.RemoveFromStart());                        
                        Properties[subKey].FromString(buffer.ToString());
                        buffer.Clear();
                        serialValues += Properties[subKey].ToString();
                        subKey = null;
                        goto default;
                    case ':' when openParens == 1: // Key to a nested property belonging to this collection
                        buffer.RemoveFromStart('(');
                        buffer.RemoveFromStart(',');
                        subKey = buffer.ToString();
                        goto default;
                    case '(':
                        openParens++;
                        goto default;
                    case ')':
                        openParens--;
                        goto default;
                    default:

                        buffer.AddFromEnd(fullString.RemoveFromStart());
                        break;
                }

            } while (fullString.Count > 0 && !exit);

            return serialValues + ")";
        }

        internal override EmProperty Copy()
        {
            var definitions = new List<EmProperty>(Properties.Count);
            foreach (EmProperty item in Properties.Values)
                definitions.Add(item.Copy());

            return new EmPropertyCollection(Key, definitions);
        }
    }

}