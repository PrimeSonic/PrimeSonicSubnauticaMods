namespace Common.EasyMarkup
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using Common;

    public class EmPropertyList<T> : EmProperty, IEnumerable<T> where T : IConvertible
    {
        private static HashSet<char> ListDelimeters { get; } = new HashSet<char> { SpChar_ListItemSplitter, SpChar_ValueDelimiter };

        public bool HasValue { get; private set; } = false;

        protected IList<T> InternalValues { get; } = new List<T>();

        public T this[int index] => this.InternalValues[index];

        public int Count => this.InternalValues.Count;

        public void Add(T item)
        {
            this.HasValue = true;
            this.InternalValues.Add(item);
        }

        public IEnumerable<T> Values => this.InternalValues;

        public IEnumerator<T> GetEnumerator() => this.InternalValues.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => this.InternalValues.GetEnumerator();

        public EmPropertyList(string key)
        {
            this.Key = key;
        }

        public EmPropertyList(string key, IEnumerable<T> values) : this(key)
        {
            this.InternalValues = new List<T>();

            foreach (T value in values)
            {
                this.InternalValues.Add(value);
            }
        }

        public override string ToString()
        {
            string val = $"{this.Key}{SpChar_KeyDelimiter}";
            foreach (T value in this.InternalValues)
            {
                val += $"{EscapeSpecialCharacters(value.ToString())}{SpChar_ListItemSplitter}";
            }

            val = val.TrimEnd(SpChar_ListItemSplitter);

            return val + SpChar_ValueDelimiter;
        }

        protected override string ExtractValue(StringBuffer fullString)
        {
            string serialValues = string.Empty;

            do
            {
                string serialValue = ReadUntilDelimiter(fullString, ListDelimeters);

                this.InternalValues.Add(ConvertFromSerial(serialValue));

                serialValues += serialValue + SpChar_ListItemSplitter;

            } while (fullString.Count > 0 && fullString.PeekStart() != SpChar_ValueDelimiter);

            this.HasValue = true;

            return serialValues.TrimEnd(SpChar_ListItemSplitter);
        }

        internal override EmProperty Copy() => new EmPropertyList<T>(this.Key, this.InternalValues);

        public virtual T ConvertFromSerial(string value)
        {
            Type type = typeof(T);

            if (type.IsEnum)
                return (T)Enum.Parse(type, value);
            else
                return (T)Convert.ChangeType(value, typeof(T));
        }

        internal override bool ValueEquals(EmProperty other)
        {
            if (other is EmPropertyList<T> otherTyped)
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