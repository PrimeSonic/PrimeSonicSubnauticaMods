namespace CyclopsAutoZapper
{
    using System.Reflection;
    using Common;
    using Harmony;
    using MoreCyclopsUpgrades.API;
    using MoreCyclopsUpgrades.API.Upgrades;

    public static class QPatch
    {
        public static void Patch()
        {
            QuickLogger.Info("Started patching " + QuickLogger.GetAssemblyVersion());

            var defenseSystem = new CyclopsAutoDefense();
            defenseSystem.Patch();

            var antiParasites = new CyclopsParasiteRemover();
            antiParasites.Patch();

            RegisterWithMoreCyclopsUpgrades(defenseSystem, antiParasites);

            var harmony = HarmonyInstance.Create("com.cyclopsautozapper.psmod");
            harmony.PatchAll(Assembly.GetExecutingAssembly());

            QuickLogger.Info("Finished Patching");
        }

        private static void RegisterWithMoreCyclopsUpgrades(CyclopsAutoDefense defenseSystem, CyclopsParasiteRemover antiParasites)
        {
            MCUServices.Register.CyclopsUpgradeHandler((SubRoot cyclops) =>
            {
                return new UpgradeHandler(defenseSystem.TechType, cyclops) { MaxCount = 1 };
            });

            MCUServices.Register.CyclopsUpgradeHandler((SubRoot cyclops) =>
            {
                return new UpgradeHandler(antiParasites.TechType, cyclops) { MaxCount = 1 };
            });

            MCUServices.Register.PdaIconOverlay(defenseSystem.TechType, (uGUI_ItemIcon icon, InventoryItem upgradeModule) =>
            {
                return new AutoDefenseIconOverlay(icon, upgradeModule);
            });

            MCUServices.Register.PdaIconOverlay(antiParasites.TechType, (uGUI_ItemIcon icon, InventoryItem upgradeModule) =>
            {
                return new AntiParasiteIconOverlay(icon, upgradeModule);
            });

            MCUServices.Register.AuxCyclopsManager<Zapper>((SubRoot cyclops) =>
            {
                return new Zapper(defenseSystem.TechType, cyclops);
            });
        }
    }
}
