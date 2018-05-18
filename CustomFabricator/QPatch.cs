namespace CustomFabricator
{
    using System.Reflection;
    using Harmony;

    public class QPatch
    {
        public static void Patch()
        {
            CustomFabricatorModule.Patch();

            //HarmonyInstance harmony = HarmonyInstance.Create("com.CustomFabricator.psmod");
            //harmony.PatchAll(Assembly.GetExecutingAssembly());
        }
    }
}
