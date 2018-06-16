namespace CustomCraftSML.Serialization
{
    using EasyMarkup;

    public class CustomSizeList : EmPropertyCollectionList<CustomSize>
    {
        private const string KeyName = "CustomSizes";

        public new CustomSize this[int index] => (CustomSize)base[index];

        public CustomSizeList() : base(KeyName, new CustomSize(KeyName))
        {
        }
    }
}
