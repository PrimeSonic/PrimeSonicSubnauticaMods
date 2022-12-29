namespace MoreCyclopsUpgrades
{
    using System.IO;
    using System.Reflection;
    using Common;
    using MoreCyclopsUpgrades.AuxConsole;
    using MoreCyclopsUpgrades.Config;
    using MoreCyclopsUpgrades.Managers;
    using SMLHelper.V2.Utility;
    using HarmonyLib;
    using BepInEx;

    /// <summary>
    /// Entry point class for patching.
    /// </summary>

    [BepInPlugin(GUID, MODNAME, VERSION)]
    [BepInDependency("com.ahk1221.smlhelper", BepInDependency.DependencyFlags.HardDependency)]
    public class Plugin : BaseUnityPlugin
    {
        #region[Declarations]
        private const string
            MODNAME = "MoreCyclopsUpgrades",
            AUTHOR = "PrimeSonic",
            GUID = "com.morecyclopsupgrades.psmod",
            VERSION = "1.0.0.0";
        #endregion

        /// <summary>
        /// Setting up the mod config.
        /// </summary>
        static Plugin()
        {
            ModConfig.LoadOnDemand();
            string executingLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            CyclopsHUDManager.CyclopsThermometer = ImageUtils.LoadSpriteFromFile(executingLocation + "/Assets/CyclopsThermometer.png");
        }

        /// <summary>
        /// Main patching method.
        /// </summary>
        public void Awake()
        {
            QuickLogger.Info("Started patching " + QuickLogger.GetAssemblyVersion());

            // If enabled, patch the Auxiliary Upgrade Console as a new buildable.
            if (ModConfig.Main.AuxConsoleEnabled)
            {
                var console = new AuxCyUpgradeConsole();
                console.Patch();
            }
            else
            {
                // SMLHelper now handles previously used but now disabled TechTypes
                QuickLogger.Info("Auxiliary Upgrade Console disabled by config settings");
            }

            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), "com.morecyclopsupgrades.psmod");

            QuickLogger.Info("Finished Patching");
        }
    }
}
