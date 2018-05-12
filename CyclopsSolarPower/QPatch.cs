namespace CyclopsSolarPower
{
    using System.Reflection;
    using Harmony;

    // QMods by qwiso https://github.com/Qwiso/QModManager
    public class QPatch
    {
        public static void Patch()
        {
            HarmonyInstance harmony = HarmonyInstance.Create("com.CyclopsSolarPower.psmod");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }
    }
}
