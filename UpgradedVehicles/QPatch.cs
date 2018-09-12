namespace UpgradedVehicles
{
    using System;
    using System.Reflection;
    using Common;
    using Harmony;

    public class QPatch
    {
        public static void Patch()
        {
            try
            {
                QuickLogger.Message("Started patching - " + QuickLogger.GetAssemblyVersion());

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
