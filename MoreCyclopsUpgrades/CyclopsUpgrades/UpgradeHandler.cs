namespace MoreCyclopsUpgrades.CyclopsUpgrades
{
    using MoreCyclopsUpgrades.Managers;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Defines a method that creates a new <see cref="UpgradeHandler"/> when needed by the <seealso cref="UpgradeManager"/>.
    /// </summary>
    /// <returns>A newly instantiated <see cref="UpgradeHandler"/> ready to handle upgrade events.</returns>
    public delegate UpgradeHandler HandlerCreator();

    /// <summary>
    /// Defines a method to invoke that takes a cyclops reference as its only parameter. Used for <seealso cref="UpgradeHandler.OnClearUpgrades"/> and <see cref="UpgradeHandler.OnFinishedUpgrades"/>.
    /// </summary>
    /// <param name="cyclops">The cyclops where the event took place.</param>
    public delegate void UpgradeEvent(SubRoot cyclops);

    /// <summary>
    /// Defines a method to invoke that takes all the needed references to identify a single upgrade module instance. Used for <seealso cref="UpgradeHandler.OnUpgradeCounted"/>.
    /// </summary>
    /// <param name="cyclops">The cyclops where the event took place.</param>
    /// <param name="modules">The equipment module where the event took place.</param>
    /// <param name="slot">The equipment slot where the event took place.</param>
    public delegate void UpgradeEventSlotBound(SubRoot cyclops, Equipment modules, string slot);

    /// <summary>
    /// Defines a method to invoke that returns whether or not an item is allowed in or out. Used for <seealso cref="UpgradeHandler.IsAllowedToAdd"/> and <seealso cref="UpgradeHandler.IsAllowedToRemove"/>.
    /// </summary>
    /// <param name="cyclops">The cyclops where the event took place.</param>
    /// <param name="item">The item being checked.</param>
    /// <param name="verbose">if set to <c>true</c> verbose text display was requested; Otherwise <c>false</c>.</param>
    /// <returns></returns>
    public delegate bool UpgradeAllowedEvent(SubRoot cyclops, Pickupable item, bool verbose);

    /// <summary>
    /// Represents all the behaviors for a cyclops upgrade module at the time of the module being installed and counted.
    /// </summary>
    public class UpgradeHandler
    {
        /// <summary>
        /// The TechType that identifies this type of upgrade module.
        /// </summary>
        public readonly TechType techType;

        private int count = 0;
        private bool maxedOut = false;

        /// <summary>
        /// Gets the number of copies of this upgrade module type currently installed in the cyclops.
        /// This value will not exceed <see cref="Count"/>.
        /// </summary>
        /// <value>
        /// The total number of upgrade modules of this <see cref="techType"/> found.
        /// </value>
        public int Count
        {
            get => Math.Min(this.MaxCount, count);
            internal set => count = value;
        }

        /// <summary>
        /// Gets or sets the maximum number of copies of the upgrade module allowed.
        /// </summary>
        /// <value>
        /// The maximum count.
        /// </value>
        public int MaxCount { get; set; } = 99;

        /// <summary>
        /// Gets a value indicating whether the maximum number of copies of this upgrade modules has been reached.
        /// </summary>
        /// <value>
        ///   <c>true</c> if <see cref="Count"/> now equals or would have exceeded <see cref="MaxCount"/>; otherwise, <c>false</c>.
        /// </value>
        public bool MaxLimitReached => count == this.MaxCount;

        /// <summary>
        /// Gets a value indicating whether there is at least one copy of this upgrade module in the cyclops.
        /// </summary>
        /// <value>
        ///   <c>true</c> if <see cref="Count"/> is at least 1; otherwise, <c>false</c>.
        /// </value>
        public bool HasUpgrade => this.Count > 0;

        /// <summary>
        /// This event is invoked when upgrades are cleared right before being re-counted.
        /// This happens every time upgrades are changed.
        /// </summary>
        public UpgradeEvent OnClearUpgrades;

        /// <summary>
        /// This event is invoked when a copy of this module's <see cref="techType"/> is found and counted.
        /// This will happen for each copy found in the cyclops.
        /// </summary>
        public UpgradeEventSlotBound OnUpgradeCounted;

        /// <summary>
        /// This event is invoked after all upgrade modules have been found.
        /// This instance's event will only be called if there is at least 1 copy of this upgrade module's <see cref="techType"/>.
        /// </summary>
        public UpgradeEvent OnFinishedUpgrades;

        /// <summary>
        /// This event is invoked after all upgrade modules have been found and counted,
        /// but only if this is the first time that the <see cref="MaxCount"/> of upgrades has been achived.
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
        /// Initializes a new instance of the <see cref="UpgradeHandler"/> class.
        /// </summary>
        /// <param name="techType">The TechType of the upgrade module.</param>
        public UpgradeHandler(TechType techType)
        {
            this.techType = techType;
        }

        internal virtual void UpgradesCleared(SubRoot cyclops)
        {
            count = 0;
            OnClearUpgrades?.Invoke(cyclops);
        }

        internal virtual void UpgradeCounted(SubRoot cyclops, Equipment modules, string slot)
        {
            count++;
            OnUpgradeCounted?.Invoke(cyclops, modules, slot);
        }

        internal virtual void UpgradesFinished(SubRoot cyclops)
        {
            if (count > this.MaxCount)
                return;

            OnFinishedUpgrades?.Invoke(cyclops);

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
            dictionary.Add(techType, this);
        }

        internal virtual bool CanUpgradeBeAdded(SubRoot cyclops, Pickupable item, bool verbose)
        {
            if (IsAllowedToAdd != null)
            {
                return IsAllowedToAdd.Invoke(cyclops, item, verbose);
            }

            return count < this.MaxCount;
        }

        internal virtual bool CanUpgradeBeRemoved(SubRoot cyclops, Pickupable item, bool verbose)
        {
            if (IsAllowedToRemove != null)
            {
                return IsAllowedToRemove.Invoke(cyclops, item, verbose);
            }

            return true;
        }
    }
}
