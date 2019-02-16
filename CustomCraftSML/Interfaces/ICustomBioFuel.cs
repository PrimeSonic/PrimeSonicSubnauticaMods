namespace CustomCraft2SML.Interfaces
{
    public interface ICustomBioFuel : ITechTyped, ICustomCraft
    {
        float Energy { get; }
    }
}
