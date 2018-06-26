namespace UpgradedVehicles
{
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

        public void OnProtoSerialize(ProtobufSerializer serializer)
        {
            foreach (SeamothStorageContainer store in Storages)
            {
                store.OnProtoSerialize(serializer);
            }
        }
        
        public void OnProtoDeserialize(ProtobufSerializer serializer)
        {
            this.Init();
            foreach (SeamothStorageContainer store in Storages)
            {
                store.OnProtoDeserialize(serializer);
            }
        }

        public void OnProtoSerializeObjectTree(ProtobufSerializer serializer)
        {
            foreach (SeamothStorageContainer store in Storages)
            {
                store.OnProtoSerializeObjectTree(serializer);
            }
        }

        public void OnProtoDeserializeObjectTree(ProtobufSerializer serializer)
        {
            foreach (SeamothStorageContainer store in Storages)
            {
                store.OnProtoDeserializeObjectTree(serializer);
            }           
        }

        public ItemsContainer GetStorageInSlot(int slotID)
        {
            return Storages[slotID].container;
        }
    }
}
