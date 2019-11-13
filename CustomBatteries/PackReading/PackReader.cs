namespace CustomBatteries.PackReading
{
    using System.Collections.Generic;

    internal interface IPackReader
    {
        IEnumerable<IPluginPack> GetAllPacks(string folderLocation);
    }
}
