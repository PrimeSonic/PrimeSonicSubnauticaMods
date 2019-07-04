namespace CyclopsBioReactor
{
    using System;
    using System.Reflection;
    using Common;
    using CyclopsBioReactor.Items;
    using CyclopsBioReactor.Management;
    using Harmony;
    using MoreCyclopsUpgrades.API;

    public static class QPatch
    {
        public static void Patch()
        {
            try
            {
                QuickLogger.Info("Started patching " + QuickLogger.GetAssemblyVersion());

                var booster = new BioReactorBooster();
                booster.Patch();

                var reactor = new CyBioReactor(booster);
                reactor.Patch();

                var harmony = HarmonyInstance.Create("com.morecyclopsupgrades.psmod");
                harmony.PatchAll(Assembly.GetExecutingAssembly());

                MCUServices.Register.AuxCyclopsManager<BioAuxCyclopsManager>((SubRoot cyclops) =>
                {
                    return new BioAuxCyclopsManager(cyclops, booster.TechType, reactor.TechType);
                });

                MCUServices.Register.NonrenewableCyclopsCharger<BioChargeHandler>((SubRoot cyclops) =>
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

                QuickLogger.Info("Finished Patching");
            }
            catch (Exception ex)
            {
                QuickLogger.Error(ex);
            }
        }
    }
}
