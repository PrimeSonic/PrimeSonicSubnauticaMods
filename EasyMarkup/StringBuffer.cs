namespace EasyMarkup
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// A double-ended-queue style data structure that represents mutable string data.
    /// </summary>
    /// <seealso cref="char" />
    /// <seealso cref="IComparable" />
    /// <seealso cref="IConvertible" />
    /// <seealso cref="System.IEquatable{StringBuffer}" />
    /// <seealso cref="string" />
    /// <seealso cref="ICloneable" />
    internal class StringBuffer : Deque<char>,
        IComparable, IConvertible, IEquatable<StringBuffer>, IEquatable<string>, ICloneable
    {
        // Constructors

        public StringBuffer()
        {

        }

        public StringBuffer(IEnumerable<char> collection) : base(collection)
        {
        }

        public StringBuffer(string value) : base(value.ToCharArray())
        {
        }

        public StringBuffer(StringBuffer original) : base(original)
        {
        }

        public StringBuffer(params StringBuffer[] originals)
        {
            foreach (StringBuffer original in originals)
                foreach (char c in original)
                    PushTail(c);
        }

        public StringBuffer(int capacity) : base(capacity)
        {
        }

        // Status Methods

        public bool IsEmpty => Count == 0;

        public int Length => Count;

        // Clone/Copy Methods

        public object Clone()
        {
            return new StringBuffer(this);
        }

        public StringBuffer Copy()
        {
            return new StringBuffer(this);
        }

        public int CompareTo(object obj) => string.Compare(ToString(), obj.ToString(), StringComparison.Ordinal);

        // Conversion Methods

        public TypeCode GetTypeCode()
        {
            return TypeCode.Object;
        }

        public bool ToBoolean(IFormatProvider provider)
        {
            return Convert.ToBoolean(ToString(), provider);
        }

        public char ToChar(IFormatProvider provider)
        {
            return Convert.ToChar(ToString(), provider);
        }

        public sbyte ToSByte(IFormatProvider provider)
        {
            return Convert.ToSByte(ToString(), provider);
        }

        public byte ToByte(IFormatProvider provider)
        {
            return Convert.ToByte(ToString(), provider);
        }

        public short ToInt16(IFormatProvider provider)
        {
            return Convert.ToInt16(ToString(), provider);
        }

        public ushort ToUInt16(IFormatProvider provider)
        {
            return Convert.ToUInt16(ToString(), provider);
        }

        public int ToInt32(IFormatProvider provider)
        {
            return Convert.ToInt32(ToString(), provider);
        }

        public uint ToUInt32(IFormatProvider provider)
        {
            return Convert.ToUInt32(ToString(), provider);
        }

        public long ToInt64(IFormatProvider provider)
        {
            return Convert.ToInt64(ToString(), provider);
        }

        public ulong ToUInt64(IFormatProvider provider)
        {
            return Convert.ToUInt64(ToString(), provider);
        }

        public float ToSingle(IFormatProvider provider)
        {
            return Convert.ToSingle(ToString(), provider);
        }

        public double ToDouble(IFormatProvider provider)
        {
            return Convert.ToDouble(ToString(), provider);
        }

        public decimal ToDecimal(IFormatProvider provider)
        {
            return Convert.ToDecimal(ToString(), provider);
        }

        public DateTime ToDateTime(IFormatProvider provider)
        {
            return Convert.ToDateTime(ToString(), provider);
        }

        public string ToString(IFormatProvider provider)
        {
            return ToString();
        }

        public object ToType(Type conversionType, IFormatProvider provider)
        {
            return Convert.ChangeType(ToString(), conversionType, provider);
        }

        public override string ToString()
        {
            if (IsEmpty)
                return string.Empty;

            char[] array = new char[Count];

            int i = 0;
            foreach (char c in this)
            {
                array[i] = c;
                i++;
            }

            return new string(array);
        }

        public override int GetHashCode() => RuntimeHelpers.GetHashCode(this);

        // Equality Checks

        public bool Equals(string other) => ToString() == other;

        public bool Equals(StringBuffer other)
        {
            if (other is null)
                return false;

            if (ReferenceEquals(this, other))
                return true;

            if (Count != other.Count)
                return false;

            var t = this.GetEnumerator();
            var o = other.GetEnumerator();

            bool result = true;
            while (o.MoveNext() && t.MoveNext())
            {
                if (t.Current != o.Current)
                {
                    result = false;
                    break;
                }
            }

            t.Dispose();
            o.Dispose();

            return result;
        }

        public override bool Equals(object obj)
        {
            if (obj is null)
                return false;

            if (ReferenceEquals(this, obj))
                return true;

            if (obj.GetType() != GetType())
                return false;

            return Equals((StringBuffer)obj);
        }

        public static bool operator ==(StringBuffer a, StringBuffer b)
        {
            if (a is null)
                return b is null;

            return a.Equals(b);
        }

        public static bool operator !=(StringBuffer a, StringBuffer b) => !BuffersAreEqual(a, b);

        public static bool operator ==(StringBuffer a, string b)
        {
            if (a is null)
                return b is null;

            return a.Equals(b);
        }

        public static bool operator !=(StringBuffer a, string b) => !BufferEqualToString(a, b);

        public static bool operator ==(string a, StringBuffer b)
        {
            if (b is null)
                return a is null;

            return b.Equals(a);
        }

        public static bool operator !=(string a, StringBuffer b) => !BufferEqualToString(b, a);

        private static bool BuffersAreEqual(StringBuffer a, StringBuffer b)
        {
            if (a is null)
                return b is null;

            return a.Equals(b);
        }

        private static bool BufferEqualToString(StringBuffer a, string b)
        {
            if (a is null)
                return b is null;

            if (a.Count != b?.Length)
                return false;

            var t = a.GetEnumerator();
            var o = b.GetEnumerator();

            bool result = true;
            while (o.MoveNext() && t.MoveNext())
            {
                if (t.Current != o.Current)
                {
                    result = false;
                    break;
                }
            }

            t.Dispose();
            //o.Dispose();

            return result;
        }

        // Peeking Methods

        public char PeekStart() => PeekHead();

        public char PeekEnd() => PeekTail();

        // Pop From Start Methods

        public char PopFromStart() => PopHead();

        public bool PopFromStartIfEquals(char value)
        {
            if (!IsEmpty && PeekHead() == value)
            {
                PopHead();
                return true;
            }

            return false;
        }

        public bool PopFromStartIfEquals(params char[] values)
        {
            if (!IsEmpty && values.Contains(PeekHead()))
            {
                PopHead();
                return true;
            }

            return false;
        }

        // Pop From End Methods

        public char PopFromEnd() => PopTail();

        public bool PopFromEndIfEquals(char value)
        {
            if (!IsEmpty && PeekTail() == value)
            {
                PopTail();
                return true;
            }

            return false;
        }

        public bool PopFromEndIfEquals(params char[] values)
        {
            if (!IsEmpty && values.Contains(this.PeekTail()))
            {
                this.PopTail();
                return true;
            }

            return false;
        }

        // Pop All From Start Methods

        public void PopAllFromStartIfEquals(char value)
        {
            while (PopFromStartIfEquals(value))
            {
            }
        }

        public void PopAllFromStartIfEquals(params char[] values)
        {
            while (PopFromStartIfEquals(values))
            {
            }
        }

        // Pop All From End Methods

        public void PopAllFromEndIfEquals(char value)
        {
            while (PopFromEndIfEquals(value))
            {
            }
        }

        public void PopAllFromEndIfEquals(params char[] values)
        {
            while (PopFromEndIfEquals(values))
            {
            }
        }

        // Starts With Methods

        public bool StartsWith(char value) => !IsEmpty && PeekHead() == value;

        public bool StartsWith(params char[] values)
        {
            if (IsEmpty || Count < values.Length)
                return false;

            int i = 0;
            foreach (char c in this)
            {
                if (c != values[i])
                    return false;

                i++;
                if (i == values.Length)
                    break;
            }

            return true;
        }

        public bool StartsWith(string value) => StartsWith(value.ToCharArray());

        public bool StartsWithAny(params char[] values) => !IsEmpty && values.Contains(PeekStart());

        // Ends With Methods

        public bool EndsWith(char value) => !IsEmpty && PeekTail() == value;

        public bool EndsWith(params char[] values)
        {
            if (IsEmpty || Count < values.Length)
                return false;

            int t = this.Count - 1;
            int v = values.Length - 1;

            for (int i = t; i > t - values.Length; i--)
            {
                if (this[i] != values[v])
                    return false;

                v--;
            }

            return true;
        }

        public bool EndsWith(string value) => EndsWith(value.ToCharArray());

        public bool EndsWithAny(params char[] values) => !IsEmpty && values.Contains(PeekTail());

        // Push to Start Methods

        public void PushToStart(params char[] values)
        {
            var stack = new Stack<char>(values);

            while (stack.Count > 0)
                PushHead(stack.Pop());
        }

        public void PushToStart(char value, int count = 1)
        {
            switch (count)
            {
                case 0:
                    return;
                case 1:
                    PushHead(value);
                    break;
                default:
                    while (count-- > 0)
                        PushHead(value);
                    break;
            }
        }

        public void PushToStart(string value) => PushToStart(value.ToCharArray());

        // Push to End Methods

        public void PushToEnd(params char[] values)
        {
            foreach (char value in values)
                PushTail(value);
        }

        public void PushToEnd(char value, int count = 1)
        {
            switch (count)
            {
                case 0:
                    return;
                case 1:
                    PushTail(value);
                    break;
                default:
                    while (count-- > 0)
                        PushTail(value);
                    break;
            }
        }

        public void PushToEnd(string value) => PushToEnd(value.ToCharArray());

        // Trim Methods

        public StringBuffer TrimStart(char value)
        {
            PopAllFromStartIfEquals(value);
            return this;
        }

        public StringBuffer TrimStart(params char[] values)
        {
            PopAllFromStartIfEquals(values);
            return this;
        }

        public StringBuffer TrimEnd(char value)
        {
            PopAllFromEndIfEquals(value);
            return this;
        }

        public StringBuffer TrimEnd(params char[] values)
        {
            PopAllFromEndIfEquals(values);
            return this;
        }

        public StringBuffer Trim(char value)
        {
            PopAllFromStartIfEquals(value);
            PopAllFromEndIfEquals(value);
            return this;
        }

        public StringBuffer Trim(params char[] values)
        {
            PopAllFromStartIfEquals(values);
            PopAllFromEndIfEquals(values);
            return this;
        }

        // Replace Methods

        public void Replace(char original, char replacement)
        {
            if (IsEmpty)
                return;

            for (int i = 0; i < this.Count; i++)
            {
                if (this[i] == original)
                    this[i] = replacement;
            }
        }

        public void Replace(char[] original, char[] replacement)
        {
            if (IsEmpty)
                return;


            var finalValue = new StringBuffer(this.Count);
            var limbo = new StringBuffer(this.Count);

            int cIndex = 0;

            int oCount = original.Length;

            do
            {
                if (this.PeekStart() == original[cIndex])
                {
                    limbo.PushToEnd(this.PopFromStart());
                    cIndex++;

                    if (cIndex == oCount) // Full match
                    {
                        foreach (char c in replacement)
                        {
                            finalValue.PushToEnd(c);
                        }

                        limbo.Clear();
                        cIndex = 0;
                    }
                }
                else
                {
                    cIndex = 0;

                    while (!limbo.IsEmpty)
                    {
                        finalValue.PushToEnd(limbo.PopFromStart());
                    }

                    finalValue.PushToEnd(this.PopFromStart());
                }

            } while (!this.IsEmpty);

            do
            {
                this.PushToEnd(finalValue.PopFromStart());
            } while (!finalValue.IsEmpty);
        }

        public void Replace(string original, string replacement) =>
            Replace(original.ToCharArray(), replacement.ToCharArray());

        // Transfer Methods

        public void TransferToStart(StringBuffer valuesToTransfer)
        {
            while (!valuesToTransfer.IsEmpty)
                PushToStart(valuesToTransfer.PopFromEnd());
        }

        public void TransferToEnd(StringBuffer valuesToTransfer)
        {
            while (!valuesToTransfer.IsEmpty)
                PushToEnd(valuesToTransfer.PopFromStart());
        }


    }

}