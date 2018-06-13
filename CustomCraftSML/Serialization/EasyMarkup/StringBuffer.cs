namespace CustomCraftSML.Serialization.EasyMarkup
{
    public class StringBuffer : DoubleQueue<char>
    {
        public StringBuffer()
        {

        }

        public StringBuffer(char[] array)
        {
            foreach (char c in array)
                base.AddLast(c);
        }

        public override string ToString()
        {
            char[] array = new char[base.Count];

            int i = 0;
            foreach (char c in this)
            {
                array[i] = c;
                i++;
            }

            return new string(array);
        }
    }
}
