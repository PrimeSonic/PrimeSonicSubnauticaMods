#if DEBUG
namespace CustomBatteries.Packs
{
    using System.Collections.Generic;
    using CustomBatteries.API;

    internal class MyPack : IModPluginPack
    {
        public Atlas.Sprite BatteryIcon { get; } = SpriteManager.Get(TechType.Battery);
        public Atlas.Sprite PowerCellIcon { get; } = SpriteManager.Get(TechType.PowerCell);
        public string PluginPackName { get; } = "Example Plugin";
        public int BatteryCapacity { get; } = 120;
        public TechType UnlocksWith { get; } = TechType.Shocker;
        public string BatteryID { get; } = "AmpBattery";
        public string BatteryName { get; } = "Ampeel Battery";
        public string BatteryFlavorText { get; } = "Recycle these annoying creatures into something more useful.";
        public IList<TechType> BatteryParts { get; } = new List<TechType> { TechType.Shocker, TechType.Titanium, TechType.Copper };
        public string PowerCellID { get; } = "AmpPowerCell";
        public string PowerCellName { get; } = "Ampeel Power Cell";
        public string PowerCellFlavorText { get; } = "Twice the power at twice the size!";
        public IList<TechType> PowerCellAdditionalParts { get; } = new List<TechType>();
    }

    internal static class MyQPatch
    {
        public static void Patch()
        {
            // Sending your mod pack to CustomBatteries

            // Step 1 - Instantiate the service class
            var service = new CustomBatteriesService();

            // Step 2 - Get your pluging pack
            var myPack = new MyPack();

            // Step 3 - Call the AddPluginPackFromMod method with your pluging pack
            service.AddPluginPackFromMod(myPack);
        }
    }
}
#endif