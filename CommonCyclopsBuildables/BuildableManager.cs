namespace CommonCyclopsBuildables
{
    using System;
    using System.Collections.Generic;
    using MoreCyclopsUpgrades.API.General;
    using UnityEngine;

    internal abstract class BuildableManager<BuildableMono> : IAuxCyclopsManager
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

        public void SyncBuildables()
        {
            tempBuildables.Clear();

            BuildableMono[] buildables = Cyclops.GetAllComponentsInChildren<BuildableMono>();

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
