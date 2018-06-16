namespace CustomCraftSML.Serialization.EasyMarkup
{
    interface IStandardTyped<T>
    {
        T ConvertFromSerial(string value);        
    }
}
