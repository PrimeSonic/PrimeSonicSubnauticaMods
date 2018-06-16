namespace CustomCraftSML.Serialization.EasyMarkup
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;

    public class StringBuffer : LinkedList<char>,
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
                    AddLast(c);
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

            LinkedListNode<char> nodeA = First;
            LinkedListNode<char> nodeB = other.First;

            while (nodeA.Next != null && nodeB.Next != null)
            {
                if (!nodeA.Value.Equals(nodeB.Value))
                    return false;

                nodeA = nodeA.Next;
                nodeB = nodeB.Next;
            }

            return true;
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
            if (b is null)
                return false;

            if (ReferenceEquals(a, b))
                return true;

            if (a.Count != b.Count)
                return false;

            LinkedListNode<char> nodeA = a.First;
            LinkedListNode<char> nodeB = b.First;

            while (nodeA.Next != null && nodeB.Next != null)
            {
                if (!nodeA.Value.Equals(nodeB.Value))
                    return false;

                nodeA = nodeA.Next;
                nodeB = nodeB.Next;
            }

            return true;
        }

        private static bool BufferEqualToString(StringBuffer a, string b)
        {
            if (a.Count != b?.Length)
                return false;

            LinkedListNode<char> nodeA = a.First;
            int index = 0;

            while (nodeA.Next != null && index < b.Length)
            {
                if (!nodeA.Value.Equals(b[index]))
                    return false;

                nodeA = nodeA.Next;
                index++;
            }

            return true;
        }

        // Peeking Methods

        public char PeekStart() => First.Value;

        public char PeekEnd() => Last.Value;

        // Pop From Start Methods

        public char PopFromStart()
        {
            char value = First.Value;

            RemoveFirst();

            return value;
        }

        public bool PopFromStartIfEquals(char value)
        {
            if (!IsEmpty && First.Value.Equals(value))
            {
                RemoveFirst();
                return true;
            }

            return false;
        }

        public bool PopFromStartIfEquals(params char[] values)
        {
            if (!IsEmpty && values.Contains(First.Value))
            {
                RemoveFirst();
                return true;
            }

            return false;
        }

        // Pop From End Methods

        public char PopFromEnd()
        {
            char value = Last.Value;

            RemoveLast();

            return value;
        }

        public bool PopFromEndIfEquals(char value)
        {
            if (!IsEmpty && Last.Value.Equals(value))
            {
                RemoveLast();
                return true;
            }

            return false;
        }

        public bool PopFromEndIfEquals(params char[] values)
        {
            if (!IsEmpty && values.Contains(Last.Value))
            {
                RemoveLast();
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

        public bool StartsWith(char value) => !IsEmpty && First.Value.Equals(value);

        public bool StartsWith(params char[] values)
        {
            if (IsEmpty || Count < values.Length)
                return false;

            LinkedListNode<char> node = First;
            int index = 0;

            do
            {
                if (node.Value != values[index])
                    return false;

                node = node.Next;
                index++;

            } while (node != null && index < values.Length);

            return true;
        }

        public bool StartsWith(string value) => StartsWith(value.ToCharArray());

        public bool StartsWithAny(params char[] values) => !IsEmpty && values.Contains(First.Value);

        // Ends With Methods

        public bool EndsWith(char value) => !IsEmpty && Last.Value.Equals(value);

        public bool EndsWith(params char[] values)
        {
            if (IsEmpty || Count < values.Length)
                return false;

            LinkedListNode<char> node = Last;
            int index = values.Length - 1;

            do
            {
                if (node.Value != values[index])
                    return false;

                node = node.Previous;
                index--;

            } while (node != null && index > -1);

            return true;
        }

        public bool EndsWith(string value) => EndsWith(value.ToCharArray());

        public bool EndsWithAny(params char[] values) => !IsEmpty && values.Contains(Last.Value);

        // Push to Start Methods

        public void PushToStart(char value) => AddFirst(value);

        public void PushToStart(params char[] values)
        {
            var stack = new Stack<char>(values);

            while (stack.Count > 0)
                AddFirst(stack.Pop());
        }

        public void PushToStart(char value, int count = 1)
        {
            switch (count)
            {
                case 0:
                    return;
                case 1:
                    PushToStart(value);
                    break;
                default:
                    while (count-- > 0)
                        PushToStart(value);
                    break;
            }
        }

        public void PushToStart(string value) => PushToStart(value.ToCharArray());

        // Push to End Methods

        public void PushToEnd(char value) => AddLast(value);

        public void PushToEnd(params char[] values)
        {
            foreach (char value in values)
                AddLast(value);
        }

        public void PushToEnd(char value, int count = 1)
        {
            switch (count)
            {
                case 0:
                    return;
                case 1:
                    PushToEnd(value);
                    break;
                default:
                    while (count-- > 0)
                        PushToEnd(value);
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

            LinkedListNode<char> node = First;

            do
            {
                if (node.Value == original)
                    node.Value = replacement;

                node = node.Next;
            } while (node != null);
        }

        public void Replace(char[] original, char[] replacement)
        {
            if (IsEmpty)
                return;

            LinkedListNode<char> node = First;

            int index = 0;
            int count = 0;
            int matchingCount = original.Length;
            int replacementCount = replacement.Length;

            LinkedListNode<char> replacementNode = node;

            do
            {
                if (node.Value == original[index])
                {
                    index++;
                    count++;

                    if (count == 1)
                        replacementNode = node;

                    if (count == matchingCount)
                    {
                        // replace
                        replacementNode.Value = replacement[0];

                        for (int i = 1; i < matchingCount; i++)
                            if (replacementNode.Next != null)
                                Remove(replacementNode.Next);

                        for (int i = 1; i < replacementCount; i++)
                        {
                            AddAfter(replacementNode, replacement[i]);
                            replacementNode = replacementNode.Next;
                        }

                        count = 0;
                        index = 0;

                        node = replacementNode.Next;
                        continue;
                    }
                }
                else // !=
                {
                    count = 0;
                    index = 0;
                }

                node = node.Next;

            } while (node != null);
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