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

            var zapper = new CyclopsAutoDefense();
            zapper.Patch();

            MCUServices.Register.CyclopsUpgradeHandler((SubRoot cyclops) =>
            {
                return new UpgradeHandler(zapper.TechType, cyclops)
                {
                    MaxCount = 2,                    
                };
            });

            MCUServices.Register.PdaIconOverlay(zapper.TechType, (uGUI_ItemIcon icon, InventoryItem upgradeModule) =>
            {
                return new AutoDefenseIconOverlay(zapper.TechType, icon, upgradeModule);
            });

            MCUServices.Register.AuxCyclopsManager<Zapper>((SubRoot cyclops) =>
            {
                return new Zapper(zapper.TechType, cyclops);
            });

            var harmony = HarmonyInstance.Create("com.cyclopsautozapper.psmod");
            harmony.PatchAll(Assembly.GetExecutingAssembly());

            QuickLogger.Info("Finished Patching");
        }
    }
}
