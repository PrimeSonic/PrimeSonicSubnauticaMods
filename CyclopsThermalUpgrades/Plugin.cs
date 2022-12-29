namespace CyclopsThermalUpgrades
{
    using System;
    using BepInEx;
    using Common;
    using CyclopsThermalUpgrades.Craftables;
    using CyclopsThermalUpgrades.Management;
    using MoreCyclopsUpgrades.API;
    using MoreCyclopsUpgrades.API.PDA;

    [BepInPlugin(GUID, MODNAME, VERSION)]
    [BepInDependency("com.ahk1221.smlhelper", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("com.morecyclopsupgrades.psmod", BepInDependency.DependencyFlags.HardDependency)]
    public class Plugin : BaseUnityPlugin
    {
        #region[Declarations]
        private const string
            MODNAME = "Cyclops Thermal Upgrades",
            AUTHOR = "PrimeSonic",
            GUID = "com.cyclopsthermalupgrades.psmod",
            VERSION = "1.0.0.0";
        #endregion

        public void Awake()
        {
            try
            {
                QuickLogger.Info($"Started patching. Version {QuickLogger.GetAssemblyVersion()}");

                var thermalMk2 = new CyclopsThermalChargerMk2();
                thermalMk2.Patch();

                MCUServices.Register.CyclopsUpgradeHandler((SubRoot cyclops) =>
                {
                    return new ThermalUpgradeHandler(TechType.CyclopsThermalReactorModule, thermalMk2.TechType, cyclops);
                });

                MCUServices.Register.CyclopsCharger<ThermalCharger>((SubRoot cyclops) =>
                {
                    return new ThermalCharger(thermalMk2.TechType, cyclops);
                });

                MCUServices.Register.PdaIconOverlay(TechType.CyclopsThermalReactorModule, CreateIconOverlay);
                MCUServices.Register.PdaIconOverlay(thermalMk2.TechType, CreateIconOverlay);

                QuickLogger.Info($"Finished patching.");
            }
            catch (Exception ex)
            {
                QuickLogger.Error(ex);
            }
        }

        internal static IconOverlay CreateIconOverlay(uGUI_ItemIcon icon, InventoryItem upgradeModule)
        {
            return new ThermalIconOverlay(icon, upgradeModule);
        }
    }
}
