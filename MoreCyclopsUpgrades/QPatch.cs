namespace MoreCyclopsUpgrades
{
    using System.Reflection;
    using Harmony;
    using UnityEngine;

    // QMods by qwiso https://github.com/Qwiso/QModManager
    public class QPatch
    {
        private const string ModName = "MoreCyclopsUpgrades";

        public static void Patch()
        {
            var assetBundle = AssetBundle.LoadFromFile($"./QMods/{ModName}/Assets/{ModName.ToLower()}.assets");

            SolarCharger.Patch(assetBundle);

            NuclearCharger.Patch(assetBundle);

            HarmonyInstance harmony = HarmonyInstance.Create($"com.{ModName}.psmod");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }
    }
}
