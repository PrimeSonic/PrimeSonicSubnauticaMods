namespace MoreCyclopsUpgrades.Monobehaviors
{
    using UnityEngine;

    internal class BioEnergy
    {
        public bool FullyConsumed => Mathf.Approximately(Energy, 0f);

        public Pickupable Item;
        public float Energy;
        public readonly float MaxEnergy;

        public BioEnergy(Pickupable item, float currentEnergy, float originalEnergy)
        {
            Item = item;
            Energy = currentEnergy;
            MaxEnergy = originalEnergy;
        }

        internal BioEnergy(Pickupable item, float energy)
        {
            Item = item;
            Energy = energy;
            MaxEnergy = CyBioReactorMono.BioReactorCharges[item.GetTechType()];
        }
    }
}