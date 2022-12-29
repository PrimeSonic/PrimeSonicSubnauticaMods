namespace CyclopsNuclearUpgrades
{
    using BepInEx;
    using Common;
    using CyclopsNuclearUpgrades.Management;
    using MoreCyclopsUpgrades.API;

    [BepInPlugin(GUID, MODNAME, VERSION)]
    [BepInDependency("com.ahk1221.smlhelper", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("com.morecyclopsupgrades.psmod", BepInDependency.DependencyFlags.HardDependency)]
    public class Plugin : BaseUnityPlugin
    {
        #region[Declarations]
        private const string
            MODNAME = "Cyclops Nuclear Upgrades",
            AUTHOR = "PrimeSonic",
            GUID = "com.cyclopsnuclearupgrades.psmod",
            VERSION = "1.0.0.0";
        #endregion

        public void Awake()
        {
            MCUServices.Logger.Info("Started patching v" + QuickLogger.GetAssemblyVersion());

            var nuclearModule = new CyclopsNuclearModule();
            var depletedModule = new DepletedNuclearModule(nuclearModule);

            nuclearModule.Patch();
            depletedModule.Patch();

            var nuclearFabricator = new NuclearFabricator(nuclearModule);
            nuclearFabricator.AddCraftNode(TechType.ReactorRod);
            nuclearFabricator.AddCraftNode(nuclearModule.TechType);
            nuclearFabricator.AddCraftNode("RReactorRodDUMMY"); // Optional - Refill nuclear reactor rod (old)
            nuclearFabricator.AddCraftNode("ReplenishReactorRod"); // Optional - Refill nuclear reactor rod (new)
            nuclearFabricator.AddCraftNode("CyNukeUpgrade1"); // Optional - Cyclops Nuclear Reactor Enhancer Mk1
            nuclearFabricator.AddCraftNode("CyNukeUpgrade2"); // Optional - Cyclops Nuclear Reactor Enhancer Mk2
            nuclearFabricator.Patch();

            MCUServices.Register.CyclopsUpgradeHandler((SubRoot cyclops) =>
            {
                return new NuclearUpgradeHandler(nuclearModule.TechType, cyclops);
            });

            MCUServices.Register.CyclopsCharger<NuclearChargeHandler>((SubRoot cyclops) =>
            {
                return new NuclearChargeHandler(cyclops, nuclearModule.TechType);
            });

            MCUServices.Register.PdaIconOverlay(nuclearModule.TechType, (uGUI_ItemIcon icon, InventoryItem upgradeModule) =>
            {
                return new NuclearIconOverlay(icon, upgradeModule);
            });

            MCUServices.Logger.Info("Finished patching");
        }
    }
}
