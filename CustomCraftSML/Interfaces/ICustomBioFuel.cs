namespace CustomCraft2SML.Interfaces
{
    internal interface ICustomBioFuel : ITechTyped, ICustomCraft
    {
        float Energy { get; }
    }
}
