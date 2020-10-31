namespace IonCubeGenerator.Mono
{
    // using Logger = QModManager.Utility.Logger;
    using IonCubeGenerator.Buildable;
    using IonCubeGenerator.Interfaces;
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    internal class CubeGeneratorContainer : ICubeContainer
    {
#if SUBNAUTICA
        private static readonly Vector2int CubeSize = CraftData.GetItemSize(TechType.PrecursorIonCrystal);
#elif BELOWZERO
        private static readonly Vector2int CubeSize = TechData.GetItemSize(TechType.PrecursorIonCrystal);
#endif
        internal static readonly GameObject CubePrefab = CraftData.GetPrefabForTechType(TechType.PrecursorIonCrystal);

        private const int ContainerHeight = 2;
        private const int ContainerWidth = 2;

        /// <summary>
        /// The maximum allowed storage in the container
        /// </summary>
        /// <returns>An <see cref="int"/> of storage slots</returns>
        internal const int MaxAvailableSpaces = ContainerHeight * ContainerWidth;

        private ItemsContainer _cubeContainer = null;
        private ChildObjectIdentifier _containerRoot = null;
        private Func<bool> isContstructed;

        public int NumberOfCubes
        {
            get => _cubeContainer.count;
            set
            {
                if (value < 0 || value > MaxAvailableSpaces)
                    return;

                if (value < _cubeContainer.count)
                {
                    do
                    {
                        RemoveSingleCube();
                    } while (value < _cubeContainer.count);
                }
                else if (value > _cubeContainer.count)
                {
                    do
                    {
                        SpawnCube();
                    } while (value > _cubeContainer.count);
                }
            }
        }

        public bool IsFull => _cubeContainer.count == MaxAvailableSpaces || !_cubeContainer.HasRoomFor(CubeSize.x, CubeSize.y);

        internal CubeGeneratorContainer(CubeGeneratorMono cubeGenerator)
        {
            isContstructed = () => { return cubeGenerator.IsConstructed; };

            if (_containerRoot == null)
            {
                // Logger.Log(Logger.Level.Debug, "Initializing StorageRoot");
                var storageRoot = new GameObject("StorageRoot");
                storageRoot.transform.SetParent(cubeGenerator.transform, false);
                _containerRoot = storageRoot.AddComponent<ChildObjectIdentifier>();
            }

            if (_cubeContainer == null)
            {
                // Logger.Log(Logger.Level.Debug, "Initializing Container");
                _cubeContainer = new ItemsContainer(ContainerWidth, ContainerHeight, _containerRoot.transform, CubeGeneratorBuildable.StorageLabel(), null);

                _cubeContainer.isAllowedToAdd += IsAllowedToAdd;
                _cubeContainer.isAllowedToRemove += IsAllowedToRemove;

                _cubeContainer.onAddItem += cubeGenerator.OnAddItemEvent;
                _cubeContainer.onRemoveItem += cubeGenerator.OnRemoveItemEvent;
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

        public void OpenStorage()
        {
            // Logger.Log(Logger.Level.Debug, $"Storage Button Clicked", showOnScreen: true);

            if (!isContstructed.Invoke())
                return;

            Player main = Player.main;
            PDA pda = main.GetPDA();
            Inventory.main.SetUsedStorage(_cubeContainer, false);
#if SUBNAUTICA
            pda.Open(PDATab.Inventory, null, null, 4f);
#elif BELOWZERO
            pda.Open(PDATab.Inventory, null, null);
#endif

        }

        internal bool SpawnCube()
        {
            if (this.IsFull)
                return false;

            var gameObject = GameObject.Instantiate<GameObject>(CubePrefab);
            CubePrefab.SetActive(false);
            gameObject.SetActive(true);

            Pickupable pickupable = gameObject.GetComponent<Pickupable>();
            pickupable.Pickup(false);
            var item = new InventoryItem(pickupable);

            _cubeContainer.UnsafeAdd(item);
            return true;
        }

        private void RemoveSingleCube()
        {
            IList<InventoryItem> cubes = _cubeContainer.GetItems(TechType.PrecursorIonCrystal);
            _cubeContainer.RemoveItem(cubes[0].item);
        }
    }
}
