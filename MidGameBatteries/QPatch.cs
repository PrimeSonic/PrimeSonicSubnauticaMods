namespace MidGameBatteries
{
    using System;
    using System.Reflection;
    using Common;
    using Harmony;
    using MidGameBatteries.Craftables;

    public static class QPatch
    {
        public static void Patch()
        {
            QuickLogger.Message("Start patching. Version: " + QuickLogger.GetAssemblyVersion());

            try
            {
                DeepBatteryCellBase.PatchAll();

                var harmony = HarmonyInstance.Create("com.midgamebatteries.psmod");
                harmony.PatchAll(Assembly.GetExecutingAssembly());

                QuickLogger.Message("Finished patching");
            }
            catch (Exception ex)
            {
                QuickLogger.Error(ex);
            }
        }
    }
}
