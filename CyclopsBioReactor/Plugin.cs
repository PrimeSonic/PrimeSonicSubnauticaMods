namespace CyclopsBioReactor
{
    using System.Reflection;
    using BepInEx;
    using Common;
    using CyclopsBioReactor.Items;
    using CyclopsBioReactor.Management;
    using HarmonyLib;
    using MoreCyclopsUpgrades.API;
    using SMLHelper.V2.Handlers;

    [BepInPlugin(GUID, MODNAME, VERSION)]
    [BepInDependency("com.ahk1221.smlhelper", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("com.morecyclopsupgrades.psmod", BepInDependency.DependencyFlags.HardDependency)]
    public class Plugin : BaseUnityPlugin
    {
        #region[Declarations]
        private const string
            MODNAME = "Cyclops Bio-Reactor",
            AUTHOR = "PrimeSonic",
            GUID = "com.cyclopsbioreactor.psmod",
            VERSION = "1.0.0.0";
        #endregion

        public void Awake()
        {
            MCUServices.Logger.Info("Started patching " + QuickLogger.GetAssemblyVersion());

            // Patch in new items
            var booster = new BioReactorBooster();
            booster.Patch();

            var reactor = new CyBioReactor(booster);
            reactor.Patch();

            // Apply Harmony patches
            var harmony = new Harmony(GUID);
            harmony.PatchAll(Assembly.GetExecutingAssembly());

            // Register with MoreCyclopsUpgrades
            MCUServices.Register.AuxCyclopsManager<BioAuxCyclopsManager>((SubRoot cyclops) => new BioAuxCyclopsManager(cyclops));
            MCUServices.Register.CyclopsCharger<BioChargeHandler>((SubRoot cyclops) => new BioChargeHandler(booster.TechType, cyclops));
            MCUServices.Register.CyclopsUpgradeHandler((SubRoot cyclops) => new BioBoosterUpgradeHandler(booster.TechType, cyclops));
            MCUServices.Register.PdaIconOverlay(booster.TechType, (uGUI_ItemIcon icon, InventoryItem upgradeModule) => new BoosterOverlay(icon, upgradeModule));

            // Register config for display
            CyBioReactorDisplayHandler.Config = OptionsPanelHandler.Main.RegisterModOptions<CyBioConfig>();

            MCUServices.Logger.Info("Finished Patching");
        }
    }
}
