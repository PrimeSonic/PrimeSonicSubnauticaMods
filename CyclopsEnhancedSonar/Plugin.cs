namespace CyclopsEnhancedSonar
{
    using System;
    using BepInEx;
    using Common;
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
            MODNAME = "Cyclops Enhanced Sonar",
            AUTHOR = "PrimeSonic",
            GUID = "com.cyclopsenhancedsonar.psmod",
            VERSION = "1.0.0.0";
        #endregion

        public void Awake()
        {
            QuickLogger.Info($"Started patching. Version {QuickLogger.GetAssemblyVersion()}");

            try
            {
                var harmony = new Harmony(GUID);
                harmony.Patch( // Create a postfix patch on the SubControl Start method to add the CySonarComponent
                    original: AccessTools.Method(typeof(SubControl), nameof(SubControl.Start)), 
                    postfix: new HarmonyMethod(typeof(Plugin), nameof(Plugin.SubControlStartPostfix)));

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
