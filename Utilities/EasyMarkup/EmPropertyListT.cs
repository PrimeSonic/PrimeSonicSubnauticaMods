namespace Common.EasyMarkup
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using Common;

    public class EmPropertyList<T> : EmProperty, IEnumerable<T> where T : IConvertible
    {
        public bool HasValue { get; private set; } = false;

        protected IList<T> InternalValues { get; } = new List<T>();

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

        public EmPropertyList(string key)
        {
            Key = key;
        }

        public EmPropertyList(string key, IEnumerable<T> values) : this(key)
        {
            InternalValues = new List<T>();

            foreach (T value in values)
            {
                InternalValues.Add(value);
            }
        }

        public override string ToString()
        {
            var val = $"{Key}{SpChar_KeyDelimiter}";
            foreach (T value in InternalValues)
            {
                val += $"{value}{SpChar_ListItemSplitter}";
            }

            val = val.TrimEnd(SpChar_ListItemSplitter);

            return val + SpChar_ValueDelimiter;
        }

        protected override string ExtractValue(StringBuffer fullString)
        {
            var value = new StringBuffer();
            string serialValues = string.Empty;

            do
            {
                while (fullString.Count > 0 && fullString.PeekStart() != SpChar_ListItemSplitter && fullString.PeekStart() != SpChar_ValueDelimiter) // separator
                {
                    value.PushToEnd(fullString.PopFromStart());
                }

                var serialValue = value.ToString();

                InternalValues.Add(ConvertFromSerial(serialValue));

                serialValues += serialValue + SpChar_ListItemSplitter;

                fullString.PopFromStart(); // Skip , separator

                value.Clear();

            } while (fullString.Count > 0 && fullString.PeekStart() != SpChar_ValueDelimiter);

            HasValue = true;

            return serialValues.TrimEnd(SpChar_ListItemSplitter);
        }

        internal override EmProperty Copy() => new EmPropertyList<T>(Key, this.InternalValues);

        public virtual T ConvertFromSerial(string value)
        {
            var type = typeof(T);

            if (type.IsEnum)
                return (T)Enum.Parse(type, value);
            else
                return (T)Convert.ChangeType(value, typeof(T));
        }


    }

}