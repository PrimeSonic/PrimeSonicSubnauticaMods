namespace IonCubeGenerator.Interfaces
{
    internal interface ICubeProduction
    {
        float CubeProgress { get; }
        float RemainingTimeToNextCube { get; set; }
    }
}