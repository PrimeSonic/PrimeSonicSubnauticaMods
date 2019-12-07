﻿namespace CyclopsAutoZapper
{
    using System.Reflection;
    using Common;
    using Harmony;

    public static class QPatch
    {
        public static void Patch()
        {
            QuickLogger.Info("Started patching " + QuickLogger.GetAssemblyVersion());

            var defenseSystem = new CyclopsAutoDefense();
            defenseSystem.Patch();

            var antiParasites = new CyclopsParasiteRemover();
            antiParasites.Patch();

            var defenseSystemMk2 = new CyclopsAutoDefenseMk2(defenseSystem);
            defenseSystemMk2.Patch();

            var harmony = HarmonyInstance.Create("com.cyclopsautozapper.psmod");
            harmony.PatchAll(Assembly.GetExecutingAssembly());

            QuickLogger.Info("Finished Patching");
        }
    }
}
