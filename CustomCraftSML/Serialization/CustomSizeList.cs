namespace CustomCraftSML.Serialization
{
    using System.Collections;
    using System.Collections.Generic;
    using EasyMarkup;

    public class CustomSizeList : EmPropertyCollectionList<CustomSize>, IEnumerable<CustomSize>
    {
        private const string KeyName = "CustomSizes";

        public new CustomSize this[int index] => (CustomSize)base[index];

        public CustomSizeList() : base(KeyName, new CustomSize(KeyName))
        {
        }

        public IEnumerator<CustomSize> GetEnumerator()
        {
            foreach (EmPropertyCollection item in Collections)
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
