namespace CustomCraftSML.Serialization.EasyMarkup
{

    public class StringBuffer : DoubleQueue<char>
    {
        public StringBuffer()
        {

        }

        public StringBuffer(char[] original)
        {
            foreach (char c in original)
                base.AddLast(c);
        }

        public StringBuffer(string original) : this(original.ToCharArray())
        {
        }

        public StringBuffer(StringBuffer original)
        {
            foreach (char c in original)
                base.AddLast(c);
        }

        public bool IsEmpty => base.Count == 0;

        public override string ToString()
        {
            if (base.Count == 0)
                return string.Empty;

            char[] array = new char[base.Count];

            int i = 0;
            foreach (char c in this)
            {
                array[i] = c;
                i++;
            }

            return new string(array);
        }

        public void Append(char value, int count = 1)
        {
            switch (count)
            {
                case 0:
                    return;
                case 1:
                    base.AddFromEnd(value);
                    break;
                default:
                    while (count-- > 0)
                        base.AddFromEnd(value);
                    break;
            }
        }
    }

}