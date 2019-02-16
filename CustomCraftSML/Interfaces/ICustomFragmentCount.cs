namespace CustomCraft2SML.Interfaces
{
    interface ICustomFragmentCount : ITechTyped, ICustomCraft
    {
        int FragmentsToScan { get; }
    }
}
