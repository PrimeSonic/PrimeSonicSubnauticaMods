namespace CustomCraft2SML.Interfaces
{
    internal interface IModifiedFood : ITechTyped
    {
        short FoodValue { get; }
        short WaterValue { get; }
    }
}
