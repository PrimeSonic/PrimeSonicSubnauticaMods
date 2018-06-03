namespace MoreCyclopsUpgrades
{
    using System.Reflection;
    using Harmony;

    public class QPatch
    {
        public static void Patch()
        {
            SolarCharger.Patch();

            NuclearCharger.Patch();

            HarmonyInstance harmony = HarmonyInstance.Create("com.MoreCyclopsUpgrades.psmod");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }
    }
}
