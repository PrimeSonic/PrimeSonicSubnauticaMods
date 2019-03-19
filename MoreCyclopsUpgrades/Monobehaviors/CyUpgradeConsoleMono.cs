namespace MoreCyclopsUpgrades.Monobehaviors
{
    using System;
    using System.Reflection;
    using Common;
    using Modules;
    using MoreCyclopsUpgrades.Managers;
    using ProtoBuf;
    using SaveData;
    using UnityEngine;
    using UnityEngine.UI;

    [ProtoContract]
    public class CyUpgradeConsoleMono : HandTarget, IHandTarget, IProtoEventListener, IProtoTreeEventListener
    {
        // This will be set externally
        public SubRoot ParentCyclops { get; private set; }

        internal CyclopsManager Manager { get; private set; }

        public Equipment Modules { get; private set; }

        private Constructable Buildable = null;

        public override void Awake()
        {
            base.Awake();

            if (Buildable == null)
            {
                Buildable = GetComponentInChildren<Constructable>();
            }

            if (SaveData == null)
            {
                string id = GetComponentInParent<PrefabIdentifier>().Id;
                SaveData = new AuxUpgradeConsoleSaveData(id);
            }

            if (this.Modules == null)
            {
                InitializeModules();
            }
        }

        private void Start()
        {
            SubRoot cyclops = GetComponentInParent<SubRoot>();

            if (cyclops is null)
            {
                QuickLogger.Debug("CyUpgradeConsoleMono: Could not find Cyclops during Start. Attempting external syncronize.");
                CyclopsManager.SyncUpgradeConsoles();
            }
            else
            {
                QuickLogger.Debug("CyUpgradeConsoleMono: Parent cyclops found!");
                ConnectToCyclops(cyclops);
            }
        }

        private void InitializeModules()
        {
            if (ModulesRoot == null)
            {
                var equipmentRoot = new GameObject("EquipmentRoot");
                equipmentRoot.transform.SetParent(this.transform, false);
                ModulesRoot = equipmentRoot.AddComponent<ChildObjectIdentifier>();
            }

            this.Modules = new Equipment(base.gameObject, ModulesRoot.transform);
            this.Modules.SetLabel("CyclopsUpgradesStorageLabel");
            UpdateVisuals();

            Type equipmentType = typeof(Equipment);
            EventInfo onEquipInfo = equipmentType.GetEvent("onEquip", BindingFlags.Public | BindingFlags.Instance);
            EventInfo onUnequipInfo = equipmentType.GetEvent("onUnequip", BindingFlags.Public | BindingFlags.Instance);

            Type cyConsoleType = typeof(CyUpgradeConsoleMono);
            MethodInfo myOnEquipMethod = cyConsoleType.GetMethod("OnEquip", BindingFlags.Instance | BindingFlags.NonPublic);
            MethodInfo myOnUnequipMethod = cyConsoleType.GetMethod("OnUnequip", BindingFlags.Instance | BindingFlags.NonPublic);

            var onEquipDelegate = Delegate.CreateDelegate(typeof(Equipment.OnEquip), this, myOnEquipMethod);
            var onUnequipDelegate = Delegate.CreateDelegate(typeof(Equipment.OnUnequip), this, myOnUnequipMethod);

            onEquipInfo.AddEventHandler(this.Modules, onEquipDelegate);
            onUnequipInfo.AddEventHandler(this.Modules, onUnequipDelegate);

            UnlockDefaultModuleSlots();
        }

        private void AddModuleSpriteHandlers()
        {
            const float topRowY = 1.15f;//-0.109f;
            const float botRowY = 1.075f;//-0.239f;

            const float leftColX = 0.15f;//0.159f;
            const float middColX = 0f;//0f;
            const float rightColX = -0.15f;//-0.152f;

            const float topRowZ = 0.11f;// 1.146f;
            const float botRowZ = 0.270f;//1.06f;

            var rotation = Quaternion.Euler(60f, 180, 0);

            Module1 = CreateModuleDisplay(new Vector3(rightColX, botRowY, botRowZ), rotation);
            Module2 = CreateModuleDisplay(new Vector3(middColX, botRowY, botRowZ), rotation);
            Module3 = CreateModuleDisplay(new Vector3(leftColX, botRowY, botRowZ), rotation);
            Module4 = CreateModuleDisplay(new Vector3(rightColX, topRowY, topRowZ), rotation);
            Module5 = CreateModuleDisplay(new Vector3(middColX, topRowY, topRowZ), rotation);
            Module6 = CreateModuleDisplay(new Vector3(leftColX, topRowY, topRowZ), rotation);
        }

        private GameObject CreateModuleDisplay(Vector3 position, Quaternion rotation)
        {
            const float scale = 0.215f;

            Canvas canvas = new GameObject("Canvas", typeof(RectTransform)).AddComponent<Canvas>();
            Transform t = canvas.transform;
            t.SetParent(this.transform, false);
            canvas.sortingLayerID = 1;

            uGUI_GraphicRaycaster raycaster = canvas.gameObject.AddComponent<uGUI_GraphicRaycaster>();

            var rt = t as RectTransform;
            RectTransformExtensions.SetParams(rt, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f));
            RectTransformExtensions.SetSize(rt, 0.5f, 0.5f);

            t.localPosition = position;
            t.localRotation = rotation;
            t.localScale = new Vector3(scale, scale, scale);

            canvas.scaleFactor = 0.01f;
            canvas.renderMode = RenderMode.WorldSpace;
            canvas.referencePixelsPerUnit = 100;

            CanvasScaler scaler = canvas.gameObject.AddComponent<CanvasScaler>();
            scaler.dynamicPixelsPerUnit = 20;

            uGUI_Icon icon = canvas.gameObject.AddComponent<uGUI_Icon>();

            return canvas.gameObject;
        }

        private void UnlockDefaultModuleSlots()
        {
            this.Modules.AddSlots(SlotHelper.SlotNames);
        }

        public void OnHandHover(GUIHand guiHand)
        {
            if (!Buildable.constructed)
                return;

            HandReticle main = HandReticle.main;
            main.SetInteractText("Use Auxiliary Cyclop Upgrade Console");
            main.SetIcon(HandReticle.IconType.Hand, 1f);
#if DEBUG
            PositionStuff(Module4.GetComponent<Canvas>().gameObject);
#endif
        }

        public void OnHandClick(GUIHand guiHand)
        {
            if (!Buildable.constructed)
                return;

            Player main = Player.main;
            PDA pda = main.GetPDA();
            Inventory.main.SetUsedStorage(this.Modules, false);
            pda.Open(PDATab.Inventory, null, null, -1f);
        }

        private void OnEquip(string slot, InventoryItem item)
        {
            CyclopsUpgradeChange();
            UpdateVisuals();

            // Disallow deconstruction while there are modules in here
            Buildable.deconstructionAllowed = false;
        }

        private void OnUnequip(string slot, InventoryItem item)
        {
            CyclopsUpgradeChange();
            UpdateVisuals();

            bool allEmpty = true;

            foreach (string slotName in SlotHelper.SlotNames)
                allEmpty &= this.Modules.GetTechTypeInSlot(slotName) == TechType.None;

            // Deconstruction only allowed if all slots are empty
            Buildable.deconstructionAllowed = allEmpty;
        }

        internal void ConnectToCyclops(SubRoot parentCyclops, CyclopsManager manager = null)
        {
            this.ParentCyclops = parentCyclops;
            this.transform.SetParent(parentCyclops.transform);
            this.Manager = manager ?? CyclopsManager.GetAllManagers(parentCyclops);

            if (!this.Manager.UpgradeManager.AuxUpgradeConsoles.Contains(this))
            {
                this.Manager.UpgradeManager.AuxUpgradeConsoles.Add(this);
            }

            QuickLogger.Debug("Auxiliary Upgrade Console has been connected", true);
        }

        internal void CyclopsUpgradeChange()
        {
            if (this.ParentCyclops is null)
                return;

            this.ParentCyclops.subModulesDirty = true;
        }

        private static readonly Vector2 SpritePivot = new Vector2(0.5f, 0.5f);

        private void UpdateVisuals()
        {
            if (Module1 is null)
            {
                AddModuleSpriteHandlers();
            }

            SetModuleVisibility("Module1", Module1);
            SetModuleVisibility("Module2", Module2);
            SetModuleVisibility("Module3", Module3);
            SetModuleVisibility("Module4", Module4);
            SetModuleVisibility("Module5", Module5);
            SetModuleVisibility("Module6", Module6);
        }

        private void SetModuleVisibility(string slot, GameObject module)
        {
            if (module is null)
            {
                QuickLogger.Debug($"SetModuleVisibility in slot {slot} module was null", true);
                return;
            }

            TechType techType = this.Modules.GetTechTypeInSlot(slot);

            bool hasItem = techType != TechType.None;

            uGUI_Icon icon = module.GetComponent<uGUI_Icon>();

            if (hasItem)
            {
                Atlas.Sprite atlasSprite = SpriteManager.Get(techType);

                if (atlasSprite is null)
                    QuickLogger.Debug($"sprite for {module.name} was null when it should not have been", true);

                icon.sprite = atlasSprite;
            }
            else
            {
                icon.sprite = null; // Clear the sprite when empty                
            }

            module.SetActive(hasItem);
            icon.enabled = hasItem;
        }

        public void OnProtoSerialize(ProtobufSerializer serializer)
        {
            foreach (string slot in SlotHelper.SlotNames)
            {
                EmModuleSaveData savedModule = SaveData.GetModuleInSlot(slot);
                InventoryItem item = this.Modules.GetItemInSlot(slot);

                if (item == null)
                {
                    savedModule.ItemID = (int)TechType.None;
                    savedModule.RemainingCharge = -1f;
                }
                else
                {
                    savedModule.ItemID = (int)item.item.GetTechType();

                    Battery battery = item.item.GetComponent<Battery>();

                    if (battery == null)
                    {
                        savedModule.RemainingCharge = -1f;
                    }
                    else
                    {
                        savedModule.RemainingCharge = battery._charge;
                    }
                }

            }

            SaveData.Save();
        }

        public void OnProtoDeserialize(ProtobufSerializer serializer)
        {
            if (this.Modules == null)
                InitializeModules();

            this.Modules.Clear();
        }

        public void OnProtoSerializeObjectTree(ProtobufSerializer serializer)
        {
        }

        public void OnProtoDeserializeObjectTree(ProtobufSerializer serializer)
        {
            if (SaveData.Load())
            {
                // Because the items here aren't being serialized with everything else normally,
                // I've used custom save data to handle whatever gets left in these slots.

                // The following is a recreation of the essential parts of the Equipment.ResponseEquipment method.
                foreach (string slot in SlotHelper.SlotNames)
                {
                    // These slots need to be added before we can add items to them
                    this.Modules.AddSlot(slot);

                    EmModuleSaveData savedModule = SaveData.GetModuleInSlot(slot);

                    if (savedModule.ItemID == 0) // (int)TechType.None
                        continue; // Nothing here

                    InventoryItem spanwedItem = CyclopsModule.SpawnCyclopsModule((TechType)savedModule.ItemID);

                    if (spanwedItem is null)
                        continue;

                    if (savedModule.RemainingCharge > 0f) // Modules without batteries are stored with a -1 value for charge
                        spanwedItem.item.GetComponent<Battery>().charge = savedModule.RemainingCharge;

                    this.Modules.AddItem(slot, spanwedItem, true);
                }
            }
            else
            {
                UnlockDefaultModuleSlots();
            }
        }

        public GameObject Module1;

        public GameObject Module2;

        public GameObject Module3;

        public GameObject Module4;

        public GameObject Module5;

        public GameObject Module6;

        public ChildObjectIdentifier ModulesRoot;

        [ProtoMember(3, OverwriteList = true)]
        [NonSerialized]
        public AuxUpgradeConsoleSaveData SaveData;
#if DEBUG
        // Also shamelessly copied from RandyKnapp
        // https://github.com/RandyKnapp/SubnauticaModSystem/blob/master/SubnauticaModSystem/HabitatControlPanel/HabitatControlPanel.cs#L711
        public void PositionStuff(GameObject thing)
        {
            Transform t = thing.transform;
            float amount = 0.005f;

            if (Input.GetKeyDown(KeyCode.Keypad8))
            {
                t.localPosition += new Vector3(0, amount, 0);
                PrintStuff(thing);
            }
            else if (Input.GetKeyDown(KeyCode.Keypad5))
            {
                t.localPosition += new Vector3(0, -amount, 0);
                PrintStuff(thing);
            }
            else if (Input.GetKeyDown(KeyCode.Keypad6))
            {
                t.localPosition += new Vector3(amount, 0, 0);
                PrintStuff(thing);
            }
            else if (Input.GetKeyDown(KeyCode.Keypad4))
            {
                t.localPosition += new Vector3(-amount, 0, 0);
                PrintStuff(thing);
            }
            else if (Input.GetKeyDown(KeyCode.Keypad1))
            {
                t.localPosition += new Vector3(0, 0, amount);
                PrintStuff(thing);
            }
            else if (Input.GetKeyDown(KeyCode.Keypad7))
            {
                t.localPosition += new Vector3(0, 0, -amount);
                PrintStuff(thing);
            }

            if (Input.GetKeyDown(KeyCode.KeypadPlus))
            {
                t.localScale += new Vector3(amount, amount, amount);
                PrintStuff(thing);
            }
            else if (Input.GetKeyDown(KeyCode.KeypadMinus))
            {
                t.localScale -= new Vector3(amount, amount, amount);
                PrintStuff(thing);
            }
        }

        private void PrintStuff(GameObject thing)
        {
            Transform t = thing.transform;
            QuickLogger.Debug(thing.name + " p=" + t.localPosition.ToString("G5") + "s=" + t.localScale.ToString("G5"), true);
        }
#endif
    }
}

