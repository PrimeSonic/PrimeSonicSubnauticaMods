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

                var unlockConfig = new EmUnlockConfig();
                unlockConfig.Initialize();

                Craftable.ForceUnlockAtStart = unlockConfig.Value;

                if (Craftable.ForceUnlockAtStart)
                    QuickLogger.Message("ForceUnlockAtStart was enabled. New items will start unlocked.");
                else
                    QuickLogger.Message("New items set to normal unlock requirements.");

                var speedModule = new SpeedBooster();
                var powerCore = new VehiclePowerCore(speedModule);
                var mothMk2 = new SeaMothMk2(powerCore);
                var suitMk2 = new ExosuitMk2(powerCore);
                var mothMk3 = new SeaMothMk3(powerCore);

                speedModule.Patch();
                powerCore.Patch();
                mothMk2.Patch();
                suitMk2.Patch();
                mothMk3.Patch();

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
