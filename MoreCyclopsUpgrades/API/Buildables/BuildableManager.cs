namespace MoreCyclopsUpgrades.API.Buildables
{
    using System;
    using System.Collections.Generic;
    using MoreCyclopsUpgrades.API.General;
    using UnityEngine;

    /// <summary>
    /// A generic <see cref="IAuxCyclopsManager" /> that can track buildables in the Cyclops
    /// </summary>
    /// <typeparam name="BuildableMono">The type of the uildable mono.</typeparam>
    /// <seealso cref="IAuxCyclopsManager" />
    public abstract class BuildableManager<BuildableMono> : IAuxCyclopsManager
        where BuildableMono : MonoBehaviour, ICyclopsBuildable
    {
        /// <summary>
        /// The list of tracked buildables.
        /// </summary>
        protected readonly List<BuildableMono> TrackedBuildables = new List<BuildableMono>();

        private readonly List<BuildableMono> tempBuildables = new List<BuildableMono>();

        /// <summary>
        /// The cyclops <see cref="SubRoot"/> reference.
        /// </summary>
        public readonly SubRoot Cyclops;

        /// <summary>
        /// Gets the number of currently tracked buildables.
        /// </summary>
        /// <value>
        /// The tracked buildables count.
        /// </value>
        public int TrackedBuildablesCount => TrackedBuildables.Count;

        /// <summary>
        /// Initializes a new instance of the <see cref="BuildableManager{BuildableMono}"/> class.
        /// </summary>
        /// <param name="cyclops">The cyclops.</param>
        protected BuildableManager(SubRoot cyclops)
        {
            Cyclops = cyclops;
        }

        /// <summary>
        /// Initializes the auxiliary manager with the specified cyclops.<para />
        /// This method is invoked only after all <see cref="IAuxCyclopsManager" /> instances have been created.<para />
        /// Use this if you need to run any additional code after the constructor.
        /// </summary>
        /// <param name="cyclops">The cyclops this manager will handle.</param>
        /// <returns>
        ///   <c>True</c> if the initialization process succeeded; Otherwise returns <c>False</c>.
        /// </returns>
        public abstract bool Initialize(SubRoot cyclops);

        /// <summary>
        /// Synchronizes the buildables, executing the <see cref="ConnectWithManager"/> method on each one found.
        /// </summary>
        public virtual void SyncBuildables()
        {
            tempBuildables.Clear();

            if (Cyclops == null)
                return;

            BuildableMono[] buildables = Cyclops.GetComponentsInChildren<BuildableMono>();

            for (int b = 0; b < buildables.Length; b++)
            {
                BuildableMono buildable = buildables[b];

                if (tempBuildables.Contains(buildable))
                    continue; // Instances already found

                tempBuildables.Add(buildable);

                if (!buildable.IsConnectedToCyclops)
                {
                    ConnectWithManager(buildable);
                }
            }

            if (tempBuildables.Count != TrackedBuildables.Count)
            {
                TrackedBuildables.Clear();
                TrackedBuildables.AddRange(tempBuildables);
            }
        }

        /// <summary>
        /// Adds the buildable to the list tracked buildables. Does not invoke <see cref="ConnectWithManager"/>.
        /// </summary>
        /// <param name="buildable">The buildable.</param>
        public void AddBuildable(BuildableMono buildable)
        {
            if (!TrackedBuildables.Contains(buildable))
                TrackedBuildables.Add(buildable);
        }

        /// <summary>
        /// Removes the buildable from the list of tracked buildables.
        /// </summary>
        /// <param name="buildable">The buildable.</param>
        public void RemoveBuildable(BuildableMono buildable)
        {
            TrackedBuildables.Remove(buildable);
        }

        /// <summary>
        /// Connects the buildable with this manager.
        /// </summary>
        /// <param name="buildable">The buildable.</param>
        protected abstract void ConnectWithManager(BuildableMono buildable);

        /// <summary>
        /// Applies and <see cref="Action"/> to all buildables this manager tracks.
        /// </summary>
        /// <param name="action">The action.</param>
        public void ApplyToAll(Action<BuildableMono> action)
        {
            for (int b = 0; b < TrackedBuildables.Count; b++)
                action.Invoke(TrackedBuildables[b]);
        }

        /// <summary>
        /// Finds the first tracked buildable that satisfies the condition and optionally performs an action on it.
        /// </summary>
        /// <param name="result">if set to <c>true</c> [result].</param>
        /// <param name="condition">The condition.</param>
        /// <param name="actionOnHit">The action on hit.</param>
        /// <returns></returns>
        public bool FindFirst(bool result, Predicate<BuildableMono> condition, Action actionOnHit)
        {
            for (int b = 0; b < TrackedBuildables.Count; b++)
            {
                if (result == condition.Invoke(TrackedBuildables[b]))
                {
                    actionOnHit?.Invoke();
                    return result;
                }
            }

            return !result;
        }
    }
}
