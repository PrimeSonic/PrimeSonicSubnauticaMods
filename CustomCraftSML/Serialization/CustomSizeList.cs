namespace CustomCraft2SML.Serialization
{
    using System.Collections;
    using System.Collections.Generic;
    using Common.EasyMarkup;

    public class CustomSizeList : EmPropertyCollectionList<CustomSize>, IEnumerable<CustomSize>
    {
        private const string KeyName = "CustomSizes";

        public new CustomSize this[int index] => base[index];

        public CustomSizeList() : base(KeyName, new CustomSize(KeyName))
        {
        }

        public new IEnumerator<CustomSize> GetEnumerator()
        {
            foreach (EmPropertyCollection item in InternalValues)
            {
                yield return (CustomSize)item;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
