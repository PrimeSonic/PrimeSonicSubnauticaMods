namespace MoreCyclopsUpgrades.Monobehaviors
{
    using UnityEngine;

    internal class BioEnergy
    {
        public bool FullyConsumed => RemainingEnergy <= 0f;

        public string EnergyString => $"{Mathf.FloorToInt(RemainingEnergy)}/{MaxEnergy}";

        public Pickupable Pickupable;
        public float RemainingEnergy;
        public readonly float MaxEnergy;

        public BioEnergy(Pickupable pickupable, float currentEnergy, float originalEnergy)
        {
            Pickupable = pickupable;
            RemainingEnergy = currentEnergy;
            MaxEnergy = originalEnergy;
        }

        internal BioEnergy(Pickupable pickupable, float energy)
        {
            Pickupable = pickupable;
            RemainingEnergy = energy;
            MaxEnergy = CyBioReactorMono.GetChargeValue(pickupable.GetTechType());
        }        
    }
}