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
                QuickLogger.Message("Started patching");

                var speedBoost = Craftable.AddForPatching(new SpeedBooster());
                var vpowerCore = Craftable.AddForPatching(new VehiclePowerCore(speedBoost));
                var seamothMk2 = Craftable.AddForPatching(new SeaMothMk2(vpowerCore));
                var exosuitMk2 = Craftable.AddForPatching(new ExosuitMk2(vpowerCore));
                var seamothMk3 = Craftable.AddForPatching(new SeaMothMk3(vpowerCore));

                Craftable.PatchAll();
                
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
