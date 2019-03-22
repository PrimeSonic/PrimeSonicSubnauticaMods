namespace MoreCyclopsUpgrades.CyclopsUpgrades
{
    using Common;
    using System;
    using System.Collections.Generic;

    public delegate UpgradeHandler HandlerCreator();
    public delegate void UpgradeEvent(SubRoot cyclops);
    public delegate void UpgradeEventSlotBound(SubRoot cyclops, Equipment modules, string slot);

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

        internal bool IsPowerProducer = false;

        /// <summary>
        /// Set this to <c>false</c> to ignore the value of <see cref="Count"/> when determining if <see cref="OnFinishedUpgrades"/> and <see cref="OnFirstTimeMaxCountReached"/>
        /// should be executed when all upgrades have been counted.
        /// </summary>
        protected bool CheckCountOnFinished = true;

        /// <summary>
        /// Gets or sets the name of this upgrade handler used in logs.
        /// </summary>
        /// <value>
        /// The name for this instance used for logging.
        /// </value>
        public string LoggingName { get; set; } = null;

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
            if (CheckCountOnFinished)
            {
                if (count == 0)
                    return;

                if (count > this.MaxCount)
                    return;
            }

            OnFinishedUpgrades?.Invoke(cyclops);

            if (CheckCountOnFinished)
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
        }

        internal virtual void RegisterSelf(IDictionary<TechType, UpgradeHandler> dictionary)
        {
            QuickLogger.Info($"{this.LoggingName ?? techType.AsString()} upgrade registered");
            dictionary.Add(techType, this);
        }
    }
}
