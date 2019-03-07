namespace CustomCraft2SML.Interfaces.InternalUse
{
    using CustomCraft2SML.Serialization;

    internal interface IParsingPackage
    {
        int ParseEntries(string serializedData, OriginFile file);
        void PrePassValidation();
        void SendToSMLHelper();
        string ListKey { get; }
        string TypeName { get; }
        string[] TutorialText { get; }
    }
}