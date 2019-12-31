namespace CustomBatteries.PackReading
{
    using CustomBatteries.API;

    internal interface IParsedPluginPack : ITextPluginPack
    {
        string PluginPackFolder { get; set; }
    }
}
