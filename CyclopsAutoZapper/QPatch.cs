namespace CyclopsAutoZapper
{
    using System.Reflection;
    using Common;
    using CyclopsAutoZapper.Managers;
    using Harmony;
    using MoreCyclopsUpgrades.API;
    using MoreCyclopsUpgrades.API.Upgrades;

    public static class QPatch
    {
        public static void Patch()
        {
            QuickLogger.Info("Started patching " + QuickLogger.GetAssemblyVersion());

            QuickLogger.Info("Patching SMLHelper items");

            var defenseSystem = new CyclopsAutoDefense();
            defenseSystem.Patch();

            var antiParasites = new CyclopsParasiteRemover();
            antiParasites.Patch();

            QuickLogger.Info("Registering with More Cyclops Upgrades");
            RegisterWithMoreCyclopsUpgrades(defenseSystem, antiParasites);

            var harmony = HarmonyInstance.Create("com.cyclopsautozapper.psmod");
            harmony.PatchAll(Assembly.GetExecutingAssembly());

            QuickLogger.Info("Finished Patching");
        }

        private static void RegisterWithMoreCyclopsUpgrades(CyclopsAutoDefense defenseSystem, CyclopsParasiteRemover antiParasites)
        {
            // Reigster upgrade handlers
            MCUServices.Register.CyclopsUpgradeHandler((SubRoot cyclops) =>
            { return new UpgradeHandler(defenseSystem.TechType, cyclops) { MaxCount = 1 }; });

            MCUServices.Register.CyclopsUpgradeHandler((SubRoot cyclops) =>
            { return new UpgradeHandler(antiParasites.TechType, cyclops) { MaxCount = 1 }; });

            // Register PDA Icon Overlays
            MCUServices.Register.PdaIconOverlay(defenseSystem.TechType, (uGUI_ItemIcon icon, InventoryItem upgradeModule) =>
            { return new AutoDefenseIconOverlay(icon, upgradeModule); });

            MCUServices.Register.PdaIconOverlay(antiParasites.TechType, (uGUI_ItemIcon icon, InventoryItem upgradeModule) =>
            { return new AntiParasiteIconOverlay(icon, upgradeModule); });

            // Register Aux Managers
            MCUServices.Register.AuxCyclopsManager<AutoDefenser>((SubRoot cyclops) =>
            { return new AutoDefenser(defenseSystem.TechType, cyclops); });

            MCUServices.Register.AuxCyclopsManager<ShieldPulser>((SubRoot cyclops) =>
            { return new ShieldPulser(antiParasites.TechType, cyclops); });
        }
    }
}
