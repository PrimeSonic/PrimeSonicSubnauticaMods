namespace CyclopsSolarUpgrades.Management
{
    using MoreCyclopsUpgrades.API;

    internal partial class Solar : IAuxCyclopsManager
    {
        private const string ManagerName = "CySolarMgr";

        internal static TechType CyclopsSolarChargerID { get; set; }
        internal static TechType CyclopsSolarChargerMk2ID { get; set; }

        public string Name { get; } = ManagerName;

        public bool Initialize(SubRoot cyclops)
        {
            return
                CyclopsSolarChargerID != TechType.None &&
                CyclopsSolarChargerMk2ID != TechType.None;
        }

        internal static Solar CreateHandler(SubRoot cyclops)
        {
            return new Solar(cyclops);
        }

        internal static Solar FindHandler(SubRoot cyclops)
        {
            return MCUServices.Client.GetManager<Solar>(cyclops, ManagerName);
        }
    }
}
