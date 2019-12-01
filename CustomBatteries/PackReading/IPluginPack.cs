namespace CustomBatteries.PackReading
{
    using System.Collections.Generic;

    public interface IPluginPack
    {
        string PluginPackName { get; }

        int BatteryCapacity { get; }

        TechType UnlocksWith { get; }

        string BatteryID { get; }

        string BatteryName { get; }

        string BatterFlavorText { get; }

        IList<TechType> BatteryParts { get; }

        string BatteryIconFile { get; }

        string PowerCellID { get; }

        string PowerCellName { get; }

        string PowerCellFlavorText { get; }

        IList<TechType> PowerCellAdditionalParts { get; }

        string PowerCellIconFile { get; }
    }
}
