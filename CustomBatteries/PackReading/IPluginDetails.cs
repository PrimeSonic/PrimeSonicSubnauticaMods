namespace CustomBatteries.PackReading
{
    using CustomBatteries.API;

    internal interface IPluginDetails : IPluginPack
    {
        string PluginPackFolder { get; set; }
    }
}
