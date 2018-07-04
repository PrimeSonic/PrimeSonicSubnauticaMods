namespace UpgradedVehicles
{
    using System;
    using System.Reflection;
    using Harmony;
    using SMLHelper.V2.Handlers;

    public class QPatch
    {
        public static void Patch()
        {
#if DEBUG
            try
            {
                Console.WriteLine($"[UpgradedVehicles] Start patching");
#endif
                SpeedBooster.Patch();
                VehiclePowerCore.Patch();
                SeaMothMk2.Patch();
                ExosuitMk2.Patch();

                if (CrossModSupportHandler.TryGetModdedTechType("SeamothHullModule4", out TechType seamothDepthMk4) &&
                    CrossModSupportHandler.TryGetModdedTechType("SeamothHullModule5", out TechType seamothDepthMk5))
                {
                    SeaMothMk3.Patch(seamothDepthMk4, seamothDepthMk5);
                }

                HarmonyInstance harmony = HarmonyInstance.Create("com.upgradedvehicles.psmod");
                harmony.PatchAll(Assembly.GetExecutingAssembly());
#if DEBUG
                Console.WriteLine($"[UpgradedVehicles] Finish patching");
            }
            catch (Exception ex)
            {
                Console.WriteLine("[UpgradedVehicles] EXCEPTION on Patch: " + ex.ToString());
            }
#endif
        }
    }
}
