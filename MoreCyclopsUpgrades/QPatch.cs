namespace MoreCyclopsUpgrades
{
    using System.Reflection;
    using Harmony;
    using UnityEngine;

    // QMods by qwiso https://github.com/Qwiso/QModManager
    public class QPatch
    {
        public static void Patch()
        {
            // Asset Bundles don't like being loaded more than once. So we load it only once and pass it around.
            var assetBundle = AssetBundle.LoadFromFile(@"./QMods/MoreCyclopsUpgrades/Assets/morecyclopsupgrades.assets");

            SolarCharger.Patch(assetBundle);
            SolarChargerMk2.Patch(assetBundle);
            NuclearCharger.Patch(assetBundle);
            PowerUpgradeMk2.Patch(assetBundle);
            PowerUpgradeMk3.Patch(assetBundle);

            HarmonyInstance harmony = HarmonyInstance.Create("com.morecyclopsupgrades.psmod");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }
    }
}
