namespace MoreCyclopsUpgrades.AuxConsole
{
    using System;
    using System.Reflection;
    using Common;
    using CommonCyclopsBuildables;
    using Managers;
    using MoreCyclopsUpgrades.API.Upgrades;
    using ProtoBuf;
    using UnityEngine;
    using UnityEngine.UI;

    [ProtoContract]
    internal class AuxCyUpgradeConsoleMono : HandTarget, IHandTarget, IProtoEventListener, IProtoTreeEventListener, ICyclopsBuildable
    {
        private SubRoot ParentCyclops;
        private UpgradeManager UpgradeManager;

        public Equipment Modules { get; private set; }

        public bool IsConnectedToCyclops => ParentCyclops != null && UpgradeManager != null;

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
                SaveData = new AuxCyUpgradeConsoleSaveData(id);
            }

            if (this.Modules == null)
            {
                InitializeModules();
            }
        }

        private void Start()
        {
            SubRoot cyclops = base.GetComponentInParent<SubRoot>();

            if (cyclops == null)
            {
                QuickLogger.Debug("CyUpgradeConsoleMono: Could not find Cyclops during Start. Attempting external syncronize.");
                for (int i = 0; i < CyclopsManager.Managers.Count; i++)
                    CyclopsManager.Managers[i].Upgrade.SyncBuildables();
            }
            else
            {
                QuickLogger.Debug("CyUpgradeConsoleMono: Parent cyclops found!");
                ConnectToCyclops(cyclops);
            }
        }

        private void InitializeModules()
        {
            QuickLogger.Debug("Initializing Equipment");
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

            Type cyConsoleType = typeof(AuxCyUpgradeConsoleMono);
            MethodInfo myOnEquipMethod = cyConsoleType.GetMethod("OnEquip", BindingFlags.Instance | BindingFlags.NonPublic);
            MethodInfo myOnUnequipMethod = cyConsoleType.GetMethod("OnUnequip", BindingFlags.Instance | BindingFlags.NonPublic);

            var onEquipDelegate = Delegate.CreateDelegate(typeof(Equipment.OnEquip), this, myOnEquipMethod);
            var onUnequipDelegate = Delegate.CreateDelegate(typeof(Equipment.OnUnequip), this, myOnUnequipMethod);

            onEquipInfo.AddEventHandler(this.Modules, onEquipDelegate);
            onUnequipInfo.AddEventHandler(this.Modules, onUnequipDelegate);

            UnlockDefaultModuleSlots();
            AddModuleSpriteHandlers();
        }

        private void AddModuleSpriteHandlers()
        {
            const float topRowY = 1.15f;//-0.109f;
            const float botRowY = 1.075f;//-0.239f;

            const float leftColX = 0.15f;//0.159f;
            const float middColX = 0f;//0f;
            const float rightColX = -0.15f;//-0.152f;

            const float topRowZ = 0.12f;// 1.146f;
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
            icon.enabled = false;
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
            main.SetInteractText(AuxCyUpgradeConsole.OnHoverText);
            main.SetIcon(HandReticle.IconType.Hand, 1f);
        }

        public void OnHandClick(GUIHand guiHand)
        {
            if (!Buildable.constructed)
                return;

            PdaOverlayManager.StartConnectingToPda(this.Modules);

            Player main = Player.main;
            PDA pda = main.GetPDA();
            Inventory.main.SetUsedStorage(this.Modules, false);
            pda.Open(PDATab.Inventory, null, new PDA.OnClose((PDA closingPda) => PdaOverlayManager.DisconnectFromPda()), -1f);
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

            for (int s = 0; s < SlotHelper.SlotNames.Length; s++)
                allEmpty &= this.Modules.GetTechTypeInSlot(SlotHelper.SlotNames[s]) == TechType.None;

            // Deconstruction only allowed if all slots are empty
            Buildable.deconstructionAllowed = allEmpty;
        }

        internal void ConnectToCyclops(SubRoot parentCyclops, UpgradeManager manager = null)
        {
            ParentCyclops = parentCyclops;
            this.transform.SetParent(parentCyclops.transform);
            UpgradeManager = manager ?? CyclopsManager.GetManager(parentCyclops).Upgrade;

            UpgradeManager upgradeManager = UpgradeManager;
            upgradeManager.AddBuildable(this);

            Equipment console = this.Modules;
            upgradeManager.AttachEquipmentEvents(ref console);
            QuickLogger.Debug("Auxiliary Upgrade Console has been connected", true);
        }

        internal void CyclopsUpgradeChange()
        {
            if (ParentCyclops is null)
                return;

            ParentCyclops.subModulesDirty = true;
        }

        private void UpdateVisuals()
        {
            if (Module1 == null)
                AddModuleSpriteHandlers();

            SetModuleVisibility("Module1", Module1);
            SetModuleVisibility("Module2", Module2);
            SetModuleVisibility("Module3", Module3);
            SetModuleVisibility("Module4", Module4);
            SetModuleVisibility("Module5", Module5);
            SetModuleVisibility("Module6", Module6);
        }

        private void SetModuleVisibility(string slot, GameObject canvasObject)
        {
            if (canvasObject == null)
                return;

            uGUI_Icon icon = canvasObject.GetComponent<uGUI_Icon>();

            if (icon == null)
                return;

            TechType techType = this.Modules.GetTechTypeInSlot(slot);
            bool hasItem = techType != TechType.None;

            if (hasItem)
            {
                Atlas.Sprite atlasSprite = SpriteManager.Get(techType);

                if (atlasSprite == null)
                    QuickLogger.Debug($"sprite for {canvasObject.name} was null when it should not have been", true);

                icon.sprite = atlasSprite;
            }
            else
            {
                icon.sprite = null; // Clear the sprite when empty                
            }

            canvasObject.SetActive(hasItem);
            icon.enabled = hasItem;
        }

        public void OnProtoSerialize(ProtobufSerializer serializer)
        {
            for (int s = 0; s < SlotHelper.SlotNames.Length; s++)
            {
                string slot = SlotHelper.SlotNames[s];
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

                QuickLogger.Debug("Loading save data");
                // The following is a recreation of the essential parts of the Equipment.ResponseEquipment method.
                for (int s = 0; s < SlotHelper.SlotNames.Length; s++)
                {
                    string slot = SlotHelper.SlotNames[s];
                    // These slots need to be added before we can add items to them
                    this.Modules.AddSlot(slot);

                    EmModuleSaveData savedModule = SaveData.GetModuleInSlot(slot);

                    if (savedModule.ItemID == 0) // (int)TechType.None
                        continue; // Nothing here

                    var techtype = (TechType)savedModule.ItemID;
                    InventoryItem spanwedItem = CyclopsUpgrade.SpawnCyclopsModule(techtype);

                    if (spanwedItem is null)
                        continue;

                    QuickLogger.Debug($"Spawned in {techtype.AsString()} from save data");

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
        public AuxCyUpgradeConsoleSaveData SaveData;
        //#if DEBUG
        //        // Also shamelessly copied from RandyKnapp
        //        // https://github.com/RandyKnapp/SubnauticaModSystem/blob/master/SubnauticaModSystem/HabitatControlPanel/HabitatControlPanel.cs#L711
        //        public void PositionStuff(GameObject thing)
        //        {
        //            Transform t = thing.transform;
        //            float amount = 0.005f;

        //            if (Input.GetKeyDown(KeyCode.Keypad8))
        //            {
        //                t.localPosition += new Vector3(0, amount, 0);
        //                PrintStuff(thing);
        //            }
        //            else if (Input.GetKeyDown(KeyCode.Keypad5))
        //            {
        //                t.localPosition += new Vector3(0, -amount, 0);
        //                PrintStuff(thing);
        //            }
        //            else if (Input.GetKeyDown(KeyCode.Keypad6))
        //            {
        //                t.localPosition += new Vector3(amount, 0, 0);
        //                PrintStuff(thing);
        //            }
        //            else if (Input.GetKeyDown(KeyCode.Keypad4))
        //            {
        //                t.localPosition += new Vector3(-amount, 0, 0);
        //                PrintStuff(thing);
        //            }
        //            else if (Input.GetKeyDown(KeyCode.Keypad1))
        //            {
        //                t.localPosition += new Vector3(0, 0, amount);
        //                PrintStuff(thing);
        //            }
        //            else if (Input.GetKeyDown(KeyCode.Keypad7))
        //            {
        //                t.localPosition += new Vector3(0, 0, -amount);
        //                PrintStuff(thing);
        //            }

        //            if (Input.GetKeyDown(KeyCode.KeypadPlus))
        //            {
        //                t.localScale += new Vector3(amount, amount, amount);
        //                PrintStuff(thing);
        //            }
        //            else if (Input.GetKeyDown(KeyCode.KeypadMinus))
        //            {
        //                t.localScale -= new Vector3(amount, amount, amount);
        //                PrintStuff(thing);
        //            }
        //        }

        //        private void PrintStuff(GameObject thing)
        //        {
        //            Transform t = thing.transform;
        //            QuickLogger.Debug(thing.name + " p=" + t.localPosition.ToString("G5") + "s=" + t.localScale.ToString("G5"), true);
        //        }
        //#endif
    }
}

