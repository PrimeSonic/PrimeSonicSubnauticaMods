namespace CyclopsNuclearUpgrades
{
    using System;
    using Common;

    public static class QPatch
    {

        public static void Patch()
        {
            try
            {
                var nuclearModule = new CyclopsNuclearModule();
                var depletedModule = new DepletedNuclearModule(nuclearModule);
                var nuclearFabricator = new NuclearFabricator(nuclearModule);

                nuclearModule.Patch();
                depletedModule.Patch();
                nuclearFabricator.Patch();
            }
            catch(Exception ex)
            {
                QuickLogger.Error(ex);
            }
        }
    }
}
