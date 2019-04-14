namespace CyclopsNuclearReactor
{
    using Common;
    using Common.EasyMarkup;
    using SMLHelper.V2.Utility;
    using System.Collections.Generic;
    using System.IO;

    internal class CyNukeReactorSaveData : EmPropertyCollectionList<CyNukeRodSaveData>
    {
        private readonly string SaveDirectory = Path.Combine(SaveUtils.GetCurrentSaveDataDir(), "CyclopsNuclearReactor");

        private string SaveFile => Path.Combine(SaveDirectory, PreFabId + ".txt");

        private readonly string PreFabId;
        private readonly int MaxSlots;

        public CyNukeReactorSaveData(string prefabID, int maxSlots) : base(prefabID)
        {
            PreFabId = prefabID;
            MaxSlots = maxSlots;
        }

        public void ClearOldData()
        {
            this.Values.Clear();
        }

        public IEnumerable<CyNukeRodSaveData> Rods
        {
            get
            {
                if (this.Values.Count > MaxSlots)
                {
                    for (int i = 0; i < MaxSlots; i++)
                        yield return this.Values[i];
                }
                else
                {
                    foreach (CyNukeRodSaveData data in this.Values)
                        yield return data;
                }
            }
        }

        public void AddRodData(TechType techType, float remainingCharge)
        {
            if (this.Values.Count == MaxSlots)
                return;

            this.Values.Add(new CyNukeRodSaveData
            {
                TechTypeID = techType,
                RemainingCharge = remainingCharge
            });
        }

        public void AddEmptySlot()
        {
            if (this.Values.Count == MaxSlots)
                return;

            this.Values.Add(new CyNukeRodSaveData
            {
                TechTypeID = TechType.None,
                RemainingCharge = 0f
            });
        }

        public void SaveData()
        {
            this.Save(SaveDirectory, this.SaveFile);
        }

        public bool LoadData()
        {
            return this.Load(SaveDirectory, this.SaveFile);
        }

        internal override EmProperty Copy()
        {
            var cyNukeReactorSaveData = new CyNukeReactorSaveData(PreFabId, MaxSlots);

            foreach (CyNukeRodSaveData item in this.Values)
                cyNukeReactorSaveData.Values.Add(item.Copy() as CyNukeRodSaveData);

            return cyNukeReactorSaveData;
        }
    }
}
