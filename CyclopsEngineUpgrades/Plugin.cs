namespace CyclopsEngineUpgrades
{
    using BepInEx;
    using Common;
    using CyclopsEngineUpgrades.Craftables;
    using CyclopsEngineUpgrades.Handlers;
    using MoreCyclopsUpgrades.API;
    using SMLHelper.V2.Handlers;

    [BepInPlugin(GUID, MODNAME, VERSION)]
    [BepInDependency("com.ahk1221.smlhelper", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("com.morecyclopsupgrades.psmod", BepInDependency.DependencyFlags.HardDependency)]
    public class Plugin : BaseUnityPlugin
    {
        #region[Declarations]
        private const string
            MODNAME = "Cyclops Engine Upgrades",
            AUTHOR = "PrimeSonic",
            GUID = "com.cyclopsengineupgrades.psmod",
            VERSION = "1.0.0.0";
        #endregion

        public void Awake()
        {
            MCUServices.Logger.Info($"Started patching. Version {QuickLogger.GetAssemblyVersion()}");

            var engineMk2Upgrade = new PowerUpgradeModuleMk2();
            var engineMk3Upgrade = new PowerUpgradeModuleMk3(engineMk2Upgrade);

            engineMk2Upgrade.Patch();
            engineMk3Upgrade.Patch();

            LanguageHandler.SetLanguageLine(EngineOverlay.BonusKey, "[Bonus Efficiency]");
            LanguageHandler.SetLanguageLine(EngineOverlay.TotalKey, "[Total Efficiency]");

            MCUServices.Register.CyclopsUpgradeHandler((SubRoot cyclops) =>
            {
                return new EngineHandler(engineMk2Upgrade, engineMk3Upgrade, cyclops);
            });

            MCUServices.Register.PdaIconOverlay(TechType.PowerUpgradeModule, CreateEngineOverlay);
            MCUServices.Register.PdaIconOverlay(engineMk2Upgrade.TechType, CreateEngineOverlay);
            MCUServices.Register.PdaIconOverlay(engineMk3Upgrade.TechType, CreateEngineOverlay);

            MCUServices.Logger.Info($"Finished patching.");
        }

        internal static EngineOverlay CreateEngineOverlay(uGUI_ItemIcon icon, InventoryItem upgradeModule)
        {
            return new EngineOverlay(icon, upgradeModule);
        }
    }
}
