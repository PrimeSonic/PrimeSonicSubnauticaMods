namespace AIOFabricator
{
    using System;
    using System.Reflection;
    using BepInEx;

    [BepInPlugin("com.Primesonic.AIOFab", "AIOFabricator", "0.0.1")]
    public class Main : BaseUnityPlugin
    {
        private static AiOFab aioFab;

        public void Awake()
        {
            Console.WriteLine("[AIOFabricator] Started patching v" + Assembly.GetExecutingAssembly().GetName().Version.ToString(3));         

            aioFab = new AiOFab();
            aioFab.Patch();

            Console.WriteLine("[AIOFabricator] Finished patching");
        }        
    }
}
