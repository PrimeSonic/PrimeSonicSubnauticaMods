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

            var defenseSystemMk2 = new CyclopsAutoDefenseMk2(defenseSystem);
            defenseSystemMk2.Patch();

            QuickLogger.Info("Registering Upgrade Handlers with More Cyclops Upgrades");

            MCUServices.Register.CyclopsUpgradeHandler((SubRoot cyclops) =>
            { return new UpgradeHandler(defenseSystem.TechType, cyclops) { MaxCount = 1 }; });

            MCUServices.Register.CyclopsUpgradeHandler((SubRoot cyclops) =>
            { return new UpgradeHandler(antiParasites.TechType, cyclops) { MaxCount = 1 }; });

            MCUServices.Register.CyclopsUpgradeHandler((SubRoot cyclops) =>
            { return new UpgradeHandler(defenseSystemMk2.TechType, cyclops) { MaxCount = 1 }; });

            QuickLogger.Info("Registering PDA Icon Overlays with More Cyclops Upgrades");

            MCUServices.Register.PdaIconOverlay(defenseSystem.TechType, (uGUI_ItemIcon icon, InventoryItem upgradeModule) =>
            { return new AutoDefenseIconOverlay(icon, upgradeModule); });

            MCUServices.Register.PdaIconOverlay(antiParasites.TechType, (uGUI_ItemIcon icon, InventoryItem upgradeModule) =>
            { return new AntiParasiteIconOverlay(icon, upgradeModule); });

            MCUServices.Register.PdaIconOverlay(defenseSystem.TechType, (uGUI_ItemIcon icon, InventoryItem upgradeModule) =>
            { return new AutoDefenseMk2IconOverlay(icon, upgradeModule); });

            QuickLogger.Info("Registering Aux Managers with More Cyclops Upgrades");

            MCUServices.Register.AuxCyclopsManager<AutoDefenser>((SubRoot cyclops) =>
            { return new AutoDefenser(defenseSystem.TechType, cyclops); });

            MCUServices.Register.AuxCyclopsManager<ShieldPulser>((SubRoot cyclops) =>
            { return new ShieldPulser(antiParasites.TechType, cyclops); });

            MCUServices.Register.AuxCyclopsManager<AutoDefenserMk2>((SubRoot cyclops) =>
            { return new AutoDefenserMk2(defenseSystemMk2.TechType, cyclops); });

            QuickLogger.Info("Applying Harmony Patches");

            var harmony = HarmonyInstance.Create("com.cyclopsautozapper.psmod");
            harmony.PatchAll(Assembly.GetExecutingAssembly());

            QuickLogger.Info("Finished Patching");
        }
    }
}
