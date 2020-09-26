namespace CyclopsBioReactor
{
    using System;
    using System.Reflection;
    using Common;
    using CyclopsBioReactor.Items;
    using CyclopsBioReactor.Management;
    using HarmonyLib;
    using MoreCyclopsUpgrades.API;
    using QModManager.API.ModLoading;

    [QModCore]
    public static class QPatch
    {
        [QModPatch]
        public static void Patch()
        {
            try
            {
                MCUServices.Logger.Info("Started patching " + QuickLogger.GetAssemblyVersion());

                var booster = new BioReactorBooster();
                booster.Patch();

                var reactor = new CyBioReactor(booster);
                reactor.Patch();

                var harmony = new Harmony("com.morecyclopsupgrades.psmod");
                harmony.PatchAll(Assembly.GetExecutingAssembly());

                MCUServices.Register.AuxCyclopsManager<BioAuxCyclopsManager>((SubRoot cyclops) =>
                {
                    return new BioAuxCyclopsManager(cyclops);
                });

                MCUServices.Register.CyclopsCharger<BioChargeHandler>((SubRoot cyclops) =>
                {
                    return new BioChargeHandler(booster.TechType, cyclops);
                });

                MCUServices.Register.CyclopsUpgradeHandler((SubRoot cyclops) =>
                {
                    return new BioBoosterUpgradeHandler(booster.TechType, cyclops);
                });

                MCUServices.Register.PdaIconOverlay(booster.TechType, (uGUI_ItemIcon icon, InventoryItem upgradeModule) =>
                {
                    return new BoosterOverlay(icon, upgradeModule);
                });

                MCUServices.Logger.Info("Finished Patching");
            }
            catch (Exception ex)
            {
                MCUServices.Logger.Error(ex);
            }
        }
    }
}
