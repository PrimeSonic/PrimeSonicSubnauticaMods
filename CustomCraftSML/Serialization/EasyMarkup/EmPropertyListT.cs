namespace CustomCraftSML.Serialization.EasyMarkup
{

    using System;
    using System.Collections.Generic;

    public class EmPropertyList<T> : EmProperty where T : IConvertible
    {
        public List<T> Values;

        public EmPropertyList(string key)
        {
            Key = key;
        }

        public EmPropertyList(string key, ICollection<T> values) : this(key)
        {
            Values = new List<T>(values.Count);

            foreach (T value in values)
            {
                Values.Add(value);
            }
        }

        public override string ToString()
        {
            var val = $"{Key}{SpChar_KeyDelimiter}";
            foreach (T value in Values)
            {
                val += $"{value}{SpChar_ListItemSplitter}";
            }

            val = val.TrimEnd(SpChar_ListItemSplitter);

            return val + SpChar_ValueDelimiter;
        }

        protected override string ExtractValue(StringBuffer fullString)
        {
            if (Values == null)
                Values = new List<T>();

            var value = new StringBuffer();
            string serialValues = string.Empty;

            do
            {
                while (fullString.Count > 0 && fullString.PeekStart() != SpChar_ListItemSplitter && fullString.PeekStart() != SpChar_ValueDelimiter) // separator
                {
                    value.PushToEnd(fullString.PopFromStart());
                }

                var serialValue = value.ToString();

                Values.Add(EmProperty<T>.ConvertFromSerial(serialValue));

                serialValues += serialValue + SpChar_ListItemSplitter;

                fullString.PopFromStart(); // Skip , separator

                value.Clear();

            } while (fullString.Count > 0 && fullString.PeekStart() != SpChar_ValueDelimiter);

            return serialValues.TrimEnd(SpChar_ListItemSplitter);
        }

        internal override EmProperty Copy() => new EmPropertyList<T>(Key);
    }

}