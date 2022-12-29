namespace IonCubeGenerator
{
    using BepInEx;
    using Common;
    using HarmonyLib;
    using IonCubeGenerator.Buildable;
    using IonCubeGenerator.Configuration;    
    using System.Reflection;

    [BepInPlugin(GUID, MODNAME, VERSION)]
    [BepInDependency("com.ahk1221.smlhelper", BepInDependency.DependencyFlags.HardDependency)]
    public class Plugin: BaseUnityPlugin
    {
        #region[Declarations]
        private const string
            MODNAME = "Ion Cube Generator",
            AUTHOR = "PrimeSonic|FCStudios",
            GUID = "com.ioncubegenerator.psmod",
            VERSION = "1.0.0.0";
        #endregion

        static Plugin()
        {
            QuickLogger.Info("Loading config.json settings");
            ModConfiguration.Initialize();
        }

        public void Awake()
        {
            QuickLogger.Info("Started patching. Version: " + QuickLogger.GetAssemblyVersion());
            CubeGeneratorBuildable.PatchSMLHelper();
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), GUID);
            QuickLogger.Info("Finished patching");
        }
    }
}
