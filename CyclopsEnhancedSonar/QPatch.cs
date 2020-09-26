namespace CyclopsEnhancedSonar
{
    using System;
    using Common;
    using HarmonyLib;
    using MoreCyclopsUpgrades.API;
    using SMLHelper.V2.Handlers;

    public static class QPatch
    {
        public static void Patch()
        {
            QuickLogger.Info($"Started patching. Version {QuickLogger.GetAssemblyVersion()}");

            try
            {
                var harmony = new Harmony("com.CyclopsEnhancedSonar.psmod");
                harmony.Patch( // Create a postfix patch on the SubControl Start method to add the CySonarComponent
                    original: AccessTools.Method(typeof(SubControl), nameof(SubControl.Start)), 
                    postfix: new HarmonyMethod(typeof(QPatch), nameof(QPatch.SubControlStartPostfix)));

                // Register a custom upgrade handler for the CyclopsSonarModule
                MCUServices.Register.CyclopsUpgradeHandler((SubRoot s) => new SonarUpgradeHandler(s));

                // Register a PDA Icon Overlay for the CyclopsSonarModule
                MCUServices.Register.PdaIconOverlay(TechType.CyclopsSonarModule, 
                                                    (uGUI_ItemIcon i, InventoryItem u) => new SonarPdaDisplay(i, u));

                // Add a language line for the text in the SonarPdaDisplay to allow it to be easily overridden
                LanguageHandler.SetLanguageLine(SonarPdaDisplay.SpeedUpKey, SonarPdaDisplay.SpeedUpText);

                QuickLogger.Info($"Finished patching.");
            }
            catch (Exception ex)
            {
                QuickLogger.Error(ex);
            }
        }

        internal static void SubControlStartPostfix(SubControl __instance)
        {
            if (__instance.gameObject.name.StartsWith("Cyclops-MainPrefab"))
                __instance.gameObject.AddComponent<CySonarComponent>();
        }
    }
}
