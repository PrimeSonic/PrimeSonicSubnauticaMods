namespace MoreCyclopsUpgrades.Caching
{
    using System.Collections.Generic;

    internal class ComponentCache
    {
        private static readonly List<ComponentCache> CyclopsCache = new List<ComponentCache>(3);
        private static readonly List<SubRoot> QuickCyclopsCache = new List<SubRoot>(3);

        internal readonly SubRoot Cyclops;
        internal readonly UpgradeManager UpgradeManager;
        internal readonly PowerManager PowerManager;
        internal readonly CrushDamage CrushDamage;

        public ComponentCache(SubRoot cyclops, UpgradeManager upgradeManager, PowerManager powerManager, CrushDamage crushDamage)
        {
            Cyclops = cyclops;
            UpgradeManager = upgradeManager;
            PowerManager = powerManager;
            CrushDamage = crushDamage;
        }

        internal static void CacheComponents(SubRoot cyclops, UpgradeManager upgradeManager, PowerManager powerManager, CrushDamage crushDamage)
        {
            if (CyclopsCache.Count == 0 ||
                CyclopsCache.Find(c => ReferenceEquals(c.Cyclops, cyclops)) != null)
            {
                CyclopsCache.Add(new ComponentCache(cyclops, upgradeManager, powerManager, crushDamage));
                QuickCyclopsCache.Add(cyclops);
            }
        }

        internal static ComponentCache Find(SubRoot cyclops) => CyclopsCache.Find(c => ReferenceEquals(c.Cyclops, cyclops));

        internal static bool IsCached(SubRoot cyclops) => QuickCyclopsCache.Contains(cyclops);
    }
}
