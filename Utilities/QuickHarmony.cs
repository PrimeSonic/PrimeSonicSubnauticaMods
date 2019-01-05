namespace Common
{
    using System.Reflection;
    using Harmony;

    internal static class QuickHarmony
    {
        public static void PatchAssembly()
        {
            var assembly = Assembly.GetExecutingAssembly();
            string name = assembly.GetName().Name;

            var harmony = HarmonyInstance.Create($"com.{name.ToLower()}.psmod");
            
            harmony.PatchAll(assembly);
        }
    }
}
