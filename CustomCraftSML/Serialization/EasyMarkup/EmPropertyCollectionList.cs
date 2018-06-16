namespace CustomCraftSML.Serialization.EasyMarkup
{

    using System.Collections.Generic;

    public class EmPropertyCollectionList : EmProperty
    {
        protected EmPropertyCollection Definitions;

        public EmPropertyCollection this[int index] => Collections[index];

        public List<EmPropertyCollection> Collections;

        public EmPropertyCollectionList(string key, ICollection<EmProperty> definitions)
        {
            Key = key;

            Definitions = new EmPropertyCollection(key, definitions);

            foreach (EmProperty property in definitions)
                Definitions[property.Key] = property;
        }

        public override string ToString()
        {
            var val = $"{Key}{SpChar_KeyDelimiter}";
            foreach (EmPropertyCollection collection in Collections)
            {
                val += SpChar_BeginComplexValue;

                foreach (var key in collection.Keys)
                {
                    val += $"{collection[key]}";
                }

                val += $"{SpChar_FinishComplexValue}{SpChar_ListItemSplitter}";
            }

            return val.TrimEnd(SpChar_ListItemSplitter) + SpChar_ValueDelimiter;
        }

        protected override string ExtractValue(StringBuffer fullString)
        {
            if (Collections == null)
                Collections = new List<EmPropertyCollection>();

            string serialValues = $"{SpChar_BeginComplexValue}";

            int openParens = 0;

            var buffer = new StringBuffer();

            do
            {
                switch (fullString.PeekStart())
                {
                    case SpChar_ValueDelimiter when openParens == 0: // End of ComplexList                        
                    case SpChar_ListItemSplitter when openParens == 0 && fullString.Count > 0: // End of a nested property belonging to this collection
                        fullString.PopFromStart();

                        var collection = (EmPropertyCollection)Definitions.Copy();
                        collection.FromString($"{Key}{SpChar_KeyDelimiter}{buffer.ToString()}{SpChar_ValueDelimiter}");
                        Collections.Add(collection);
                        buffer.Clear();
                        serialValues += $"{collection.SerializedValue}{SpChar_ListItemSplitter}";
                        break;
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
            } while (fullString.Count > 0);

            return serialValues.TrimEnd(SpChar_ListItemSplitter) + SpChar_FinishComplexValue;
        }

        internal override EmProperty Copy()
        {
            var definitions = new List<EmProperty>(Definitions.Keys.Count);
            foreach (EmProperty item in Definitions.Values)
                definitions.Add(item.Copy());

            return new EmPropertyCollectionList(Key, definitions);
        }
    }

}