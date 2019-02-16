namespace CustomCraft2SML.Serialization
{
    internal interface IParsingPackage
    {
        int ParseEntries(string serializedData);
        void PrePassValidation();
        void SendToSMLHelper();
        string ListKey { get; }
    }
}