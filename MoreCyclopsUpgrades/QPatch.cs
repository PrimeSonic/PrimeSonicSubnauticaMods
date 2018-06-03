namespace MoreCyclopsUpgrades
{
    using System.Reflection;
    using Harmony;

    // QMods by qwiso https://github.com/Qwiso/QModManager
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
