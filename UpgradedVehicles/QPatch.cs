namespace UpgradedVehicles
{
    using System;
    using System.Reflection;
    using Harmony;

    public class QPatch
    {
        public static void Patch()
        {

            try
            {
                Console.WriteLine($"[UpgradedVehicles] Start patching");

                var speedModule = new SpeedBooster();
                speedModule.Patch();

                var powerCore = new VehiclePowerCore(speedModule);
                powerCore.Patch();

                var mothMk2 = new SeaMothMk2(powerCore);
                mothMk2.Patch();

                var suitMk2 = new ExosuitMk2(powerCore);
                suitMk2.Patch();

                var mothMk3 = new SeaMothMk3(powerCore);
                mothMk3.Patch();

                HarmonyInstance harmony = HarmonyInstance.Create("com.upgradedvehicles.psmod");
                harmony.PatchAll(Assembly.GetExecutingAssembly());

                Console.WriteLine($"[UpgradedVehicles] Finish patching");
            }
            catch (Exception ex)
            {
                Console.WriteLine("[UpgradedVehicles] EXCEPTION on Patch: " + ex.ToString());
            }

        }
    }
}
