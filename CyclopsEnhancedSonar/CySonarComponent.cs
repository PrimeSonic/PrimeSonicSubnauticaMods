namespace CyclopsEnhancedSonar
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    // Code adapted from the original CyclopsNearFieldSonar mod by frigidpenguin
    internal class CySonarComponent : MonoBehaviour
    {
        private static IEnumerator<YieldInstruction> EachFrameUntil(Func<bool> action)
        {
            while (!action())
            {
                yield return null;
            }
        }

        private readonly GameObject template = CraftData.GetPrefabForTechType(TechType.Seaglide);

        public void SetMapState(bool state)
        {
            if (state)
                script?.EnableMap();
            else
                script?.DisableMap();
        }

        private const float scale = 6.699605f;
        private const float fadeRadius = 1.503953f;
        private const float shipScale = 0.2f;
        private readonly Vector3 position = new Vector3(-0.9762846f, 2, -10.6917f);
        private readonly Vector3 shipPosition = new Vector3(0, 0, 0);

        private VehicleInterface_Terrain script;
        private Material material;
        private GameObject ship;

        private void Start()
        {
            Transform root = this.gameObject.transform.Find("SonarMap_Small");
            var holder = new GameObject("NearFieldSonar");
            holder.transform.SetParent(root, false);
            holder.transform.localScale = Vector3.one * 0.1f;

            GameObject prefab = template.GetComponent<VehicleInterface_MapController>().interfacePrefab;
            var hologram = GameObject.Instantiate(prefab);
            hologram.transform.SetParent(holder.transform, false);

            script = hologram.GetComponentInChildren<VehicleInterface_Terrain>();
            script.active = true;
            script.EnableMap();

            StartCoroutine(EachFrameUntil(() =>
            {
                material = script.materialInstance;
                return UpdateParameters();
            }));

            ship = GameObject.Instantiate(this.gameObject.transform.Find("HolographicDisplay/CyclopsMini_Mid").gameObject);
            ship.transform.SetParent(root, false);

            MeshRenderer[] cyclopsMeshRenderers = ship.transform.GetComponentsInChildren<MeshRenderer>();

            foreach (MeshRenderer meshRenderer in cyclopsMeshRenderers)
            {
                if (meshRenderer.gameObject.name.StartsWith("cyclops_room_"))
                {
                    meshRenderer.enabled = false;
                }
            }

            MeshRenderer oldShipRenderer = root.Find("CyclopsMini").GetComponent<MeshRenderer>();
            Material shipMaterial = oldShipRenderer.material;

            foreach (MeshRenderer meshRenderer in cyclopsMeshRenderers)
                meshRenderer.sharedMaterial = shipMaterial;

            oldShipRenderer.enabled = false;
            root.Find("Base").GetComponent<MeshRenderer>().enabled = false;
        }

        private bool UpdateParameters()
        {
            if (material == null)
                return false;

            script.hologramHolder.transform.localScale = Vector3.one * scale;
            script.hologramHolder.transform.localPosition = position * (1 / scale);

            material.SetFloat("_FadeRadius", fadeRadius);

            ship.transform.localPosition = shipPosition;
            ship.transform.localScale = Vector3.one * shipScale;

            return true;
        }
    }
}
