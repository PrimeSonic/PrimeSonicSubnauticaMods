namespace MoreCyclopsUpgrades.Caching
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    internal class BioEnergyCollection : IEnumerable<BioEnergy>, ICollection<BioEnergy>
    {
        private readonly List<BioEnergy> collection = new List<BioEnergy>();
        private readonly List<BioEnergy> forRemoval = new List<BioEnergy>();

        public int Count => collection.Count;
        public int SpacesOccupied { get; private set; } = 0;

        public bool IsReadOnly { get; }

        public BioEnergy Find(Pickupable pickupable)
        {
            return collection.Find(material => material.Pickupable == pickupable);
        }


        public BioEnergy GetCandidateForRemoval()
        {
            if (collection.Count == 0)
                return null;

            List<BioEnergy> largeCandidates = collection.FindAll(c => c.Size > 1);

            if (largeCandidates != null && largeCandidates.Count > 0)
            {
                return GetMaterialWithLeastEnergy(largeCandidates);
            }
            else // candidates Count == 0
            {
                return GetMaterialWithLeastEnergy(collection);
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

        public void Add(BioEnergy material)
        {
            collection.Add(material);
            this.SpacesOccupied += material.Size;
        }

        public void StageForRemoval(BioEnergy material)
        {
            forRemoval.Add(material);
        }

        public bool Remove(BioEnergy material)
        {
            bool removed = collection.Remove(material);

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
                foreach (BioEnergy material in forRemoval)
                {
                    Remove(material, container);
                }

                forRemoval.Clear();
            }
        }

        public IEnumerator<BioEnergy> GetEnumerator()
        {
            return collection.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return collection.GetEnumerator();
        }

        public void Clear()
        {
            collection.Clear();
            this.SpacesOccupied = 0;
        }

        public bool Contains(BioEnergy material)
        {
            return collection.Contains(material);
        }

        public void CopyTo(BioEnergy[] array, int arrayIndex)
        {
            collection.CopyTo(array, arrayIndex);
        }
    }
}
