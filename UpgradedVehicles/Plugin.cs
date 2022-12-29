namespace UpgradedVehicles
{
    using System;
    using System.Reflection;
    using BepInEx;
    using Common;
    using HarmonyLib;
    using SMLHelper.V2.Handlers;
    using UpgradedVehicles.SaveData;


    [BepInPlugin(GUID, MODNAME, VERSION)]
    [BepInDependency("com.ahk1221.smlhelper", BepInDependency.DependencyFlags.HardDependency)]
    public class Plugin : BaseUnityPlugin
    {
        #region[Declarations]
        private const string
            MODNAME = "Upgraded Vehicles",
            AUTHOR = "PrimeSonic",
            GUID = "com.upgradedvehicles.psmod",
            VERSION = "1.0.0.0";

        internal const string WorkBenchTab = "HullArmor";
        #endregion

        public void Awake()
        {
            try
            {
                QuickLogger.Info("Started patching - " + QuickLogger.GetAssemblyVersion());

                //Handle Config Options
                var configOptions = new UpgradeOptions();
                configOptions.Initialize();

                QuickLogger.DebugLogsEnabled = configOptions.DebugLogsEnabled;
                QuickLogger.Debug("Debug logs enabled");

                CrossModUpdates();

                CraftTreeHandler.AddTabNode(CraftTree.Type.Workbench, WorkBenchTab, "Armor Modules", SpriteManager.Get(TechType.VehicleArmorPlating));

                //Handle SpeedBooster
                var speedModule = new SpeedBooster();
                speedModule.Patch();

                //Handle HullArmorUpgrades
                var hullArmorMk2Module = new HullArmorMk2();
                hullArmorMk2Module.Patch();

                var hullArmorMk3Module = new HullArmorMk3(hullArmorMk2Module.TechType);
                hullArmorMk3Module.Patch();

                var hullArmorMk4Module = new HullArmorMk4(hullArmorMk3Module.TechType);
                hullArmorMk4Module.Patch();

                VehicleUpgrader.SetBonusSpeedMultipliers(configOptions);

                var harmony = new Harmony("com.upgradedvehicles.psmod");
                harmony.PatchAll(Assembly.GetExecutingAssembly());

                QuickLogger.Info("Finished patching");
            }
            catch (Exception ex)
            {
                QuickLogger.Error(ex);
            }
        }

        public static void CrossModUpdates()
        {
            QuickLogger.Info("Checking if MoreSeamothDepth mod is present");
            if (TechTypeHandler.TryGetModdedTechType("SeamothHullModule4", out TechType vehicleHullModule4) &&
                TechTypeHandler.TryGetModdedTechType("SeamothHullModule5", out TechType vehicleHullModule5))
            {
                QuickLogger.Info("Detected Seamoth Depth Modules Mk4 & Mk5");
                VehicleUpgrader.SeamothDepthModules.Add(vehicleHullModule4, 4);
                VehicleUpgrader.SeamothDepthModules.Add(vehicleHullModule5, 5);
                VehicleUpgrader.CommonUpgradeModules.Add(vehicleHullModule4);
                VehicleUpgrader.CommonUpgradeModules.Add(vehicleHullModule5);
                VehicleUpgrader.DepthUpgradeModules.Add(vehicleHullModule4);
                VehicleUpgrader.DepthUpgradeModules.Add(vehicleHullModule5);
            }
        }
    }
}
