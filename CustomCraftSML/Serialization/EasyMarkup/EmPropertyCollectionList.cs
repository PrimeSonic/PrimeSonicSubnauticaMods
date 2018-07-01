namespace CustomCraftSML.Serialization.EasyMarkup
{
    using System.Collections.Generic;
    using Common;

    public class EmPropertyCollectionList<T> : EmProperty where T : EmPropertyCollection
    {
        protected EmPropertyCollection Template;

        public EmPropertyCollection this[int index] => Collections[index];

        public int Count => Collections.Count;

        public readonly List<EmPropertyCollection> Collections = new List<EmPropertyCollection>();

        public EmPropertyCollectionList(string key, T template)
        {
            Key = key;
            Template = template;
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
            string serialValues = $"{SpChar_BeginComplexValue}";

            int openParens = 0;

            var buffer = new StringBuffer();

            do
            {
                switch (fullString.PeekStart())
                {
                    case SpChar_ValueDelimiter when openParens == 0: // End of ComplexList                        
                    case SpChar_ListItemSplitter when openParens == 0 && fullString.Count > 0: // End of a nested property belonging to this collection
                        fullString.PopFromStart(); // Skip delimiter

                        var collection = (EmPropertyCollection)Template.Copy();
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
            return new EmPropertyCollectionList<T>(Key, (T)Template.Copy());
        }
    }

}