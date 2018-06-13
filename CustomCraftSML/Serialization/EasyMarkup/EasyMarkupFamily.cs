namespace CustomCraftSML.Serialization.EasyMarkup
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Text.RegularExpressions;
    using UnityEngine.Assertions;

    public abstract class EmProperty
    {
        protected static readonly Regex WhiteSpace = new Regex(@"[\s\r\n\t]+", RegexOptions.Compiled | RegexOptions.Multiline);

        public string Key;
        internal string SerializedValue;

        public override string ToString()
        {
            return $"{Key}:{SerializedValue};";
        }

        public void FromString(string rawValue)
        {
            string flatValue = WhiteSpace.Replace(rawValue, string.Empty);

            char[] cleanValue = flatValue.ToCharArray();

            var fullString = new StringBuffer(cleanValue);

            FromString(fullString);
        }

        internal virtual void FromString(StringBuffer fullString)
        {
            var key = ExtractKey(fullString);
            if (string.IsNullOrEmpty(Key))
                Key = key;
            else
                Assert.AreEqual(Key, key);

            SerializedValue = ExtractValue(fullString);
        }

        protected virtual string ExtractKey(StringBuffer fullString)
        {
            var key = new StringBuffer();
            while (fullString.Count > 0 && fullString.PeekStart() != ':')
                key.AddFromEnd(fullString.RemoveFromStart());

            fullString.RemoveFromStart(); // Skip : separator

            return key.ToString();
        }

        protected virtual string ExtractValue(StringBuffer fullString)
        {
            var value = new StringBuffer();
            while (fullString.Count > 0 && fullString.PeekStart() != ';')
                value.AddFromEnd(fullString.RemoveFromStart());

            fullString.RemoveFromStart(); // Skip ; separator

            return value.ToString();
        }

        internal abstract EmProperty Copy();

        public string PrintyPrint()
        {
            //string value = ToString();

            throw new NotImplementedException();
        }
    }

    public class EmProperty<T> : EmProperty where T : IConvertible
    {
        protected T ObjValue;

        public T Value
        {
            get => ObjValue;
            set
            {
                ObjValue = value;
                SerializedValue = ObjValue.ToString(CultureInfo.InvariantCulture);
            }
        }

        public EmProperty(string key)
        {
            Key = key;
        }

        public EmProperty(string key, T value) : this(key)
        {
            Value = value;
        }

        protected override string ExtractValue(StringBuffer fullString)
        {
            string serialValue = base.ExtractValue(fullString);

            Value = ConvertFromSerial(serialValue);

            return serialValue;
        }

        internal static T ConvertFromSerial(string value)
        {
            return (T)Convert.ChangeType(value, typeof(T));
        }

        internal override EmProperty Copy() => new EmProperty<T>(Key);
    }

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

    public class EmPropertyCollectionList : EmProperty
    {
        protected EmPropertyCollection Definitions;

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
            var val = $"{Key}:";
            foreach (EmPropertyCollection collection in Collections)
            {
                val += "(";

                foreach (var key in collection.Keys)
                {
                    val += $"{collection[key]}";
                }

                val += "),";
            }

            return val.TrimEnd(',') + ";";
        }

        protected override string ExtractValue(StringBuffer fullString)
        {
            if (Collections == null)
                Collections = new List<EmPropertyCollection>();

            string serialValues = "(";

            int openParens = 0;

            var buffer = new StringBuffer();

            do
            {
                switch (fullString.PeekStart())
                {
                    case ';' when openParens == 0: // End of ComplexList                        
                    case ',' when openParens == 0 && fullString.Count > 0: // End of a nested property belonging to this collection
                        fullString.RemoveFromStart();

                        var collection = Definitions.Copy() as EmPropertyCollection;
                        collection.FromString($"{Key}:{buffer.ToString()};");
                        Collections.Add(collection);
                        buffer.Clear();
                        serialValues += $"{collection.SerializedValue},";
                        break;
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
            } while (fullString.Count > 0);

            return serialValues.TrimEnd(',') + ")";
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


