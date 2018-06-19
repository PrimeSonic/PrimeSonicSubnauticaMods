namespace UpgradedVehicles
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using UnityEngine;

    public class SeaMothStorageDeluxe : MonoBehaviour
    {
        public SeamothStorageContainer[] Storages;

        public SeaMothStorageDeluxe()
        {
            
        }

        void Start()
        {
            var holder = new GameObject("StorageHolder");
            holder.transform.parent = transform;
            holder.transform.localPosition = Vector3.one;

            for (int i = 0; i < 4; i++)
            {
                var storage = new GameObject("Storage");
                storage.transform.parent = holder.transform;
                storage.transform.localPosition = Vector3.one;

                storage.AddComponent<SeamothStorageContainer>();
            }
        }

        public ItemsContainer GetStorageInSlot(int slotID)
        {
            return Storages[slotID].container;
        }
    }
}
