namespace CustomCraft2SML.Interfaces
{
    internal interface ICustomSize : ITechTyped, ICustomCraft
    {
        short Height { get; }
        short Width { get; }
    }
}