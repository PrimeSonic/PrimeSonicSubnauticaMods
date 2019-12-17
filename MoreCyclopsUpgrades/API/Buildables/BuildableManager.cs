namespace MoreCyclopsUpgrades.API.Buildables
{
    using System;
    using System.Collections.Generic;
    using MoreCyclopsUpgrades.API.General;
    using UnityEngine;

    /// <summary>
    /// A generic <see cref="IAuxCyclopsManager"/> that can track buildables in the Cyclops
    /// </summary>
    /// <typeparam name="BuildableMono">The type of the uildable mono.</typeparam>
    /// <seealso cref="IAuxCyclopsManager" />
    public abstract class BuildableManager<BuildableMono> : IAuxCyclopsManager
        where BuildableMono : MonoBehaviour, ICyclopsBuildable
    {
        protected readonly List<BuildableMono> TrackedBuildables = new List<BuildableMono>();
        private readonly List<BuildableMono> tempBuildables = new List<BuildableMono>();

        public readonly SubRoot Cyclops;

        public int TrackedBuildablesCount => TrackedBuildables.Count;

        protected BuildableManager(SubRoot cyclops)
        {
            Cyclops = cyclops;
        }

        public abstract bool Initialize(SubRoot cyclops);

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

        public void AddBuildable(BuildableMono buildable)
        {
            if (!TrackedBuildables.Contains(buildable))
                TrackedBuildables.Add(buildable);
        }

        public void RemoveBuildable(BuildableMono buildable)
        {
            TrackedBuildables.Remove(buildable);
        }

        protected abstract void ConnectWithManager(BuildableMono buildable);

        public void ApplyToAll(Action<BuildableMono> action)
        {
            for (int b = 0; b < TrackedBuildables.Count; b++)
                action.Invoke(TrackedBuildables[b]);
        }

        public bool FindFirst(bool result, Func<BuildableMono, bool> condition, Action actionOnHit)
        {
            for (int b = 0; b < TrackedBuildables.Count; b++)
            {
                if (result == condition.Invoke(TrackedBuildables[b]))
                {
                    actionOnHit.Invoke();
                    return result;
                }
            }

            return !result;
        }
    }
}
