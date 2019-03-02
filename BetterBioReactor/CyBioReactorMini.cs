namespace BetterBioReactor
{
    using System.Collections.Generic;
    using BetterBioReactor.SaveData;
    using Common;
    using ProtoBuf;
    using UnityEngine;

    [ProtoContract]
    internal class CyBioReactorMini : MonoBehaviour, IProtoEventListener, IProtoTreeEventListener
    {
        internal static readonly Dictionary<BaseBioReactor, CyBioReactorMini> LookupMiniReactor = new Dictionary<BaseBioReactor, CyBioReactorMini>();

        private const int TextDelayTicks = 60;
        private int textDelay = TextDelayTicks;
        private bool pdaIsOpen = false;
        private bool isLoadingSaveData = false;
        private float numberOfContainerSlots;
        private CyBioReactorSaveData SaveData;
        private List<BioEnergy> MaterialsProcessing;
        private List<BioEnergy> FullyConsumed;
        public int MaxPower = -1;

        public BaseBioReactor BioReactor { get; private set; }

        // Careful, this map only exists while the PDA screen is open
        public Dictionary<InventoryItem, uGUI_ItemIcon> InventoryMapping { get; private set; }

        public void Awake()
        {
            if (BioReactor is null)
            {
                BioReactor = GetComponentInParent<BaseBioReactor>();
            }

            if (SaveData is null)
            {
                string id = BioReactor.GetComponentInParent<PrefabIdentifier>().Id;
                SaveData = new CyBioReactorSaveData(id);
            }
        }

        public void ConnectToBioRector(BaseBioReactor bioReactor)
        {
            BioReactor = bioReactor;
            if (!LookupMiniReactor.ContainsKey(BioReactor))
                LookupMiniReactor.Add(BioReactor, this);

            (BioReactor.container as IItemsContainer).onAddItem += OnAddItem;
            BioReactor.container.Clear();

            int totalContainerSpaces = BioReactor.container.sizeX * BioReactor.container.sizeY;
            numberOfContainerSlots = totalContainerSpaces;
            MaterialsProcessing = new List<BioEnergy>(totalContainerSpaces);
            FullyConsumed = new List<BioEnergy>(totalContainerSpaces);
        }

        #region Player interaction

        // Completely replaces the original OnHover method in the BaseBioReactor
        internal void OnHover()
        {
            HandReticle main = HandReticle.main;
            int currentPower = Mathf.RoundToInt(BioReactor._powerSource.GetPower());
            string maxPowerText = $"{MaxPower}{(BioReactor.producingPower ? "+" : "")}";

            main.SetInteractText(Language.main.GetFormat("UseBaseBioReactor", currentPower, maxPowerText), "Tooltip_UseBaseBioReactor", false, true, true);
            main.SetIcon(HandReticle.IconType.Hand, 1f);
        }

        // Completely replaces the original OnUse method in the BaseBioReactor
        internal void OnPdaOpen(BaseBioReactorGeometry model)
        {
            pdaIsOpen = true;

            PDA pda = Player.main.GetPDA();
            Inventory.main.SetUsedStorage(BioReactor.container, false);
            pda.Open(PDATab.Inventory, model.storagePivot, new PDA.OnClose(OnPdaClose), 4f);
        }

        internal void OnPdaClose(PDA pda)
        {
            this.InventoryMapping = null;

            foreach (BioEnergy item in MaterialsProcessing)
            {
                item.DisplayText = null;
            }

            pdaIsOpen = false;

            (BioReactor.container as IItemsContainer).onAddItem -= OnAddItemLate;
        }

        private void OnAddItem(InventoryItem item)
        {
            if (isLoadingSaveData)
                return;

            if (BaseBioReactor.charge.TryGetValue(item.item.GetTechType(), out float bioEnergyValue) && bioEnergyValue > 0f)
            {
                var bioenergy = new BioEnergy(item.item, bioEnergyValue, bioEnergyValue)
                {
                    Size = item.width * item.height
                };

                MaterialsProcessing.Add(bioenergy);
            }
        }

        private void OnAddItemLate(InventoryItem item)
        {
            if (this.InventoryMapping is null)
                return;

            if (this.InventoryMapping.TryGetValue(item, out uGUI_ItemIcon icon))
            {
                BioEnergy bioEnergy = MaterialsProcessing.Find(m => m.Pickupable == item.item);

                if (bioEnergy is null)
                    return;

                bioEnergy.AddDisplayText(icon);
            }
        }

        #endregion

        // This method completely replaces the original ProducePower method in the BaseBioReactor
        internal float ProducePower(float chargePerSecond)
        {
            float powerProduced = 0f;

            if (chargePerSecond > 0f && // More than zero energy being produced per item per time delta
                MaterialsProcessing.Count > 0) // There should be materials in the reactor to process
            {
                float chargePerSecondPerItem = chargePerSecond / numberOfContainerSlots * 2;

                foreach (BioEnergy material in MaterialsProcessing)
                {
                    float availablePowerPerItem = Mathf.Min(material.RemainingEnergy, material.Size * chargePerSecondPerItem);

                    material.RemainingEnergy -= availablePowerPerItem;
                    powerProduced += availablePowerPerItem;

                    if (material.FullyConsumed)
                        FullyConsumed.Add(material);
                }
            }

            if (FullyConsumed.Count > 0)
            {
                foreach (BioEnergy material in FullyConsumed)
                {
                    MaterialsProcessing.Remove(material);
                    BioReactor.container.RemoveItem(material.Pickupable, true);
                    Destroy(material.Pickupable.gameObject);
                }

                FullyConsumed.Clear();
            }

            return powerProduced;
        }

        internal void UpdateDisplayText()
        {
            if (!pdaIsOpen)
                return;

            if (textDelay-- > 0)
                return; // Slow down the text update

            textDelay = TextDelayTicks;

            foreach (BioEnergy material in MaterialsProcessing)
                material.UpdateInventoryText();
        }

        #region Save data handling

        public void OnProtoSerialize(ProtobufSerializer serializer)
        {
            SaveData.ReactorBatterCharge = BioReactor._powerSource.power;
            SaveData.SaveMaterialsProcessing(MaterialsProcessing);

            SaveData.Save();
        }

        public void OnProtoDeserialize(ProtobufSerializer serializer)
        {
            QuickLogger.Debug("Looking for save data");

            isLoadingSaveData = true;

            BioReactor.container.Clear();

            isLoadingSaveData = false;
        }

        public void OnProtoSerializeObjectTree(ProtobufSerializer serializer)
        {
        }

        public void OnProtoDeserializeObjectTree(ProtobufSerializer serializer)
        {
            QuickLogger.Debug("Looking for save data for object tree");

            isLoadingSaveData = true;

            bool hasSaveData = SaveData.Load();

            BioReactor.container.Clear();

            if (hasSaveData)
            {
                QuickLogger.Debug("Save data found");

                BioReactor._powerSource.power = SaveData.ReactorBatterCharge;

                List<BioEnergy> materials = SaveData.GetMaterialsInProcessing();

                foreach (BioEnergy material in materials)
                {
                    InventoryItem inventoryItem = BioReactor.container.AddItem(material.Pickupable);
                    MaterialsProcessing.Add(material);
                    material.Size = inventoryItem.width * inventoryItem.height;
                }
            }

            isLoadingSaveData = false;
        }

        #endregion 

        public void ConnectToInventory(Dictionary<InventoryItem, uGUI_ItemIcon> lookup)
        {
            this.InventoryMapping = lookup;

            (BioReactor.container as IItemsContainer).onAddItem += OnAddItemLate;

            if (MaterialsProcessing.Count == 0)
                return;

            foreach (KeyValuePair<InventoryItem, uGUI_ItemIcon> pair in lookup)
            {
                InventoryItem item = pair.Key;
                uGUI_ItemIcon icon = pair.Value;

                BioEnergy bioEnergy = MaterialsProcessing.Find(m => m.Pickupable == item.item);

                if (bioEnergy is null)
                    continue;

                bioEnergy.AddDisplayText(icon);
            }
        }
    }
}