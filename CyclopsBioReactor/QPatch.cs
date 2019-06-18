namespace CyclopsBioReactor
{
    using System;
    using System.Reflection;
    using Common;
    using CyclopsBioReactor.Items;
    using Harmony;

    public static class QPatch
    {
        public static void Patch()
        {
            try
            {
                QuickLogger.Info("Started patching " + QuickLogger.GetAssemblyVersion());

                var booster = new BioReactorBooster();
                booster.Patch();

                var reactor = new CyBioReactor(booster);
                reactor.Patch();

                var harmony = HarmonyInstance.Create("com.morecyclopsupgrades.psmod");
                harmony.PatchAll(Assembly.GetExecutingAssembly());

                QuickLogger.Info("Finished Patching");
            }
            catch (Exception ex)
            {
                QuickLogger.Error(ex);
            }
        }
    }
}
