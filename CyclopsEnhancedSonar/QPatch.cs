namespace CyclopsEnhancedSonar
{
    using System;
    using System.Reflection;
    using Common;
    using Harmony;
    using MoreCyclopsUpgrades.API;

    public static class QPatch
    {
        public static void Patch()
        {
            QuickLogger.Info($"Started patching. Version {QuickLogger.GetAssemblyVersion()}");

            try
            {                
                MethodInfo subControlStartMethod = AccessTools.Method(typeof(SubControl), nameof(SubControl.Start));

                var harmony = HarmonyInstance.Create("com.CyclopsEnhancedSonar.psmod");
                harmony.Patch(subControlStartMethod, postfix: new HarmonyMethod(typeof(QPatch), nameof(QPatch.SubControlStartPostfix)));
                
                MCUServices.Register.CyclopsUpgradeHandler((SubRoot cyclops) => new SonarUpgradeHandler(cyclops));
                MCUServices.Register.PdaIconOverlay(TechType.CyclopsSonarModule, (uGUI_ItemIcon icon, InventoryItem upgradeModule) => new SonarPdaDisplay(icon, upgradeModule));

                QuickLogger.Info($"Finished patching.");
            }
            catch (Exception ex)
            {
                QuickLogger.Error(ex);
            }
        }

        public static void SubControlStartPostfix(SubControl __instance)
        {
            if (__instance.gameObject.name.StartsWith("Cyclops-MainPrefab"))
                __instance.gameObject.AddComponent<CySonarComponent>();
        }
    }
}
