namespace MoreScannerRoomUpgrades.Monobehaviors
{
    using System.Collections.Generic;
    using Common;
    using Craftables;
    using UnityEngine;
    using UWE;

    internal class VehicleMapScanner : MonoBehaviour
    {
        internal static readonly List<VehicleMapScanner> VehicleMapScanners = new List<VehicleMapScanner>();

        private const int ScanRadius = 250; // Half as much as the base radius of the MapRoomFunctionality
        private const int ScanArea = ScanRadius * ScanRadius;
        private const float ScanRange = ScanRadius;
        private const float PowerDrainPerSecond = 0.05f;
        private const double ScanInterval = 10f;
        private const float ScanDistanceInterval = 50f;
        private const float mapScale = 1f / ScanRadius;

        public int numNodesScanned;
        private float timeLastPowerDrain;
        private double timeLastScan;
        private bool reevaluateScanOrigin = false;
        private readonly List<ResourceTracker.ResourceInfo> resourceNodes = new List<ResourceTracker.ResourceInfo>();
        private readonly List<GameObject> mapBlips = new List<GameObject>();

        private Vector3 LastScanOrigin = Vector3.zero;
        private PowerRelay VehiclePowerRelay = null;
        private TechType TypeToScan = TechType.None;

        internal Vehicle LinkedVehicle { get; private set; }

        internal int LinkedVehicleSlotID { get; private set; } = -1;

        internal bool IsScanActive() => TypeToScan != TechType.None;

        private bool IsVehicleLinked() => this.LinkedVehicle != null;

        internal Vector3 LinkedVehiclePosition() => this.LinkedVehicle.transform.position;

        private bool IsVehiclePowered() => VehiclePowerRelay != null && (!GameModeUtils.RequiresPower() || VehiclePowerRelay.IsPowered());

        internal void LinkVehicle(Vehicle vehicle, int slotID)
        {
            this.LinkedVehicle = vehicle;
            this.LinkedVehicleSlotID = slotID;
            VehiclePowerRelay = vehicle.gameObject.GetComponentInParent<PowerRelay>();
            vehicle.modules.onUnequip += OnEquipmentRemoved;

            if (VehiclePowerRelay != null)
            {
                VehiclePowerRelay.powerDownEvent.AddHandler(base.gameObject, new Event<PowerRelay>.HandleFunction(OnPowerDown));
            }
        }

        internal void UnlinkVehicle()
        {
            if (!IsVehicleLinked())
                return;

            this.LinkedVehicle.modules.onUnequip -= OnEquipmentRemoved;
            this.LinkedVehicle = null;
            this.LinkedVehicleSlotID = -1;
            VehiclePowerRelay = null;
            TypeToScan = TechType.None;
        }

        private void OnEquipmentRemoved(string slot, InventoryItem item)
        {
            int count = this.LinkedVehicle.modules.GetCount(VehicleMapScannerModule.ItemID);

            if (count == 0 || item.item.gameObject == this.gameObject)
            {
                UnlinkVehicle();
            }
        }

        private void OnPowerDown(PowerRelay powerRelay)
        {
            //this.ambientSound.Stop(); TODO
            StopScanning();
        }

        public void OnResourceDiscovered(ResourceTracker.ResourceInfo info)
        {
            if (TypeToScan == info.techType && Utilities.Distance(LinkedVehiclePosition(), info.position) <= 250000f)
            {
                resourceNodes.Add(info);
            }
        }

        public void OnResourceRemoved(ResourceTracker.ResourceInfo info)
        {
            if (TypeToScan == info.techType)
            {
                resourceNodes.Remove(info);
            }
        }

        private void Start()
        {
            QuickLogger.Debug($"VehicleMapScanner started", true);

            ResourceTracker.onResourceDiscovered += OnResourceDiscovered;
            ResourceTracker.onResourceRemoved += OnResourceRemoved;

            if (!VehicleMapScanners.Contains(this))
            {
                VehicleMapScanners.Add(this);
            }
        }

        private void Update()
        {
            if (!IsScanActive() || !IsVehiclePowered())
                return;

            UpdateScanning();
        }

        private void OnDestroy()
        {
            UnlinkVehicle();

            ResourceTracker.onResourceDiscovered -= OnResourceDiscovered;
            ResourceTracker.onResourceRemoved -= OnResourceRemoved;

            if (VehicleMapScanners.Contains(this))
            {
                VehicleMapScanners.Remove(this);
            }
        }

        public void StartScanning(TechType newTypeToScan)
        {
            TypeToScan = newTypeToScan;

            Vector3 vehiclePosition = LinkedVehiclePosition();
            LastScanOrigin = new Vector3(vehiclePosition.x, vehiclePosition.y, vehiclePosition.z);

            QuickLogger.Debug($"Started scan for {newTypeToScan} at {LastScanOrigin}", true);
            ObtainResourceNodes(TypeToScan);

            numNodesScanned = 0;
            timeLastScan = 0;
        }

        public void StopScanning()
        {
            TypeToScan = TechType.None;
            LastScanOrigin = Vector3.zero;
        }

        private void ObtainResourceNodes(TechType typeToScan)
        {
            resourceNodes.Clear();
            Vector3 scannerPosition = LinkedVehiclePosition();
            Dictionary<string, ResourceTracker.ResourceInfo>.ValueCollection nodes = ResourceTracker.GetNodes(typeToScan);
            if (nodes != null)
            {
                foreach (ResourceTracker.ResourceInfo resourceInfo in nodes)
                {
                    if (Utilities.Distance(scannerPosition, resourceInfo.position) <= ScanArea)
                    {
                        resourceNodes.Add(resourceInfo);
                    }
                }
            }

            QuickLogger.Debug($"Found {resourceNodes} resource nodes", true);

            resourceNodes.Sort(delegate (ResourceTracker.ResourceInfo a, ResourceTracker.ResourceInfo b)
            {
                float distanceToA = Utilities.Distance(a.position, scannerPosition);
                float distanceToB = Utilities.Distance(b.position, scannerPosition);

                return distanceToA.CompareTo(distanceToB);
            });
        }

        private void UpdateScanning()
        {
            DayNightCycle main = DayNightCycle.main;
            if (!main)
            {
                return;
            }

            double timePassed = main.timePassed;
            if (timeLastScan + ScanInterval <= timePassed)
            {
                timeLastScan = timePassed;
                reevaluateScanOrigin = true;
            }

            if (timeLastPowerDrain + 1f < Time.time)
            {
                VehiclePowerRelay.ConsumeEnergy(PowerDrainPerSecond, out float amountConsumed);
                timeLastPowerDrain = Time.time;
            }

            if (reevaluateScanOrigin && Utilities.Distance(LastScanOrigin, LinkedVehiclePosition()) >= ScanDistanceInterval)
            {
                reevaluateScanOrigin = false;
                ObtainResourceNodes(TypeToScan);
            }
        }

        public IList<ResourceTracker.ResourceInfo> GetNodes() => resourceNodes;

        public void GetDiscoveredNodes(ICollection<ResourceTracker.ResourceInfo> outNodes)
        {
            int num = Mathf.Min(numNodesScanned, resourceNodes.Count);
            for (int i = 0; i < num; i++)
            {
                outNodes.Add(resourceNodes[i]);
            }
        }
    }
}
