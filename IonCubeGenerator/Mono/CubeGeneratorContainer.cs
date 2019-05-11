namespace IonCubeGenerator.Mono
{
    using Common;
    using IonCubeGenerator.Buildable;
    using IonCubeGenerator.Interfaces;
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    internal class CubeGeneratorContainer : ICubeContainer
    {
        private static readonly Vector2int CubeSize = CraftData.GetItemSize(TechType.PrecursorIonCrystal);
        private static readonly GameObject CubePrefab = CraftData.GetPrefabForTechType(TechType.PrecursorIonCrystal);

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
                if (value >= 0 && value < _cubeContainer.count)
                {
                    do
                    {
                        RemoveSingleCube();
                    } while (--value < _cubeContainer.count);
                }
                else if (value <= MaxAvailableSpaces && value > _cubeContainer.count)
                {
                    do
                    {
                        SpawnCube();
                    } while (++value > _cubeContainer.count);
                }
            }
        }

        public bool IsFull => _cubeContainer.count == MaxAvailableSpaces || !_cubeContainer.HasRoomFor(CubeSize.x, CubeSize.y);

        internal CubeGeneratorContainer(CubeGeneratorMono cubeGenerator)
        {
            isContstructed = () => { return cubeGenerator.IsConstructed; };

            if (_containerRoot == null)
            {
                QuickLogger.Debug("Initializing StorageRoot");
                var storageRoot = new GameObject("StorageRoot");
                storageRoot.transform.SetParent(cubeGenerator.transform, false);
                _containerRoot = storageRoot.AddComponent<ChildObjectIdentifier>();
            }

            if (_cubeContainer == null)
            {
                QuickLogger.Debug("Initializing Container");
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
            QuickLogger.Debug($"Storage Button Clicked", true);

            if (!isContstructed.Invoke())
                return;

            Player main = Player.main;
            PDA pda = main.GetPDA();
            Inventory.main.SetUsedStorage(_cubeContainer, false);
            pda.Open(PDATab.Inventory, null, null, 4f);
        }

        internal bool SpawnCube()
        {
            if (this.IsFull)
                return false;

            var gameObject = GameObject.Instantiate<GameObject>(CubePrefab);

            Pickupable pickupable = gameObject.GetComponent<Pickupable>().Pickup(false);
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
