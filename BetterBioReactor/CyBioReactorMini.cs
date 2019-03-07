namespace BetterBioReactor
{
    using BetterBioReactor.SaveData;
    using Common;
    using ProtoBuf;
    using System.Collections.Generic;
    using UnityEngine;

    // The immediate access to the internals of the BaseBioReactor (without the use of Reflection) was made possible thanks to the AssemblyPublicizer
    // https://github.com/CabbageCrow/AssemblyPublicizer
    [ProtoContract]
    internal class CyBioReactorMini
    {
        private static readonly Dictionary<BaseBioReactor, CyBioReactorMini> LookupMiniReactor = new Dictionary<BaseBioReactor, CyBioReactorMini>();
        internal static CyBioReactorMini GetMiniReactor(BaseBioReactor bioReactor)
        {
            if (LookupMiniReactor.TryGetValue(bioReactor, out CyBioReactorMini existingBioMini))
                return existingBioMini;

            QuickLogger.Debug("CyBioReactorMini Connected");

            var createdBioMini = new CyBioReactorMini(bioReactor);
            LookupMiniReactor.Add(bioReactor, createdBioMini);

            return createdBioMini;
        }

        private const int TextDelayTicks = 60;
        private int textDelay = TextDelayTicks;
        private bool pdaIsOpen = false;
        private bool isLoadingSaveData = false;
        private float numberOfContainerSlots = 12;
        private CyBioReactorSaveData SaveData;
        private readonly List<BioEnergy> MaterialsProcessing = new List<BioEnergy>();
        private readonly List<BioEnergy> FullyConsumed = new List<BioEnergy>();

        public int MaxPower = -1;
        public string MaxPowerText => $"{MaxPower}{(this.BioReactor.producingPower ? "+" : "")}";
        public int CurrentPower => Mathf.RoundToInt(this.BioReactor._powerSource.GetPower());

        public BaseBioReactor BioReactor { get; private set; }

        // Careful, this map only exists while the PDA screen is open
        public Dictionary<InventoryItem, uGUI_ItemIcon> InventoryMapping { get; private set; }

        public CyBioReactorMini(BaseBioReactor bioReactor)
        {
            this.BioReactor = bioReactor;

            string id = this.BioReactor.GetComponentInParent<PrefabIdentifier>().Id;
            SaveData = new CyBioReactorSaveData(id);
        }

        public void UpdateInternals()
        {
            (this.BioReactor.container as IItemsContainer).onAddItem += OnAddItem;
            this.BioReactor.container.Clear();

            MaxPower = Mathf.RoundToInt(this.BioReactor._powerSource.GetMaxPower());

            int totalContainerSpaces = this.BioReactor.container.sizeX * this.BioReactor.container.sizeY;
            numberOfContainerSlots = totalContainerSpaces;

            RestoreItemsFromSaveData();
        }

        #region Player interaction

        // Completely replaces the original OnHover method in the BaseBioReactor
        internal void OnHover()
        {
            HandReticle main = HandReticle.main;

            // All this is getting updated in Unity 2018
            main.SetInteractText(Language.main.GetFormat("UseBaseBioReactor", this.CurrentPower, this.MaxPowerText), "Tooltip_UseBaseBioReactor", false, true, true);
            main.SetIcon(HandReticle.IconType.Hand, 1f);
        }

        // Completely replaces the original OnUse method in the BaseBioReactor
        internal void OnPdaOpen(BaseBioReactorGeometry model)
        {
            pdaIsOpen = true;

            PDA pda = Player.main.GetPDA();
            Inventory.main.SetUsedStorage(this.BioReactor.container, false);
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

            (this.BioReactor.container as IItemsContainer).onAddItem -= OnAddItemLate;
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
            else
            {
                GameObject.Destroy(item.item);
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
                    this.BioReactor.container.RemoveItem(material.Pickupable, true);
                    GameObject.Destroy(material.Pickupable.gameObject);
                }

                FullyConsumed.Clear();
            }

            return powerProduced;
        }

        internal void UpdateDisplayText()
        {
            if (textDelay-- > 0)
                return; // Slow down the text update

            textDelay = TextDelayTicks;

            if (MaterialsProcessing.Count > 0 || this.CurrentPower > 0)
            {
                string maxPowerText = this.MaxPowerText;
                string currentPowerString = this.CurrentPower.ToString().PadLeft(maxPowerText.Length, ' ');

                BaseBioReactorGeometry baseBioReactorGeometry = this.BioReactor.GetModel();
                baseBioReactorGeometry.text.text = $"<color=#00ff00>{Language.main.Get("BaseBioReactorActive")}\n{currentPowerString}/{maxPowerText}</color>";
            }

            if (!pdaIsOpen)
                return;

            foreach (BioEnergy material in MaterialsProcessing)
                material.UpdateInventoryText();
        }

        #region Save data handling

        public void OnProtoSerialize(ProtobufSerializer serializer)
        {
            SaveData.SaveMaterialsProcessing(MaterialsProcessing);
        }

        public void OnProtoDeserialize(ProtobufSerializer serializer)
        {
            QuickLogger.Debug("Preventing original RestoreItems call");
        }

        public void OnProtoSerializeObjectTree(ProtobufSerializer serializer)
        {
        }

        public void OnProtoDeserializeObjectTree(ProtobufSerializer serializer)
        {
            QuickLogger.Debug("Looking for save data for object tree");

            this.BioReactor.container.Clear();

            if (SaveData.LoadSaveFile())
            {
                isLoadingSaveData = true;
                QuickLogger.Debug("Save data found");
            }
        }

        private void RestoreItemsFromSaveData()
        {
            if (!isLoadingSaveData || SaveData is null)
                return;

            foreach (BioEnergy material in SaveData.GetMaterialsInProcessing())
            {
                InventoryItem inventoryItem = this.BioReactor.container.AddItem(material.Pickupable);
                MaterialsProcessing.Add(material);
                material.Size = inventoryItem.width * inventoryItem.height;
            }

            QuickLogger.Debug("Original items restored");

            isLoadingSaveData = false;
        }

        #endregion

        public void ConnectToInventory(Dictionary<InventoryItem, uGUI_ItemIcon> lookup)
        {
            this.InventoryMapping = lookup;

            (this.BioReactor.container as IItemsContainer).onAddItem += OnAddItemLate;

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