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

        public CyNukeReactorSaveData(string prefabID, int maxSlots) : base("CNR")
        {
            PreFabId = prefabID;
            MaxSlots = maxSlots;
        }

        public void ClearOldData()
        {
            this.Values.Clear();
        }

        public IEnumerable<CyNukeRodSaveData> SlotData => this.Values;

        public void AddSlotData(IEnumerable<SlotData> slotDataCollection)
        {
            foreach (SlotData slotData in slotDataCollection)
            {
                this.Values.Add(new CyNukeRodSaveData(slotData));
            }
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
