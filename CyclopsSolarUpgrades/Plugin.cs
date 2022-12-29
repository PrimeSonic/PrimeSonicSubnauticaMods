namespace CyclopsSolarUpgrades
{
    using System;
    using BepInEx;
    using Common;
    using CyclopsSolarUpgrades.Craftables;
    using CyclopsSolarUpgrades.Management;
    using MoreCyclopsUpgrades.API;
    using MoreCyclopsUpgrades.API.PDA;

    [BepInPlugin(GUID, MODNAME, VERSION)]
    [BepInDependency("com.ahk1221.smlhelper", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("com.morecyclopsupgrades.psmod", BepInDependency.DependencyFlags.HardDependency)]
    public class Plugin : BaseUnityPlugin
    {
        #region[Declarations]
        private const string
            MODNAME = "Cyclops Solar Upgrades",
            AUTHOR = "PrimeSonic",
            GUID = "com.cyclopssolarupgrades.psmod",
            VERSION = "1.0.0.0";
        #endregion

        public void Awake()
        {
            try
            {
                QuickLogger.Info($"Started patching. Version {QuickLogger.GetAssemblyVersion()}");

                var solar1 = new CyclopsSolarCharger();
                var solar2 = new CyclopsSolarChargerMk2(solar1);

                solar1.Patch();
                solar2.Patch();

                MCUServices.Register.CyclopsCharger<SolarCharger>((SubRoot cyclops) =>
                {
                    return new SolarCharger(solar1.TechType, solar2.TechType, cyclops);
                });

                MCUServices.Register.CyclopsUpgradeHandler((SubRoot cyclops) =>
                {
                    return new SolarUpgradeHandler(solar1.TechType, solar2.TechType, cyclops);
                });

                MCUServices.Register.PdaIconOverlay(solar1.TechType, CreateIconOverlay);

                MCUServices.Register.PdaIconOverlay(solar2.TechType, CreateIconOverlay);

                QuickLogger.Info($"Finished patching.");
            }
            catch (Exception ex)
            {
                QuickLogger.Error(ex);
            }
        }

        internal static IconOverlay CreateIconOverlay(uGUI_ItemIcon icon, InventoryItem upgradeModule)
        {
            return new SolarIconOverlay(icon, upgradeModule);
        }
    }
}
