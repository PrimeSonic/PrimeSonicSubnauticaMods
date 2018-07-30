namespace DataBoxScannerFix
{
    using System;
    using System.Reflection;
    using Common;
    using Harmony;

    public static class QPatch
    {

        public static void Patch()
        {
            try
            {
                HarmonyInstance harmony = HarmonyInstance.Create("com.dataBoxscannerfix.psmod");
                harmony.PatchAll(Assembly.GetExecutingAssembly());
            }
            catch(Exception e)
            {
                QuickLogger.Error(e.ToString());
            }
        }

    }
}
