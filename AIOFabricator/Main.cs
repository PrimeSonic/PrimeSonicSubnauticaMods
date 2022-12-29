namespace AIOFabricator
{
    using System;
    using System.Reflection;
    using BepInEx;

    [BepInPlugin(GUID, MODNAME, VERSION)]
    [BepInDependency("com.ahk1221.smlhelper", BepInDependency.DependencyFlags.HardDependency)]
    public class Plugin : BaseUnityPlugin
    {
        #region[Declarations]
        private const string
            MODNAME = "AIOFabricator",
            AUTHOR = "PrimeSonic",
            GUID = "com.aiofabricator.psmod",
            VERSION = "1.0.0.0";
        #endregion

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
