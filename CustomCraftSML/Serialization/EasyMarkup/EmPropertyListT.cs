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
            var val = $"{Key}:";
            foreach (T value in Values)
            {
                val += $"{value},";
            }

            val = val.TrimEnd(',');

            return val + ';';
        }

        protected override string ExtractValue(StringBuffer fullString)
        {
            if (Values == null)
                Values = new List<T>();

            var value = new StringBuffer();
            string serialValues = "";

            do
            {
                while (fullString.Count > 0 && fullString.PeekStart() != ',' && fullString.PeekStart() != ';') // separator
                {
                    value.AddFromEnd(fullString.RemoveFromStart());
                }

                var serialValue = value.ToString();

                Values.Add(EmProperty<T>.ConvertFromSerial(serialValue));

                serialValues += serialValue + ",";

                fullString.RemoveFromStart(); // Skip , separator

                value.Clear();

            } while (fullString.Count > 0 && fullString.PeekStart() != ';');

            return serialValues.TrimEnd(',');
        }

        internal override EmProperty Copy() => new EmPropertyList<T>(Key);
    }

}