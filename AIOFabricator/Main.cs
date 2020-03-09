namespace AIOFabricator
{
    using System;
    using System.Reflection;
    using QModManager.API.ModLoading;

    [QModCore]
    public static class Main
    {
        private static AiOFab aioFab;

        [QModPatch]
        public static void Start()
        {
            Console.WriteLine("[AIOFabricator] Started patching v" + Assembly.GetExecutingAssembly().GetName().Version.ToString(3));         

            aioFab = new AiOFab();
            aioFab.Patch();

            Console.WriteLine("[AIOFabricator] Finished patching");
        }        
    }
}
