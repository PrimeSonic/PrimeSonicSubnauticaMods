namespace CustomCraft2SML.Interfaces
{
    internal interface IParsingPackage
    {
        int ParseEntries(string serializedData);
        void PrePassValidation();
        void SendToSMLHelper();
        string ListKey { get; }
    }
}