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

            try
            {
                Console.WriteLine($"[UpgradedVehicles] Start patching");

                var speedModule = new SpeedBooster();
                speedModule.Patch();

                var powerCore = new VehiclePowerCore(speedModule.TechType);
                powerCore.Patch();

                var mothMk2 = new SeaMothMk2(powerCore.TechType);
                mothMk2.Patch();

                var suitMk2 = new ExosuitMk2(powerCore.TechType);
                suitMk2.Patch();

                if (TechTypeHandler.TryGetModdedTechType("SeamothHullModule4", out TechType seamothDepthMk4) &&
                    TechTypeHandler.TryGetModdedTechType("SeamothHullModule5", out TechType seamothDepthMk5))
                {
                    var mothMk3 = new SeaMothMk3(powerCore.TechType, seamothDepthMk4, seamothDepthMk5);
                    mothMk3.Patch();
                }

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
