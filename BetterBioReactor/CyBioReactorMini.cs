namespace BetterBioReactor
{
    using BetterBioReactor.SaveData;
    using Common;
    using ProtoBuf;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;

    // The immediate access to the internals of the BaseBioReactor (without the use of Reflection) was made possible thanks to the AssemblyPublicizer
    // https://github.com/CabbageCrow/AssemblyPublicizer
    [ProtoContract]
    internal class CyBioReactorMini
    {
        private static readonly Dictionary<BaseBioReactor, CyBioReactorMini> LookupMiniReactor = new Dictionary<BaseBioReactor, CyBioReactorMini>();
        internal static CyBioReactorMini GetMiniReactor(ref BaseBioReactor bioReactor)
        {
            if (LookupMiniReactor.TryGetValue(bioReactor, out CyBioReactorMini existingBioMini))
                return existingBioMini;

            var createdBioMini = new CyBioReactorMini(ref bioReactor);
            LookupMiniReactor.Add(bioReactor, createdBioMini);

            QuickLogger.Debug("CyBioReactorMini Connected");

            return createdBioMini;
        }

        internal static bool PdaIsOpen = false;
        internal static CyBioReactorMini OpenInPda = null;

        private const float TextDelayInterval = 2f;
        private float textDelay = TextDelayInterval;

        private bool isLoadingSaveData = false;
        private float numberOfContainerSlots = 12;
        private CyBioReactorSaveData SaveData;
        private readonly List<BioEnergy> MaterialsProcessing = new List<BioEnergy>();
        private readonly List<BioEnergy> FullyConsumed = new List<BioEnergy>();

        public int MaxPower = -1;
        public string MaxPowerText => $"{MaxPower}{(BioReactor.producingPower ? "+" : "")}";
        public int CurrentPower => Mathf.RoundToInt(BioReactor._powerSource.GetPower());

        public readonly BaseBioReactor BioReactor;
        private BaseBioReactorGeometry fallbackGeometry = null;

        // Careful, this map only exists while the PDA screen is open
        public Dictionary<InventoryItem, uGUI_ItemIcon> InventoryMapping { get; private set; }

        public CyBioReactorMini(ref BaseBioReactor bioReactor)
        {
            BioReactor = bioReactor;

            PrefabIdentifier prefabIdentifier = bioReactor.GetComponentInParent<PrefabIdentifier>() ?? bioReactor.GetComponent<PrefabIdentifier>();

            string id = prefabIdentifier.Id;

            QuickLogger.Debug($"CyBioReactorMini PrefabIdentifier: {id}");

            SaveData = new CyBioReactorSaveData(id);
        }

        public void Start()
        {
            QuickLogger.Debug("CyBioReactorMini starting");

            (BioReactor.container as IItemsContainer).onAddItem += OnAddItem;
            BioReactor.container.Clear();

            MaxPower = Mathf.RoundToInt(BioReactor._powerSource.GetMaxPower());

            int totalContainerSpaces = BioReactor.container.sizeX * BioReactor.container.sizeY;
            numberOfContainerSlots = totalContainerSpaces;

            RestoreItemsFromSaveData();

            QuickLogger.Debug("CyBioReactorMini started");
        }

        #region Player interaction

        // Completely replaces the original OnHover method in the BaseBioReactor
        internal void OnHover()
        {
            HandReticle main = HandReticle.main;

            string text1 = Language.main.GetFormat("UseBaseBioReactor", this.CurrentPower, this.MaxPowerText);
#if SUBNAUTICA
            main.SetInteractText(text1, "Tooltip_UseBaseBioReactor", false, true, HandReticle.Hand.Right);
#elif BELOWZERO
            main.SetText(HandReticle.TextType.Hand, text1, false, GameInput.Button.LeftHand);
            main.SetText(HandReticle.TextType.HandSubscript, "Tooltip_UseBaseBioReactor", true);
            
#endif
            main.SetIcon(HandReticle.IconType.Hand, 1f);
        }

        // Completely replaces the original OnUse method in the BaseBioReactor
        internal void OnPdaOpen(BaseBioReactorGeometry model)
        {
            fallbackGeometry = model;

            PdaIsOpen = true;
            OpenInPda = this;

            PDA pda = Player.main.GetPDA();
            Inventory.main.SetUsedStorage(BioReactor.container, false);
#if SUBNAUTICA
            pda.Open(PDATab.Inventory, model.storagePivot, new PDA.OnClose(OnPdaClose), 4f);
#elif BELOWZERO
            pda.Open(PDATab.Inventory, model.storagePivot, new PDA.OnClose(OnPdaClose));
#endif
        }

        internal void OnPdaClose(PDA pda)
        {
            this.InventoryMapping = null;

            foreach (BioEnergy item in MaterialsProcessing)
            {
                item.DisplayText = null;
            }

            PdaIsOpen = false;
            OpenInPda = null;

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
                    BioReactor.container.RemoveItem(material.Pickupable, true);
                    GameObject.Destroy(material.Pickupable.gameObject);
                }

                FullyConsumed.Clear();
            }

            return powerProduced;
        }

        internal void UpdateDisplayText()
        {
            if (Time.time < textDelay)
                return; // Slow down the text update

            textDelay = Time.time + TextDelayInterval;

            if (MaterialsProcessing.Count > 0 || this.CurrentPower > 0)
            {
#if SUBNAUTICA
                Text displayText = (BioReactor.GetModel() ?? fallbackGeometry)?.text;
#elif BELOWZERO
                TMPro.TextMeshProUGUI displayText = (BioReactor.GetModel() ?? fallbackGeometry)?.text;
#endif
                if (displayText != null)
                {
                    string maxPowerText = this.MaxPowerText;
                    string currentPowerString = this.CurrentPower.ToString().PadLeft(maxPowerText.Length, ' ');
                    displayText.text = $"<color=#00ff00>{Language.main.Get("BaseBioReactorActive")}\n{currentPowerString}/{maxPowerText}</color>";
                }
            }

            if (!PdaIsOpen)
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

            BioReactor.container.Clear();

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
                InventoryItem inventoryItem = BioReactor.container.AddItem(material.Pickupable);
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