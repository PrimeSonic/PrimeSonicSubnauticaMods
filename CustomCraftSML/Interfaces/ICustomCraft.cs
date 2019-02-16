namespace CustomCraft2SML.Interfaces
{
    public interface ICustomCraft
    {
        string ID { get; }
        bool PassesPreValidation();
        bool SendToSMLHelper();
    }
}