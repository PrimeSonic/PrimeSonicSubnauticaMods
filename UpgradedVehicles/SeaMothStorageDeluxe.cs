namespace UpgradedVehicles
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using UnityEngine;

    public class SeaMothStorageDeluxe : MonoBehaviour
    {
        public readonly SeamothStorageContainer[] Storages = new SeamothStorageContainer[4];

        public SeaMothStorageDeluxe()
        {

        }

        public void Init()
        {
            GameObject prefab = Resources.Load<GameObject>("WorldEntities/Tools/SeamothStorageModule");

            var holder = new GameObject("StorageHolder");
            holder.transform.parent = transform;
            holder.transform.localPosition = Vector3.one;

            for (int i = 0; i < 4; i++)
            {
                var storage = GameObject.Instantiate(prefab);
                storage.transform.parent = holder.transform;
                storage.transform.localPosition = Vector3.one;

                Storages[i] = storage.GetComponent<SeamothStorageContainer>();
            }
        }

        public ItemsContainer GetStorageInSlot(int slotID)
        {
            return Storages[slotID].container;
        }
    }
}
