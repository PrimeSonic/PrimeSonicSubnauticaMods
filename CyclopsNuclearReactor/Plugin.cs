namespace CyclopsNuclearReactor
{
    using System.Reflection;
    using BepInEx;
    using Common;
    using HarmonyLib;
    using MoreCyclopsUpgrades.API;

    [BepInPlugin(GUID, MODNAME, VERSION)]
    [BepInDependency("com.ahk1221.smlhelper", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("com.morecyclopsupgrades.psmod", BepInDependency.DependencyFlags.HardDependency)]
    public class Plugin : BaseUnityPlugin
    {
        #region[Declarations]
        private const string
            MODNAME = "Cyclops Nuclear Reactor",
            AUTHOR = "PrimeSonic",
            GUID = "com.cyclopsnuclearreactor.psmod",
            VERSION = "1.0.0.0";
        #endregion

        public void Awake()
        {
            MCUServices.Logger.Info("Started patching. Version: " + QuickLogger.GetAssemblyVersion());

            CyNukReactorBuildable.PatchSMLHelper();
            CyNukeEnhancerMk1.PatchSMLHelper();
            CyNukeEnhancerMk2.PatchSMLHelper();

            MCUServices.Logger.Debug("Registering with MoreCyclopsUpgrades");

            MCUServices.Register.CyclopsCharger<CyNukeChargeManager>((SubRoot cyclops) =>
            {
                return new CyNukeChargeManager(cyclops);
            });

            MCUServices.Register.CyclopsUpgradeHandler((SubRoot cyclops) =>
            {
                return new CyNukeEnhancerHandler(cyclops);
            });

            MCUServices.Register.AuxCyclopsManager<CyNukeManager>((SubRoot cyclops) =>
            {
                return new CyNukeManager(cyclops);
            });

            var harmony = new Harmony(GUID);
            harmony.PatchAll(Assembly.GetExecutingAssembly());

            MCUServices.Logger.Info("Finished patching");
        }
    }
}
