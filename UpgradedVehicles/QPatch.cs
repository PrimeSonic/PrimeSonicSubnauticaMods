namespace UpgradedVehicles
{
    using System;
    using System.Reflection;
    using Common;
    using Harmony;
    using SMLHelper.V2.Handlers;

    public class QPatch
    {
        public static void Patch()
        {
            try
            {
                QuickLogger.Message("Started patching - " + QuickLogger.GetAssemblyVersion());

                if (TechTypeHandler.TryGetModdedTechType("SeamothHullModule4", out TechType vehicleHullModule4) &&
                    TechTypeHandler.TryGetModdedTechType("SeamothHullModule5", out TechType vehicleHullModule5))
                {
                    VehicleUpgrader.SetModdedDepthModules(vehicleHullModule4, vehicleHullModule5);
                }

                SpeedBooster.HandlePatching();
                
                HarmonyInstance harmony = HarmonyInstance.Create("com.upgradedvehicles.psmod");
                harmony.PatchAll(Assembly.GetExecutingAssembly());

                QuickLogger.Message("Finished patching");
            }
            catch (Exception ex)
            {
                QuickLogger.Error("EXCEPTION on Patch: " + ex.ToString());
            }

        }
    }
}
