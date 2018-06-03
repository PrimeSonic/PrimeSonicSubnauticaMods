namespace MoreCyclopsUpgrades
{
    public class QPatch
    {
        public static void Patch()
        {
            SolarCharger.Patch();

            NuclearCharger.Patch();
        }
    }
}
