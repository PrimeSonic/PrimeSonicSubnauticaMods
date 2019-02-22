namespace CustomCraft2SML.Interfaces
{
    using CustomCraft2SML.Serialization;

    internal interface ICustomCraft
    {
        string ID { get; }
        bool PassesPreValidation();
        bool SendToSMLHelper();

        OriginFile Origin { get; set; }
    }
}