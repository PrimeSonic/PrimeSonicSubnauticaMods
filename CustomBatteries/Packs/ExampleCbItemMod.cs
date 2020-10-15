#if EXAMPLE // EXAMPLE MOD
namespace CustomBatteries.Packs
{
    using System.Collections.Generic;
    using CustomBatteries.API;
    using QModManager.API.ModLoading;

    [QModCore]
    public static class ExampleCbItemMod
    {
        [QModPatch]
        public static void MainPatch()
        {
            var myBattery = new CbBattery
            {
                EnergyCapacity = 200,
                ID = "MyBatteryID",
                Name = "My Cool Battery",
                FlavorText = "Hey, I made a battery!",
                UnlocksWith = TechType.Lithium,
                CraftingMaterials = new List<TechType>
                {
                    TechType.Lithium, TechType.Lithium,
                    TechType.Titanium, TechType.Copper,
                    TechType.WhiteMushroom
                }
            };

            myBattery.Patch();

            var myPowerCell = new CbPowerCell
            {
                EnergyCapacity = myBattery.EnergyCapacity * 2,
                ID = "MyBatteryID",
                Name = "My Cool Battery",
                FlavorText = "Hey, I made a power cell!",
                UnlocksWith = TechType.Lithium,
                CraftingMaterials = new List<TechType>
                {
                    myBattery.TechType, myBattery.TechType,
                    TechType.Silicone
                }
            };

            myPowerCell.Patch();
        }
    }
}
#endif