namespace MoreScannerRoomUpgrades.Managers
{
    using System.Collections;
    using System.Collections.Generic;
    using Craftables;
    using UnityEngine;
    using UnityEngine.Rendering;
    using UWE;

    internal class VehicleMapScanner : MonoBehaviour
    {
        private const float hologramRadius = 1f;
        public int mapChunkSize = 32;
        public int mapLOD = 2;
        public const int scanRadius = 250; // Half as much as the base radius of the MapRoomFunctionality
        public const int scanArea = scanRadius * scanRadius;
        private const float scanRange = 150f;
        private const float rangePerUpgrade = 50f;
        private const float baseScanTime = 14f;
        private const float scanTimeReductionPerUpgrade = 3f;
        private const float rotationTime = 50f;
        private const float powerPerSecond = 0.5f;

        private const float mapScale = hologramRadius / scanRange;

        private const double scanInterval = 10f;

        public int numNodesScanned;
        private readonly List<ResourceTracker.ResourceInfo> resourceNodes = new List<ResourceTracker.ResourceInfo>();
        private readonly List<GameObject> mapBlips = new List<GameObject>();
        private bool powered = true;
        private float timeLastPowerDrain;
        private double timeLastScan;
        private float prevFadeRadius;
        private Material matInstance;
        private bool prevScanActive;
        private GameObject mapWorld;


        internal Vehicle LinkedVehicle;

        internal bool IsVehicleLinked => LinkedVehicle != null;

        internal bool IsScanActive { get; private set; }

        internal TechType TypeToScan { get; set; } = TechType.None;

        internal PowerRelay VehiclePowerRelay { get; private set; } = null;

        internal void LinkToVehicle(Vehicle vehicle)
        {
            LinkedVehicle = vehicle;
            vehicle.modules.onUnequip += OnEquipmentRemoved;
            this.VehiclePowerRelay = vehicle.gameObject.GetComponentInParent<PowerRelay>();
        }

        internal void UnlinkVehicle()
        {
            LinkedVehicle = null;
            this.IsScanActive = false;
            this.TypeToScan = TechType.None;
            LinkedVehicle.modules.onUnequip -= OnEquipmentRemoved;
        }

        private void OnEquipmentRemoved(string slot, InventoryItem item)
        {
            int count = LinkedVehicle.modules.GetCount(VehicleMapScannerModule.ItemID);

            if (count == 0 || item.item.gameObject == this.gameObject)
            {
                UnlinkVehicle();
            }
        }

        private void OnPowerUp(PowerRelay powerRelay)
        {
            powered = true;
            timeLastScan = 0.0;
            //this.ambientSound.Play(); TODO
        }

        private void OnPowerDown(PowerRelay powerRelay)
        {
            powered = false;
            //this.ambientSound.Stop(); TODO
            this.IsScanActive = false;
            this.TypeToScan = TechType.None;
        }

        public void OnResourceDiscovered(ResourceTracker.ResourceInfo info)
        {
            if (this.TypeToScan == info.techType && (LinkedVehicle.transform.position - info.position).sqrMagnitude <= 250000f)
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

        public void ReloadMapWorld()
        {
            Destroy(mapWorld);
            base.StartCoroutine(LoadMapWorld());
        }

        private void Start()
        {
            this.ReloadMapWorld();
            if (this.TypeToScan != TechType.None)
            {
                double num = this.timeLastScan;
                int num2 = this.numNodesScanned;
                this.StartScanning(this.TypeToScan);
                this.timeLastScan = num;
                this.numNodesScanned = num2;
            }
            ResourceTracker.onResourceDiscovered += this.OnResourceDiscovered;
            ResourceTracker.onResourceRemoved += this.OnResourceRemoved;
            // todo
            //this.matInstance = UnityEngine.Object.Instantiate<Material>(      );
            //this.matInstance.SetFloat(ShaderPropertyID._ScanIntensity, 0f);
            //this.matInstance.SetVector(ShaderPropertyID._MapCenterWorldPos, base.transform.position);

            if (VehiclePowerRelay != null)
            {
                VehiclePowerRelay.powerDownEvent.AddHandler(base.gameObject, new Event<PowerRelay>.HandleFunction(this.OnPowerDown));
                VehiclePowerRelay.powerUpEvent.AddHandler(base.gameObject, new Event<PowerRelay>.HandleFunction(this.OnPowerUp));
                this.powered = (!GameModeUtils.RequiresPower() || VehiclePowerRelay.IsPowered());
            }

        }

        private void OnDestroy()
        {
            UnityEngine.Object.Destroy(this.matInstance);
            ResourceTracker.onResourceDiscovered -= this.OnResourceDiscovered;
            ResourceTracker.onResourceRemoved -= this.OnResourceRemoved;
        }

        private IEnumerator LoadMapWorld()
        {
            mapWorld = new GameObject("Map");
            mapWorld.transform.SetParent(LinkedVehicle.transform, false);
            LargeWorldStreamer streamer = LargeWorldStreamer.main;
            Int3 wsCenterBlock = streamer.GetBlock(base.transform.position);
            Int3 wsMinBlock = wsCenterBlock - 500;
            Int3 wsMaxBlock = wsCenterBlock + 500;
            Int3 msCenterBlock = wsCenterBlock >> mapLOD;
            Int3 msMinChunk = (wsMinBlock >> mapLOD) / mapChunkSize;
            Int3 msMaxChunk = (wsMaxBlock >> mapLOD) / mapChunkSize;
            float chunkScale = mapScale * (1 << mapLOD);
            Int3.RangeEnumerator iter = Int3.Range(msMinChunk, msMaxChunk);
            while (iter.MoveNext())
            {
                Int3 chunkId = iter.Current;
                string chunkPath = string.Format("WorldMeshes/Mini{0}/Chunk-{1}-{2}-{3}", new object[]
                {
                mapLOD,
                chunkId.x,
                chunkId.y,
                chunkId.z
                });
                ResourceRequest request = Resources.LoadAsync<Mesh>(chunkPath);
                yield return request;
                var mesh = (Mesh)request.asset;
                if (mesh)
                {
                    Int3 msChunkBlock = chunkId * mapChunkSize;
                    Int3 msChunkPos = msChunkBlock - msCenterBlock;
                    var chunk = new GameObject(chunkPath);
                    chunk.transform.SetParent(mapWorld.transform, false);
                    chunk.transform.localScale = new Vector3(chunkScale, chunkScale, chunkScale);
                    chunk.transform.localPosition = msChunkPos.ToVector3() * chunkScale;
                    MeshFilter mf = chunk.AddComponent<MeshFilter>();
                    mf.sharedMesh = mesh;
                    MeshRenderer renderer = chunk.AddComponent<MeshRenderer>();
                    renderer.shadowCastingMode = ShadowCastingMode.Off;
                    renderer.sharedMaterial = matInstance;
                    renderer.receiveShadows = false;
                }
            }
            yield break;
        }

        public void StartScanning(TechType newTypeToScan)
        {
            this.TypeToScan = newTypeToScan;
            ObtainResourceNodes(this.TypeToScan);
            mapBlips.Clear();
            //UnityEngine.Object.Destroy(this.mapBlipRoot);
            this.IsScanActive = (this.TypeToScan != TechType.None);
            numNodesScanned = 0;
            timeLastScan = 0.0;
        }

        private void ObtainResourceNodes(TechType typeToScan)
        {
            resourceNodes.Clear();
            Vector3 scannerPos = LinkedVehicle.transform.position;
            Dictionary<string, ResourceTracker.ResourceInfo>.ValueCollection nodes = ResourceTracker.GetNodes(typeToScan);
            if (nodes != null)
            {
                foreach (ResourceTracker.ResourceInfo resourceInfo in nodes)
                {
                    if ((scannerPos - resourceInfo.position).sqrMagnitude <= scanArea)
                    {
                        resourceNodes.Add(resourceInfo);
                    }
                }
            }
            resourceNodes.Sort(delegate (ResourceTracker.ResourceInfo a, ResourceTracker.ResourceInfo b)
            {
                float sqrMagnitude = (a.position - scannerPos).sqrMagnitude;
                float sqrMagnitude2 = (b.position - scannerPos).sqrMagnitude;
                return sqrMagnitude.CompareTo(sqrMagnitude2);
            });
        }

        private void UpdateBlips()
        {
            if (this.IsScanActive)
            {
                int num = Mathf.Min(numNodesScanned + 1, resourceNodes.Count);
                if (num != numNodesScanned)
                {
                    numNodesScanned = num;
                }
                for (int i = 0; i < num; i++)
                {
                    ResourceTracker.ResourceInfo resourceInfo = resourceNodes[i];
                    if (i >= mapBlips.Count)
                    {
                        mapBlips.Add(this.gameObject);
                    }
                    mapBlips[i].SetActive(true);
                }
                for (int j = num; j < mapBlips.Count; j++)
                {
                    mapBlips[j].SetActive(false);
                }
            }
        }

        private void UpdateScanning()
        {
            DayNightCycle main = DayNightCycle.main;
            if (!main)
            {
                return;
            }
            double timePassed = main.timePassed;
            if (timeLastScan + scanInterval <= timePassed && powered)
            {
                timeLastScan = timePassed;
                UpdateBlips();
                float num = 1f / (scanRange * mapScale);
                if (prevFadeRadius != num)
                {
                    matInstance.SetFloat(ShaderPropertyID._FadeRadius, num);
                    prevFadeRadius = num;
                }
            }
            if (this.IsScanActive != prevScanActive)
            {
                matInstance.SetFloat(ShaderPropertyID._ScanIntensity, (!this.IsScanActive) ? 0f : 0.35f);
                prevScanActive = this.IsScanActive;
            }
            if (this.IsScanActive && this.IsVehicleLinked && timeLastPowerDrain + 1f < Time.time)
            {
                this.VehiclePowerRelay.ConsumeEnergy(0.5f, out float amountConsumed);
                timeLastPowerDrain = Time.time;
            }
        }
    }
}
