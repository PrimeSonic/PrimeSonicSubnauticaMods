namespace UpgradedVehicles
{
    using System;
    using System.Reflection;
    using Harmony;

    public class QPatch
    {
        public static void Patch()
        {
#if DEBUG
            try
            {
                Console.WriteLine($"[UpgradedVehicles] Start patching");
#endif

                SeaMothMk2.Patch();


                HarmonyInstance harmony = HarmonyInstance.Create("com.upgradedvehicles.psmod");
                harmony.PatchAll(Assembly.GetExecutingAssembly());
#if DEBUG
                Console.WriteLine($"[UpgradedVehicles] Finish patching");
            }
            catch (Exception ex)
            {
                Console.WriteLine("[UpgradedVehicles] ERROR: " + ex.ToString());
            }
#endif
        }
    }
}
