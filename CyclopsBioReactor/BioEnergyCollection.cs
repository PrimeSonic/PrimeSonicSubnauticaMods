namespace CyclopsBioReactor
{
    using System.Collections.Generic;
    using UnityEngine;

    internal class BioEnergyCollection : List<BioEnergy>
    {
        private readonly List<BioEnergy> forRemoval = new List<BioEnergy>();
        public int SpacesOccupied { get; private set; } = 0;

        public bool IsReadOnly => false;

        public BioEnergy Find(Pickupable pickupable)
        {
            return Find(material => material.Pickupable == pickupable);
        }

        public BioEnergy GetCandidateForRemoval()
        {
            if (this.Count == 0)
                return null;

            List<BioEnergy> largeCandidates = FindAll(c => c.Size > 1);

            if (largeCandidates != null && largeCandidates.Count > 0)
            {
                return GetMaterialWithLeastEnergy(largeCandidates);
            }
            else // candidates Count == 0
            {
                return GetMaterialWithLeastEnergy(this);
            }
        }

        private BioEnergy GetMaterialWithLeastEnergy(IList<BioEnergy> collectionToCheck)
        {
            BioEnergy candidate = collectionToCheck[0];

            for (int i = 1; i < collectionToCheck.Count; i++)
            {
                if (collectionToCheck[i].RemainingEnergy < candidate.RemainingEnergy)
                    candidate = collectionToCheck[i];
            }

            return candidate;
        }

        public new void Add(BioEnergy material)
        {
            base.Add(material);
            this.SpacesOccupied += material.Size;
        }

        public void Add(BioEnergy material, ItemsContainer container)
        {
            InventoryItem inventoryItem = container.AddItem(material.Pickupable);
            material.Size = inventoryItem.width * inventoryItem.height;
            base.Add(material);
            this.SpacesOccupied += material.Size;
        }

        public void StageForRemoval(BioEnergy material)
        {
            forRemoval.Add(material);
        }

        public new bool Remove(BioEnergy material)
        {
            bool removed = base.Remove(material);

            if (removed)
                this.SpacesOccupied -= material.Size;

            return removed;
        }

        public bool Remove(BioEnergy material, ItemsContainer container)
        {
            bool removed = Remove(material);

            if (removed)
            {
                container.RemoveItem(material.Pickupable, true);
                Object.Destroy(material.Pickupable.gameObject);
            }

            return removed;
        }

        public void ClearAllStagedForRemoval(ItemsContainer container)
        {
            if (forRemoval.Count > 0)
            {
                for (int i = 0; i < forRemoval.Count; i++)
                    Remove(forRemoval[i], container);

                forRemoval.Clear();
            }
        }

        public new void Clear()
        {
            base.Clear();
            this.SpacesOccupied = 0;
        }
    }
}
