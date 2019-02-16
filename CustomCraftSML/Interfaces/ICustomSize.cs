namespace CustomCraft2SML.Interfaces
{
    public interface ICustomSize : ITechTyped, ICustomCraft
    {
        short Height { get; }
        short Width { get; }
    }
}