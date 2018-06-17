namespace CustomCraftSML.Serialization.EasyMarkup
{
    public interface IEmProperty
    {
        string Key { get; }
        void FromString(string rawValue);
        string PrintyPrint();
        string ToString();
    }
}