namespace MoreScannerRoomUpgrades.Monobehaviors
{
    using System.Collections.Generic;
    using Craftables;
    using UnityEngine;
    using UWE;

    internal class VehicleMapScanner : MonoBehaviour
    {
        private const int ScanRadius = 250; // Half as much as the base radius of the MapRoomFunctionality
        private const int ScanArea = ScanRadius * ScanRadius;
        private const float ScanRange = ScanRadius;
        private const float PowerDrainPerSecond = 0.05f;
        private const double ScanInterval = 10f;
        private const float ScanDistanceInterval = 50f;

        public int NumberOfNodesScanned { get; private set; }
        public Vector3 LastScanOrigin { get; private set; } = Vector3.zero;

        private readonly List<ResourceTracker.ResourceInfo> resourceNodes = new List<ResourceTracker.ResourceInfo>();

        internal bool IsPowered { get; private set; }

        private float timeLastPowerDrain;
        private double timeLastScan;
        private bool reevaluateScanOrigin = false;

        private float Distance(Vector3 a, Vector3 b) => (a - b).sqrMagnitude;

        internal Vehicle LinkedVehicle { get; private set; }

        internal bool IsVehicleLinked => this.LinkedVehicle != null;

        internal bool IsScanActive { get; private set; }

        internal Vector3 LinkedVehiclePosition => this.LinkedVehicle.transform.position;

        internal TechType TypeToScan { get; set; } = TechType.None;

        internal PowerRelay VehiclePowerRelay { get; private set; } = null;

        internal bool IsVehiclePowered() => this.VehiclePowerRelay != null && (!GameModeUtils.RequiresPower() || this.VehiclePowerRelay.IsPowered());

        internal void LinkVehicle(Vehicle vehicle)
        {
            this.LinkedVehicle = vehicle;
            this.VehiclePowerRelay = vehicle.gameObject.GetComponentInParent<PowerRelay>();
            vehicle.modules.onUnequip += OnEquipmentRemoved;
            this.IsPowered = IsVehiclePowered();

            if (this.VehiclePowerRelay != null)
            {
                this.VehiclePowerRelay.powerDownEvent.AddHandler(base.gameObject, new Event<PowerRelay>.HandleFunction(OnPowerDown));
                this.VehiclePowerRelay.powerUpEvent.AddHandler(base.gameObject, new Event<PowerRelay>.HandleFunction(OnPowerUp));
                this.IsPowered = IsVehiclePowered();
            }
        }

        internal void UnlinkVehicle()
        {
            if (!this.IsVehicleLinked)
                return;

            this.LinkedVehicle = null;
            this.VehiclePowerRelay = null;
            this.IsScanActive = false;
            this.TypeToScan = TechType.None;
            this.IsPowered = false;
            this.LinkedVehicle.modules.onUnequip -= OnEquipmentRemoved;
        }

        private void OnEquipmentRemoved(string slot, InventoryItem item)
        {
            int count = this.LinkedVehicle.modules.GetCount(VehicleMapScannerModule.ItemID);

            if (count == 0 || item.item.gameObject == this.gameObject)
            {
                UnlinkVehicle();
            }
        }

        private void OnPowerUp(PowerRelay powerRelay)
        {
            StopScanning();
            //this.ambientSound.Play(); TODO
        }

        private void OnPowerDown(PowerRelay powerRelay)
        {
            //this.ambientSound.Stop(); TODO
            this.IsPowered = !GameModeUtils.RequiresPower();
            StopScanning();
        }

        public void OnResourceDiscovered(ResourceTracker.ResourceInfo info)
        {
            if (this.TypeToScan == info.techType && Distance(this.LinkedVehiclePosition, info.position) <= 250000f)
            {
                resourceNodes.Add(info);
            }
        }

        public void OnResourceRemoved(ResourceTracker.ResourceInfo info)
        {
            if (this.TypeToScan == info.techType)
            {
                resourceNodes.Remove(info);
            }
        }

        private void Start()
        {
            ResourceTracker.onResourceDiscovered += OnResourceDiscovered;
            ResourceTracker.onResourceRemoved += OnResourceRemoved;
        }

        private void Update()
        {
            if (!this.IsScanActive || !this.IsPowered)
                return;

            UpdateScanning();
            UpdatePowerConsumption();
        }

        private void OnDestroy()
        {
            UnlinkVehicle();
            ResourceTracker.onResourceDiscovered -= OnResourceDiscovered;
            ResourceTracker.onResourceRemoved -= OnResourceRemoved;
        }

        public void StartScanning(TechType newTypeToScan)
        {
            this.TypeToScan = newTypeToScan;

            Vector3 vehiclePosition = this.LinkedVehiclePosition;
            this.LastScanOrigin = new Vector3(vehiclePosition.x, vehiclePosition.y, vehiclePosition.z);
            ObtainResourceNodes(this.TypeToScan);

            this.IsScanActive = (this.TypeToScan != TechType.None);
            this.NumberOfNodesScanned = 0;
            timeLastScan = 0;
        }

        public void StopScanning()
        {
            this.TypeToScan = TechType.None;
            this.LastScanOrigin = Vector3.zero;
            this.IsScanActive = false;
        }

        private void ObtainResourceNodes(TechType typeToScan)
        {
            resourceNodes.Clear();
            Vector3 scannerPosition = LinkedVehiclePosition;
            Dictionary<string, ResourceTracker.ResourceInfo>.ValueCollection nodes = ResourceTracker.GetNodes(typeToScan);
            if (nodes != null)
            {
                foreach (ResourceTracker.ResourceInfo resourceInfo in nodes)
                {
                    if (Distance(scannerPosition, resourceInfo.position) <= ScanArea)
                    {
                        resourceNodes.Add(resourceInfo);
                    }
                }
            }

            resourceNodes.Sort(delegate (ResourceTracker.ResourceInfo a, ResourceTracker.ResourceInfo b)
            {
                float distanceToA = Distance(a.position, scannerPosition);
                float distanceToB = Distance(b.position, scannerPosition);

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
                this.VehiclePowerRelay.ConsumeEnergy(PowerDrainPerSecond, out float amountConsumed);
                timeLastPowerDrain = Time.time;
            }
        }

        private void UpdatePowerConsumption()
        {
            if (reevaluateScanOrigin && Distance(this.LastScanOrigin, this.LinkedVehiclePosition) >= ScanDistanceInterval)
            {
                reevaluateScanOrigin = false;
                ObtainResourceNodes(this.TypeToScan);
            }
        }
    }
}
