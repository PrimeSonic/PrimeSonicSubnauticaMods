namespace UpgradedVehicles
{
    using System;
    using System.Reflection;
    using Common;
    using HarmonyLib;
    using QModManager.API;
    using QModManager.API.ModLoading;
    using SMLHelper.V2.Handlers;
    using UpgradedVehicles.SaveData;

    [QModCore]
    public class QPatch
    {
        internal const string WorkBenchTab = "HullArmor";

        [QModPatch]
        public static void Patch()
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
#if SUBNAUTICA
            QuickLogger.Info("Checking if MoreSeamothDepth mod is present");

            IQMod moreSeamothDepth = QModServices.Main.FindModById("MoreSeamothDepth");
            if (moreSeamothDepth != null &&
                TechTypeHandler.TryGetModdedTechType("SeamothHullModule4", out TechType vehicleHullModule4) &&
                TechTypeHandler.TryGetModdedTechType("SeamothHullModule5", out TechType vehicleHullModule5))
            {
                QuickLogger.Info("Detected Seamoth Depth Modules Mk4 & Mk5");
                // the AddDepthModule will add the module to the common upgrades set, the common depth modules set, and the vehicle-specific set
                VehicleUpgrader.AddDepthModule(vehicleHullModule4, 4, VehicleUpgrader.EVehicleType.Seamoth);
                VehicleUpgrader.AddDepthModule(vehicleHullModule5, 5, VehicleUpgrader.EVehicleType.Seamoth);
            }
#elif BELOWZERO
            QuickLogger.Info("Checking if SeaTruckDepthUpgrades mod is present");

            IQMod moreSeamothDepth = QModServices.Main.FindModById("SeaTruckDepthUpgrades");
            if (moreSeamothDepth != null &&
                TechTypeHandler.TryGetModdedTechType("SeaTruckDepthMK4", out TechType vehicleHullModule4) &&
                TechTypeHandler.TryGetModdedTechType("SeaTruckDepthMK5", out TechType vehicleHullModule5) &&
                TechTypeHandler.TryGetModdedTechType("SeaTruckDepthMK6", out TechType vehicleHullModule6))
            {
                QuickLogger.Info("Detected SeaTruck Depth Modules Mk4, Mk5 and Mk6");
                VehicleUpgrader.AddDepthModule(vehicleHullModule4, 4, VehicleUpgrader.EVehicleType.Seatruck);
                VehicleUpgrader.AddDepthModule(vehicleHullModule5, 5, VehicleUpgrader.EVehicleType.Seatruck);
                VehicleUpgrader.AddDepthModule(vehicleHullModule6, 6, VehicleUpgrader.EVehicleType.Seatruck);
            }

            IQMod seatruckSpeed = QModServices.Main.FindModById("SeaTruckSpeedUpgrades");
            if (seatruckSpeed != null
                && TechTypeHandler.TryGetModdedTechType("SeaTruckSpeedMK1", out TechType speedMk1)
                && TechTypeHandler.TryGetModdedTechType("SeaTruckSpeedMK2", out TechType speedMk2)
                && TechTypeHandler.TryGetModdedTechType("SeaTruckSpeedMK3", out TechType speedMk3))
            {
                QuickLogger.Info("Detected Seatruck Speed Modules Mk1, Mk2 and Mk3");
                VehicleUpgrader.AddSpeedModifier(speedMk1, 1f);
                VehicleUpgrader.AddSpeedModifier(speedMk2, 2f);
                VehicleUpgrader.AddSpeedModifier(speedMk3, 3f);
            }

            IQMod seatruckArmour = QModServices.Main.FindModById("SeaTruckArmorUpgrades");
            if (seatruckSpeed != null
                && TechTypeHandler.TryGetModdedTechType("SeaTruckArmorMK1", out TechType armour1)
                && TechTypeHandler.TryGetModdedTechType("SeaTruckArmorMK2", out TechType armour2)
                && TechTypeHandler.TryGetModdedTechType("SeaTruckArmorMK3", out TechType armour3))
            {
                QuickLogger.Info("Detected Seatruck Armour Modules Mk1, Mk2 and Mk3");
                VehicleUpgrader.AddArmourModule(armour1, 1);
                VehicleUpgrader.AddArmourModule(armour2, 2);
                VehicleUpgrader.AddArmourModule(armour3, 3);
            }
#endif
        }
    }
}
