namespace UpgradedVehicles
{
    using System;
    using System.Reflection;
    using Common;
    using Harmony;
    using SMLHelper.V2.Assets;
    using SMLHelper.V2.Handlers;
    using UpgradedVehicles.SaveData;

    public class QPatch
    {
        private static UpgradeOptions configOptions;
        private static Craftable speedModule;
        private static Craftable HullArmorMk2Module;
        private static Craftable HullArmorMk3Module;
        private static Craftable HullArmorMk4Module;

        public static void Patch()
        {
            try
            {
                QuickLogger.Info("Started patching - " + QuickLogger.GetAssemblyVersion());

                //Handle CrossMod Updates
                if (TechTypeHandler.TryGetModdedTechType("SeamothHullModule4", out TechType vehicleHullModule4) &&
                    TechTypeHandler.TryGetModdedTechType("SeamothHullModule5", out TechType vehicleHullModule5))
                {
                    VehicleUpgrader.SetModdedDepthModules(vehicleHullModule4, vehicleHullModule5);
                }
                //Handle SpeedBooster
                speedModule = SpeedBooster.GetSpeedBoosterCraftable();
                VehicleUpgrader.SetNewModule(speedModule);
                
                //Handle HullArmorUpgrades (ugly and repetitive...)
                HullArmorMk2Module = HullArmorMk2.GetHullArmorMk2Craftable();
                HullArmorMk3Module = HullArmorMk3.GetHullArmorMk3Craftable();
                HullArmorMk4Module = HullArmorMk4.GetHullArmorMk4Craftable();
                VehicleUpgrader.SetNewModule(HullArmorMk2Module);
                VehicleUpgrader.SetNewModule(HullArmorMk3Module);
                VehicleUpgrader.SetNewModule(HullArmorMk4Module);

                //Handle Config Options
                configOptions = new UpgradeOptions();
                configOptions.Initialize();

                VehicleUpgrader.SetBonusSpeedMultipliers(configOptions.SeamothBonusSpeedMultiplier, configOptions.ExosuitBonusSpeedMultiplier);

                var harmony = HarmonyInstance.Create("com.upgradedvehicles.psmod");
                harmony.PatchAll(Assembly.GetExecutingAssembly());

                QuickLogger.Info("Finished patching");
            }
            catch (Exception ex)
            {
                QuickLogger.Error("EXCEPTION on Patch: " + ex.ToString());
            }
        }
    }
}
