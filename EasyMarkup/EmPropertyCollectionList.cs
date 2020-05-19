namespace EasyMarkup
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    internal class EmPropertyCollectionList<ListedType> : EmProperty, IEnumerable<ListedType>, IValueConfirmation
        where ListedType : EmPropertyCollection, new()
    {
        public override bool HasValue => this.Count > 0;

        public Type ItemType => typeof(ListedType);

        protected EmPropertyCollection Template;

        public ListedType this[int index] => this.Values[index];

        public int Count => this.Values.Count;

        public void Add(ListedType item)
        {
            this.Values.Add(item);
        }

        public IList<ListedType> Values { get; } = new List<ListedType>();

        public IEnumerator<ListedType> GetEnumerator()
        {
            return this.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.Values.GetEnumerator();
        }

        public EmPropertyCollectionList(string key)
        {
            this.Key = key;
            Template = new ListedType();
        }

        public override string ToString()
        {
            if (!this.HasValue && this.Optional)
                return string.Empty;

            string val = $"{this.Key}{SpChar_KeyDelimiter}";
            foreach (EmPropertyCollection collection in this.Values)
            {
                val += SpChar_BeginComplexValue;

                foreach (EmProperty property in collection.Values)
                {
                    if (!property.HasValue && property.Optional)
                        continue;

                    val += $"{property}";
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

                        var collection = (ListedType)Template.Copy();
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

        internal override EmProperty Copy()
        {
            return new EmPropertyCollectionList<ListedType>(this.Key) { Optional = this.Optional };
        }

        internal override bool ValueEquals(EmProperty other)
        {
            if (other is EmPropertyCollectionList<ListedType> otherTyped)
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