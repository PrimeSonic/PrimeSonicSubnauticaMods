namespace IonCubeGenerator.Mono
{
    using Common;
    using IonCubeGenerator.Buildable;
    using UnityEngine;

    internal partial class CubeGeneratorMono
    {
        private const int ContainerHeight = 2;
        private const int ContainerWidth = 2;
        private const int MaxAvailableSpaces = ContainerHeight * ContainerWidth;

        private ItemsContainer _cubeContainer = null;
        private ChildObjectIdentifier _containerRoot = null;
        private Constructable _buildable = null;

        private bool IsConstructed => _buildable != null && _buildable.constructed;
        internal int CurrentCubeCount => _cubeContainer.count;

        public void Awake()
        {
            if (_buildable == null)
            {
                _buildable = GetComponentInParent<Constructable>();
            }

            if (_saveData == null)
            {
                string id = GetComponentInParent<PrefabIdentifier>().Id;
                _saveData = new CubeGeneratorSaveData(id, MaxAvailableSpaces);
            }

            InitializeContainer();

            CreateDisplayedIonCube();
        }

        internal void ClearContainer()
        {
            InitializeContainer();

            _cubeContainer.Clear(false);
        }

        private void InitializeContainer()
        {
            if (_containerRoot == null)
            {
                QuickLogger.Debug("Initializing StorageRoot");
                var storageRoot = new GameObject("StorageRoot");
                storageRoot.transform.SetParent(this.transform, false);
                _containerRoot = storageRoot.AddComponent<ChildObjectIdentifier>();
            }

            if (_cubeContainer == null)
            {
                QuickLogger.Debug("Initializing Container");
                _cubeContainer = new ItemsContainer(ContainerWidth, ContainerHeight, _containerRoot.transform, CubeGeneratorBuildable.StorageLabel(), null);

                _cubeContainer.isAllowedToAdd += IsAllowedToAdd;
                _cubeContainer.isAllowedToRemove += IsAllowedToRemove;

                _cubeContainer.onAddItem += OnAddItemEvent;
                _cubeContainer.onRemoveItem += OnRemoveItemEvent;
            }
        }

        private bool IsAllowedToAdd(Pickupable pickupable, bool verbose)
        {
            return false;
        }

        private bool IsAllowedToRemove(Pickupable pickupable, bool verbose)
        {
            return true;
        }

        private void OnAddItemEvent(InventoryItem item)
        {
            _buildable.deconstructionAllowed = false;
        }

        private void OnRemoveItemEvent(InventoryItem item)
        {
            TryStartingNextCube();
            _buildable.deconstructionAllowed = _cubeContainer.count == 0;
        }

        private void OpenStorageState()
        {
            QuickLogger.Debug($"Storage Button Clicked", true);

            if (!this.IsConstructed)
                return;

            Player main = Player.main;
            PDA pda = main.GetPDA();
            Inventory.main.SetUsedStorage(_cubeContainer, false);
            pda.Open(PDATab.Inventory, null, null, 4f);
        }
    }
}
