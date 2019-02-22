namespace CustomCraft2SML.Interfaces
{
    using CustomCraft2SML.Serialization;

    internal interface IParsingPackage
    {
        int ParseEntries(string serializedData, OriginFile file);
        void PrePassValidation();
        void SendToSMLHelper();
        string ListKey { get; }
        string TypeName { get; }
    }
}