namespace MoreCyclopsUpgrades
{
    using System.Collections.Generic;

    internal class SortedCyclopsModules : SortedList<CyclopsModules, CyclopsModule>
    {
        public SortedCyclopsModules()
        {
        }

        public SortedCyclopsModules(int capacity) : base(capacity)
        {
        }

        internal void Add(CyclopsModule module) => Add(module.ModuleID, module);

    }
}
