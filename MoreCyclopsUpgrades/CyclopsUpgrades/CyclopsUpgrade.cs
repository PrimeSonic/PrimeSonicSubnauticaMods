namespace MoreCyclopsUpgrades.CyclopsUpgrades
{
    using MoreCyclopsUpgrades.Modules;
    using System;
    using System.Collections.Generic;

    public delegate void UpgradeEvent(SubRoot cyclops);
    public delegate void UpgradeEventSlotBound(SubRoot cyclops, Equipment modules, string slot);

    /// <summary>
    /// Represents all the behaviors for a cyclops upgrade module at the time of the module being installed and counted.
    /// </summary>
    public class CyclopsUpgrade
    {
        /// <summary>
        /// The TechType that identifies this type of upgrade module.
        /// </summary>
        public readonly TechType techType;

        private int count = 0;

        internal bool IsPowerProducer = false;

        /// <summary>
        /// Gets the number of copies of this upgrade module type currently installed in the cyclops.
        /// This value will not exceed <see cref="Count"/>.
        /// </summary>
        /// <value>
        /// The total number of upgrade modules of this <see cref="techType"/> found.
        /// </value>
        public int Count => Math.Min(this.MaxCount, count);

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
        /// Initializes a new instance of the <see cref="CyclopsUpgrade"/> class.
        /// </summary>
        /// <param name="techType">The TechType of the upgrade module.</param>
        public CyclopsUpgrade(TechType techType)
        {
            this.techType = techType;
        }

        internal virtual void UpgradesCleared(SubRoot cyclops)
        {
            OnClearUpgrades?.Invoke(cyclops);
            count = 0;
        }

        internal virtual void UpgradeCounted(SubRoot cyclops, Equipment modules, string slot)
        {
            count++;

            OnUpgradeCounted?.Invoke(cyclops, modules, slot);
        }

        internal virtual void UpgradesFinished(SubRoot cyclops)
        {
            if (count == 0)
                return;

            if (count > this.MaxCount)
            {
                ErrorMessage.AddMessage($"Cannot exceed more than {this.MaxCount} {CyclopsModule.CyclopsModulesByTechType[techType].NameID}");
                return;
            }

            OnFinishedUpgrades?.Invoke(cyclops);

            if (count == this.MaxCount)
            {
                ErrorMessage.AddMessage($"Maximum number of {CyclopsModule.CyclopsModulesByTechType[techType].NameID} reached");
                return;
            }
        }

        internal virtual void RegisterSelf(IDictionary<TechType, CyclopsUpgrade> dictionary)
        {
            dictionary.Add(techType, this);
        }


    }
}
