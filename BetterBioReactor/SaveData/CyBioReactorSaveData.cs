namespace BetterBioReactor.SaveData
{
    using System.Collections.Generic;
    using System.IO;
    using Common;
    using EasyMarkup;
    using UnityEngine;

    internal class CyBioReactorSaveData : EmPropertyCollection
    {
        internal const float MaxPower = 500;
        private const string ReactorBatterChargeKey = "BRP";
        private const string MaterialsKey = "MAT";
        private const string MainKey = "BBR";
        private readonly string ID;

        private EmPropertyCollectionList<EmModuleSaveData> _materials;

        private static ICollection<EmProperty> GetDefinitions => new List<EmProperty>()
        {
            new EmProperty<float>(ReactorBatterChargeKey, 0){ Optional = true },
            new EmPropertyCollectionList<EmModuleSaveData>(MaterialsKey)
        };

        public CyBioReactorSaveData(ICollection<EmProperty> definitions) : base(MainKey, definitions)
        {
            _materials = (EmPropertyCollectionList<EmModuleSaveData>)Properties[MaterialsKey];
        }

        public CyBioReactorSaveData(string preFabID) : this(GetDefinitions)
        {
            ID = preFabID;
        }

        public void SaveMaterialsProcessing(IEnumerable<BioEnergy> materialsInProcessor)
        {
            _materials.Values.Clear();

            foreach (BioEnergy item in materialsInProcessor)
            {
                _materials.Add(new EmModuleSaveData
                {
                    ItemID = (int)item.Pickupable.GetTechType(),
                    RemainingCharge = item.RemainingEnergy
                });
            }

            this.Save(this.SaveDirectory, this.SaveFile);
        }

        public IEnumerable<BioEnergy> GetMaterialsInProcessing()
        {
            foreach (EmModuleSaveData savedItem in _materials.Values)
            {
                var techTypeID = (TechType)savedItem.ItemID;

                GameObject prefab = CraftData.GetPrefabForTechType(techTypeID);

                if (prefab == null)
                {
                    QuickLogger.Warning($"Unable to find prefab for TechType '{techTypeID}'");
                    continue;
                }

                var gameObject = GameObject.Instantiate(prefab);

                Pickupable pickupable = gameObject.GetComponent<Pickupable>();

                if (pickupable == null)
                {
                    QuickLogger.Warning($"Unable to find Pickupable component for item '{techTypeID.AsString()}'");
                    continue;
                }

                pickupable.Pickup(false);

                yield return new BioEnergy(pickupable, savedItem.RemainingCharge);
            }
        }

        private string SaveDirectory => Path.Combine(SaveLoadManager.GetTemporarySavePath(), MainKey);
        private string SaveFile => Path.Combine(this.SaveDirectory, ID + ".txt");

        public bool LoadSaveFile()
        {
            return this.Load(this.SaveDirectory, this.SaveFile);
        }

        internal override EmProperty Copy()
        {
            return new CyBioReactorSaveData(this.CopyDefinitions);
        }
    }
}
