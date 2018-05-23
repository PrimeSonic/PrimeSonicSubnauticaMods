namespace VModFabricator
{
    using System.Reflection;
    using Harmony;

    public class QPatch
    {
        public static void Patch()
        {
            VModFabricatorModule.Patch();

            //HarmonyInstance harmony = HarmonyInstance.Create("com.VModFabricator.psmod");
            //harmony.PatchAll(Assembly.GetExecutingAssembly());
        }
    }
}
