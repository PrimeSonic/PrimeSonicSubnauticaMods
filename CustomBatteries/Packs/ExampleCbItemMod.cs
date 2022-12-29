#if EXAMPLE // EXAMPLE MOD
namespace CustomBatteries.Packs
{
    using System.Collections.Generic;
    using CustomBatteries.API;
    using BepInEx;

    [BepInPlugin(GUID, MODNAME, VERSION)]
    [BepInDependency("com.custombatteries.psmod", BepInDependency.DependencyFlags.HardDependency)]
    public class Plugin : BaseUnityPlugin
    {
        private const string
            MODNAME = "Example CBItem Battery Mod",
            AUTHOR = "PrimeSonic|MrPurple6411",
            GUID = "com.examplecbitembatterymod.psmod",
            VERSION = "1.0.0.0";

        public void Awake()
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