namespace Common.EasyMarkup
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using Common;

    public class EmPropertyCollectionList<T> : EmProperty, IEnumerable<T>, IValueConfirmation where T : EmPropertyCollection
    {
        public bool HasValue { get; private set; } = false;

        public Type ItemType => typeof(T);

        protected IList<T> InternalValues { get; } = new List<T>();

        protected EmPropertyCollection Template;

        public T this[int index] => InternalValues[index];

        public int Count => InternalValues.Count;

        public void Add(T item)
        {
            HasValue = true;
            InternalValues.Add(item);
        }

        public IEnumerable<T> Values => InternalValues;

        public IEnumerator<T> GetEnumerator() => InternalValues.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => InternalValues.GetEnumerator();

        public EmPropertyCollectionList(string key, T template)
        {
            Key = key;
            Template = template;
        }

        public override string ToString()
        {
            var val = $"{Key}{SpChar_KeyDelimiter}";
            foreach (EmPropertyCollection collection in InternalValues)
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

                        var collection = (T)Template.Copy();
                        collection.FromString($"{Key}{SpChar_KeyDelimiter}{buffer.ToString()}{SpChar_ValueDelimiter}");
                        InternalValues.Add(collection);
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

            HasValue = true;

            return serialValues.TrimEnd(SpChar_ListItemSplitter) + SpChar_FinishComplexValue;
        }

        internal override EmProperty Copy() => new EmPropertyCollectionList<T>(Key, (T)Template.Copy());
    }

}