namespace CustomCraftSML.Serialization
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using Common;

    internal abstract class ValueType
    {
        internal abstract void ParseFromString(string line);
    }

    internal class SingleValue<T> : ValueType
    {
        //internal static Regex WhiteSpace = new Regex(@"[\s\t]+", RegexOptions.Compiled | RegexOptions.Multiline);

        internal string Value1;

        internal virtual T GetTypedValue1() => (T)Convert.ChangeType(Value1, typeof(T));

        internal override void ParseFromString(string line)
        {
            throw new NotImplementedException();
        }
    }

    internal class DoubleValue<T, K> : SingleValue<T>
    {
        internal string Value2;

        internal virtual K GetTypedValue2() => (K)Convert.ChangeType(Value2, typeof(K));

        internal override void ParseFromString(string line)
        {
            throw new NotImplementedException();
        }
    }

    internal class SingleList<T> : ValueType
    {
        internal List<SingleValue<T>> ValueList;

        internal virtual List<T> GetList()
        {
            var typedList = new List<T>(ValueList.Count);

            foreach (SingleValue<T> item in ValueList)
            {
                typedList.Add(item.GetTypedValue1());
            }

            return typedList;
        }

        internal override void ParseFromString(string line)
        {
            throw new NotImplementedException();
        }
    }

    internal class DoubleList<T, K> : ValueType
    {
        internal List<DoubleValue<T, K>> ValueList;

        internal virtual List<KeyValuePair<T, K>> GetList()
        {
            var typedList = new List<KeyValuePair<T, K>>(ValueList.Count);

            foreach (DoubleValue<T, K> item in ValueList)
            {
                typedList.Add(new KeyValuePair<T, K>(item.GetTypedValue1(), item.GetTypedValue2()));
            }

            return typedList;
        }

        internal override void ParseFromString(string line)
        {
            throw new NotImplementedException();
        }
    }

    internal class TechTypeValue : SingleValue<TechType>
    {
        internal override TechType GetTypedValue1()
        {
            try
            {
                return (TechType)Enum.Parse(typeof(TechType), Value1.WithFirstUpper());
            }
            catch (Exception ex)
            {
                Logger.Log($"Error parsing TechType string: {Value1}", ex.ToString());
                return TechType.None;
            }
        }
    }

    internal abstract class SerialDefinition
    {
        internal string MasterKey;

        internal readonly Dictionary<string, ValueType> Properties = new Dictionary<string, ValueType>();

        internal SerialDefinition(string masterKey, Dictionary<string, ValueType> keyValuesPairs)
        {
            MasterKey = masterKey;
            Properties = keyValuesPairs;
        }
    }

}
