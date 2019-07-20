namespace IonCubeGenerator.Interfaces
{
    internal interface ICubeProduction
    {
        bool NotAllowToGenerate { get; }

        float StartUpPercent { get; }
        float GenerationPercent { get; }
        float CoolDownPercent { get; }
    }
}