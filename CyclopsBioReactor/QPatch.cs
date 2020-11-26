namespace CyclopsBioReactor
{
    using System.Reflection;
    using Common;
    using CyclopsBioReactor.Items;
    using CyclopsBioReactor.Management;
    using HarmonyLib;
    using MoreCyclopsUpgrades.API;
    using QModManager.API.ModLoading;
    using SMLHelper.V2.Handlers;

    [QModCore]
    public static class QPatch
    {
        [QModPatch]
        public static void Patch()
        {
            MCUServices.Logger.Info("Started patching " + QuickLogger.GetAssemblyVersion());

            // Patch in new items
            var booster = new BioReactorBooster();
            booster.Patch();

            var reactor = new CyBioReactor(booster);
            reactor.Patch();

            // Apply Harmony patches
            var harmony = new Harmony("com.morecyclopsupgrades.psmod");
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
