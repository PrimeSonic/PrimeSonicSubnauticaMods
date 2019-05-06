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
        private const float DelayedStartTime = 0.5f;
        private const float RepeatingUpdateInterval = 1f;

        private ItemsContainer _cubeContainer = null;
        private ChildObjectIdentifier _containerRoot = null;
        private Constructable _buildable = null;

        private bool IsConstructed => _buildable != null && _buildable.constructed;
        private int CurrentCubeCount => _cubeContainer.count;

        public override void Awake()
        {
            base.Awake();

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

            RetrieveAnimator();
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
    }
}
