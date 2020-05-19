namespace CustomCraft2SML.Interfaces.InternalUse
{
    using CustomCraft2SML.Serialization;

    internal interface ICustomCraft
    {
        string ID { get; }
        bool PassesPreValidation(OriginFile originFile);
        bool PassedSecondValidation { get; }
        bool SendToSMLHelper();

        OriginFile Origin { get; set; }
        string[] TutorialText { get; }
    }
}