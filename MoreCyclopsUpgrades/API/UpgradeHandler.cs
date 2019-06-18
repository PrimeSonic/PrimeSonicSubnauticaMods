namespace MoreCyclopsUpgrades.API
{
    using System;
    using System.Collections.Generic;
    using MoreCyclopsUpgrades.Managers;

    /// <summary>
    /// Defines a method that creates a new <see cref="UpgradeHandler"/> when needed by the <seealso cref="UpgradeManager"/>.
    /// </summary>
    /// <returns>A newly instantiated <see cref="UpgradeHandler"/> ready to handle upgrade events.</returns>
    public delegate UpgradeHandler UpgradeHandlerCreateEvent(SubRoot cyclops);

    /// <summary>
    /// Defines a method to invoke that takes a cyclops reference as its only parameter. Used for <seealso cref="UpgradeHandler.OnClearUpgrades"/> and <see cref="UpgradeHandler.OnFinishedWithUpgrades"/>.
    /// </summary>
    /// <param name="cyclops">The cyclops where the event took place.</param>
    public delegate void UpgradeEvent();

    /// <summary>
    /// Defines a method to invoke that takes all the needed references to identify a single upgrade module instance. Used for <seealso cref="UpgradeHandler.OnUpgradeCounted"/>.
    /// </summary>
    /// <param name="cyclops">The cyclops where the event took place.</param>
    /// <param name="modules">The equipment module where the event took place.</param>
    /// <param name="slot">The equipment slot where the event took place.</param>
    public delegate void UpgradeEventSlotBound(Equipment modules, string slot);

    /// <summary>
    /// Defines a method to invoke that returns whether or not an item is allowed in or out. Used for <seealso cref="UpgradeHandler.IsAllowedToAdd"/> and <seealso cref="UpgradeHandler.IsAllowedToRemove"/>.
    /// </summary>
    /// <param name="cyclops">The cyclops where the event took place.</param>
    /// <param name="item">The item being checked.</param>
    /// <param name="verbose">if set to <c>true</c> verbose text display was requested; Otherwise <c>false</c>.</param>
    /// <returns></returns>
    public delegate bool UpgradeAllowedEvent(Pickupable item, bool verbose);

    /// <summary>
    /// Represents all the behaviors for a cyclops upgrade module at the time of the module being installed and counted.
    /// </summary>
    public class UpgradeHandler
    {
        /// <summary>
        /// The cyclops sub where this upgrade handler is being used.
        /// </summary>
        public readonly SubRoot cyclops;

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
            internal set => count = Math.Min(this.MaxCount, value);
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
        /// This event is invoked when upgrades are being cleared, right before <see cref="Count"/> is reset.<para/>
        /// This happens every time upgrades are changed.
        /// </summary>
        public UpgradeEvent OnClearUpgrades;

        /// <summary>
        /// This event is invoked when a copy of this module's <see cref="techType"/> is found and counted.<para/>
        /// This will happen for each copy found in the cyclops.
        /// </summary>
        public UpgradeEventSlotBound OnUpgradeCounted;

        /// <summary>
        /// This event is invoked after all upgrade modules have been found.<para/>
        /// This instance's event will only be called if there is at least 1 copy of this upgrade module's <see cref="techType"/>.<para/>
        /// This happens every time upgrades are changed.
        /// </summary>
        public UpgradeEvent OnFinishedWithUpgrades;

        /// <summary>
        /// This event is invoked after all upgrade modules have been found.<para/>
        /// This instance's event will only be called if no copies of this upgrade module's <see cref="techType"/> were found.<para/>
        /// This happens every time upgrades are changed.
        /// </summary>
        public UpgradeEvent OnFinishedWithoutUpgrades;

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
        /// Initializes a new instance of the <see cref="UpgradeHandler"/> class.
        /// </summary>
        /// <param name="techType">The TechType of the upgrade module.</param>
        public UpgradeHandler(TechType techType, SubRoot cyclops)
        {
            this.techType = techType;
            this.cyclops = cyclops;
        }

        internal virtual void UpgradesCleared()
        {
            OnClearUpgrades?.Invoke();
            count = 0;
        }

        internal virtual void UpgradeCounted(Equipment modules, string slot)
        {
            count++;
            OnUpgradeCounted?.Invoke(modules, slot);
        }

        internal virtual void UpgradesFinished()
        {
            if (count > this.MaxCount)
            {
                OnFinishedWithoutUpgrades?.Invoke();
            }
            else
            {
                OnFinishedWithUpgrades?.Invoke();

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
        }

        internal virtual void RegisterSelf(IDictionary<TechType, UpgradeHandler> dictionary)
        {
            dictionary.Add(techType, this);
        }

        internal virtual bool CanUpgradeBeAdded(Pickupable item, bool verbose)
        {
            if (IsAllowedToAdd != null)
            {
                return IsAllowedToAdd.Invoke(item, verbose);
            }

            return count < this.MaxCount;
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
