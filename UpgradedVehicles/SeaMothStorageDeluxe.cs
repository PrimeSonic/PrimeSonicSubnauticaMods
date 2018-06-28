namespace UpgradedVehicles
{
    using System;
    using UnityEngine;

    [Obsolete("Not to be used. Not compatible with saving.", true)]
    public class SeaMothStorageDeluxe : MonoBehaviour
    {
        public bool Initialized { get; internal set; } = false;

        public SeaMoth ParentSeamoth;

        public readonly SeamothStorageContainer[] Storages = new SeamothStorageContainer[4];

        public SeaMothStorageDeluxe()
        {

        }

        public void Init(SeaMoth parent)
        {
            ParentSeamoth = parent;
            Init();
        }

        public void Init()
        {
            //GameObject storagePrefab = Resources.Load<GameObject>("WorldEntities/Tools/SeamothStorageModule");

            if (Initialized)
                return;

            var holder = ParentSeamoth.modulesRoot;
            holder.transform.parent = this.transform;
            holder.transform.localPosition = Vector3.one;

            SeamothStorageContainer[] storages = holder.GetAllComponentsInChildren<SeamothStorageContainer>();
            Console.WriteLine($"[UpgradedVehicles] SeaMothStorageDeluxe : Awake Storages:Length {storages.Length}");

            // This is weird but each storage component shows up twice with this call
            for (int outerIndex = 0; outerIndex < 8; outerIndex += 2)
            {
                int index = outerIndex / 2;
                
                //var storage = GameObject.Instantiate(storagePrefab);
                storages[outerIndex].transform.parent = holder.transform;
                storages[outerIndex].transform.localPosition = Vector3.one;

                Storages[index] = storages[outerIndex];

                SeamothStorageInput seamothStorageInput = ParentSeamoth.storageInputs[index];
                seamothStorageInput.seamoth = ParentSeamoth;
                seamothStorageInput.SetEnabled(true);
                Storages[index].enabled = true;
            }

            Initialized = true;
            Console.WriteLine($"[UpgradedVehicles] SeaMothStorageDeluxe : Initialized");
        }
        
        public ItemsContainer GetStorageInSlot(int slotID)
        {
            return Storages[slotID].container;
        }
    }
}
