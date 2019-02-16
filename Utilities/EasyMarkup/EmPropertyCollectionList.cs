namespace Common.EasyMarkup
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using Common;

    public class EmPropertyCollectionList<T> : EmProperty, IEnumerable<T>, IValueConfirmation where T : EmPropertyCollection
    {
        public bool Optional { get; set; } = false;

        public bool HasValue => Count > 0;

        public Type ItemType => typeof(T);

        protected EmPropertyCollection Template;

        public T this[int index] => this.Values[index];

        public int Count => this.Values.Count;

        public void Add(T item)
        {
            this.Values.Add(item);
        }

        public IList<T> Values { get; } = new List<T>();

        public IEnumerator<T> GetEnumerator() => this.Values.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => this.Values.GetEnumerator();

        public EmPropertyCollectionList(string key, T template)
        {
            this.Key = key;
            Template = template;
        }

        public override string ToString()
        {
            if (!this.HasValue && this.Optional)
                return string.Empty;

            string val = $"{this.Key}{SpChar_KeyDelimiter}";
            foreach (EmPropertyCollection collection in this.Values)
            {
                val += SpChar_BeginComplexValue;

                foreach (string key in collection.Keys)
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
                        collection.FromString($"{this.Key}{SpChar_KeyDelimiter}{buffer.ToString()}{SpChar_ValueDelimiter}");
                        this.Values.Add(collection);
                        buffer.Clear();
                        serialValues += $"{collection.SerializedValue}{SpChar_ListItemSplitter}";
                        break;
                    case SpChar_BeginComplexValue:
                        openParens++;
                        goto default;
                    case SpChar_FinishComplexValue:
                        openParens--;
                        if (openParens < 0)
                            throw new EmException(UnbalancedContainersError, buffer);
                        goto default;
                    default:
                        buffer.PushToEnd(fullString.PopFromStart());
                        break;
                }
            } while (fullString.Count > 0);

            if (openParens != 0)
                throw new EmException(UnbalancedContainersError, buffer);

            return serialValues.TrimEnd(SpChar_ListItemSplitter) + SpChar_FinishComplexValue;
        }

        internal override EmProperty Copy() => new EmPropertyCollectionList<T>(this.Key, (T)Template.Copy());

        internal override bool ValueEquals(EmProperty other)
        {
            if (other is EmPropertyCollectionList<T> otherTyped)
            {
                if (this.Count != otherTyped.Count)
                    return false;

                for (int i = 0; i < this.Count; i++)
                {
                    if (!this[i].Equals(otherTyped[i]))
                        return false;
                }

                return true;
            }

            return false;
        }
    }

}