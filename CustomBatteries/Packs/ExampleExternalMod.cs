#if EXAMPLE // EXAMPLE MOD
namespace CustomBatteries.Packs
{
    // You will need to include these namespaces to use the API classes and interfaces
    using System.Collections.Generic;
    using CustomBatteries.API;

    // Step 0 - You will need to make your own class that implements the IModPluginPack interface.
    // It can be as simple as this example right here.
    internal class MyPlugin : IModPluginPack
    {
        // ProTip: You can load your own sprites directly from PNG files using SMLHelper's ImageUtils class.
        public Atlas.Sprite BatteryIcon => SpriteManager.Get(TechType.Battery);
        public Atlas.Sprite PowerCellIcon => SpriteManager.Get(TechType.PowerCell);
        public string PluginPackName => "Example Plugin";
        public int BatteryCapacity => 120; // Power cells are always twice the capacity of the battery
        public TechType UnlocksWith => TechType.Shocker;
        public string BatteryID => "AmpBattery";
        public string BatteryName => "Ampeel Battery";
        public string BatteryFlavorText => "Recycle these annoying creatures into something more useful.";
        public IList<TechType> BatteryParts => new List<TechType> { TechType.Shocker, TechType.Titanium, TechType.Copper };
        public string PowerCellID => "AmpPowerCell";
        public string PowerCellName => "Ampeel Power Cell";
        public string PowerCellFlavorText => "Twice the power at twice the size!";
        public IList<TechType> PowerCellAdditionalParts => new List<TechType> { TechType.Lithium };
    }

    internal static class MyQPatch
    {
        public static void Patch()
        {
            // Step 1 - Instantiate the service class
            var cbService = new CustomBatteriesService();

            // Step 2 - Get your plugin pack instance
            var myPlugin = new MyPlugin();

            // Step 3 - Call the AddPluginPackFromMod method with your plugin pack            
            CustomPack myPack = cbService.AddPluginPackFromMod(myPlugin);

            // It will return a CustomPack that has everything you might need to continue patching.
            // The ModPrefabs CustomBattery and CustomPowerCell are already patched.
            TechType myBattery = myPack.CustomBattery.TechType;
            TechType myPowercell = myPack.CustomPowerCell.TechType;

            // Do whatever other patching your mod requires
        }
    }
}
#endif
