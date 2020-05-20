namespace MoreCyclopsUpgrades.API.Upgrades
{
    using System;
    using System.Collections.Generic;
    using Common;
    using MoreCyclopsUpgrades.API.Buildables;

    /// <summary>
    /// Represents all the behaviors for a cyclops upgrade module at the time of the module being installed and counted.
    /// </summary>
    public class UpgradeHandler
    {
        /// <summary>
        /// Defines a method to invoke with no paramters. Used for <seealso cref="OnClearUpgrades"/> and <see cref="OnFinishedUpgrades"/>.
        /// </summary>
        public delegate void UpgradeEvent();

        /// <summary>
        /// Defines a method to invoke that takes all the needed references to identify a single upgrade module instance. Used for <seealso cref="OnUpgradeCountedDetailed" />.
        /// </summary>
        /// <param name="modules">The equipment module where the event took place.</param>
        /// <param name="slot">The equipment slot where the event took place.</param>
        /// <param name="inventoryItem">The inventory item.</param>
        public delegate void UpgradeEventSlotBound(Equipment modules, string slot, InventoryItem inventoryItem);

        /// <summary>
        /// Defines a method to invoke that returns whether or not an item is allowed in or out. Used for <seealso cref="IsAllowedToAdd"/> and <seealso cref="IsAllowedToRemove"/>.
        /// </summary>
        /// <param name="item">The item being checked.</param>
        /// <param name="verbose">if set to <c>true</c> verbose text display was requested; Otherwise <c>false</c>.</param>
        /// <returns></returns>
        public delegate bool UpgradeAllowedEvent(Pickupable item, bool verbose);

        /// <summary>
        /// The cyclops sub where this upgrade handler is being used.
        /// </summary>
        public readonly SubRoot Cyclops;

        /// <summary>
        /// The TechType that identifies this type of upgrade module.
        /// </summary>
        public readonly TechType TechType;

        private int count = 0;
        private bool maxedOut = false;

        /// <summary>
        /// Gets the number of copies of this upgrade module type currently installed in the cyclops.
        /// This value will not exceed <see cref="Count"/>.
        /// </summary>
        /// <value>
        /// The total number of upgrade modules of this <see cref="TechType"/> found.
        /// </value>
        public int Count
        {
            get => Math.Min(this.MaxCount, count);
            internal set => count = Math.Min(this.MaxCount, value);
        }

        /// <summary>
        /// Gets or sets the maximum number of copies of the upgrade module allowed.
        /// </summary>
        /// <value>
        /// The maximum count.
        /// </value>
        public int MaxCount { get; set; } = 12;

        /// <summary>
        /// Gets a value indicating whether the maximum number of copies of this upgrade module has been reached.
        /// </summary>
        /// <value>
        ///   <c>true</c> if <see cref="Count"/> now equals or would have exceeded <see cref="MaxCount"/>; otherwise, <c>false</c>.
        /// </value>
        public bool MaxLimitReached => count == this.MaxCount;

        /// <summary>
        /// Gets a value indicating whether the maximum number of copies of this upgrade module has been exceeded.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the real count of modules is greater than <see cref="MaxCount"/>; otherwise, <c>false</c>.
        /// </value>
        public bool ExceededMaxLimit => count > this.MaxCount;

        /// <summary>
        /// Gets a value indicating whether there is at least one copy of this upgrade module in the cyclops.
        /// </summary>
        /// <value>
        ///   <c>true</c> if <see cref="Count"/> is at least 1; otherwise, <c>false</c>.
        /// </value>
        public bool HasUpgrade => this.Count > 0;

        /// <summary>
        /// This event is invoked when upgrades are being cleared, right before <see cref="Count"/> is reset.<para/>
        /// This happens every time upgrades are changed.
        /// </summary>
        public UpgradeEvent OnClearUpgrades;

        /// <summary>
        /// This event is invoked when a copy of this module's <see cref="TechType"/> is found and counted.<para/>
        /// This will happen for each copy found in the cyclops.<para/>
        /// Unlike <see cref="OnUpgradeCounted"/>, this event will contain parameter references to the upgrade module and its location.
        /// </summary>
        public UpgradeEventSlotBound OnUpgradeCountedDetailed;

        /// <summary>
        /// This event is invoked when a copy of this module's <see cref="TechType"/> is found and counted.<para/>
        /// This will happen for each copy found in the cyclops.<para/>
        /// Unlike <see cref="OnUpgradeCountedDetailed"/>, this event will send no extra parameters.
        /// </summary>
        public UpgradeEvent OnUpgradeCounted;

        /// <summary>
        /// This event is invoked after all upgrade modules have been found.<para/>
        /// This happens every time upgrades are changed.
        /// </summary>
        public UpgradeEvent OnFinishedUpgrades;

        /// <summary>
        /// This event is invoked after all upgrade modules have been found and counted,<para/>
        /// but only if this is the first time that the <see cref="MaxCount"/> of upgrades has been achieved.
        /// </summary>
        public Action OnFirstTimeMaxCountReached;

        /// <summary>
        /// This event is invoked when the player is attempting to add an upgrade of this type to an upgrade console.
        /// </summary>
        public UpgradeAllowedEvent IsAllowedToAdd;

        /// <summary>
        /// This event is invoked when the player is attempting to remove an upgrade of this type to an upgrade console.
        /// </summary>
        public UpgradeAllowedEvent IsAllowedToRemove;

        /// <summary>
        /// Initializes a new instance of the <see cref="UpgradeHandler" /> class.
        /// </summary>
        /// <param name="techType">The TechType of the upgrade module.</param>
        /// <param name="cyclops">The cyclops where the handler is being registered.</param>
        public UpgradeHandler(TechType techType, SubRoot cyclops)
        {
            TechType = techType;
            Cyclops = cyclops;
        }

        private readonly List<InventoryItem> trackedItems = new List<InventoryItem>();

        /// <summary>
        /// The collection of upgrade module <see cref="InventoryItem"/>s that are tracked by this upgrade handler.
        /// </summary>
        public IEnumerable<InventoryItem> TrackedItems => trackedItems;

        /// <summary>
        /// Gets the assembly name of the source mod that created this UpgradeHandler.
        /// </summary>
        public string SourceMod { get; internal set; }

        internal virtual void UpgradesCleared()
        {
            OnClearUpgrades?.Invoke();
            count = 0;
            trackedItems.Clear();
        }

        internal virtual void UpgradeCounted(UpgradeSlot upgradeSlot)
        {
            count++;
            InventoryItem trackedItem = upgradeSlot.GetItemInSlot();
            trackedItems.Add(trackedItem);
            OnUpgradeCounted?.Invoke();
            OnUpgradeCountedDetailed?.Invoke(upgradeSlot.equipment, upgradeSlot.slotName, trackedItem);
        }

        internal virtual void UpgradesFinished()
        {
            OnFinishedUpgrades?.Invoke();

            CheckIfMaxedOut();
        }

        internal void CheckIfMaxedOut()
        {
            if (!maxedOut) // If we haven't maxed out before, check this block
            {
                maxedOut = count == this.MaxCount; // Are we maxed out now?

                if (maxedOut) // If we are, invoke the event
                    OnFirstTimeMaxCountReached?.Invoke();
            }
            else // If we are in this block, that means we maxed out in a previous cycle
            {
                maxedOut = count == this.MaxCount; // Evaluate this again in case an upgrade was removed
            }
        }

        internal virtual void RegisterSelf(IDictionary<TechType, UpgradeHandler> dictionary)
        {
            dictionary.Add(TechType, this);
            QuickLogger.Debug($"Added UpgradeHandler for '{TechType.AsString()}'");
        }

        internal virtual bool CanUpgradeBeAdded(Pickupable item, bool verbose)
        {
            if (IsAllowedToAdd != null)
            {
                return IsAllowedToAdd.Invoke(item, verbose);
            }

            if (count < this.MaxCount)
                return true;

            // When maxed out, we can still allow currently equipped items to be moved around.
            if (trackedItems.Count == this.MaxCount)
            {
                for (int i = 0; i < this.MaxCount; i++)
                {
                    if (trackedItems[i] == item.inventoryItem)
                        return true;
                }
            }

            return false;
        }

        internal virtual bool CanUpgradeBeRemoved(Pickupable item, bool verbose)
        {
            if (IsAllowedToRemove != null)
            {
                return IsAllowedToRemove.Invoke(item, verbose);
            }

            return true;
        }
    }
}
