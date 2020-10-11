namespace CustomBatteries.Items
{
	using UnityEngine;

	public abstract class CbPlaceTool : PlaceTool, IProtoEventListener
	{
		[SerializeField]
		public bool HasBeenPlaced = false;

		public override void OnPlace()
		{
			base.OnPlace();
            GameObject model = GetModel();
            if (!HasBeenPlaced)
            {
                // If model is correct, apply translation.
                if (model != null)
                    model.transform.localPosition += GetTranslate();
                HasBeenPlaced = true;
			}
            // If model is correct, refresh SkyApplier.
            if (model != null)
            {
                SkyApplier sa = model.GetComponent<SkyApplier>() ?? model.GetComponentInParent<SkyApplier>();
                if (sa != null)
                    Object.DestroyImmediate(sa);
                sa = model.AddComponent<SkyApplier>();
                if (sa != null)
                {
                    sa.anchorSky = Skies.Auto;
                    Renderer[] rends = model.GetComponentsInChildren<Renderer>();
                    if (rends == null || rends.Length <= 0)
                        rends = model.GetComponents<Renderer>();
                    sa.renderers = rends;
                    sa.dynamic = true;
                    sa.updaterIndex = 0;
                    sa.enabled = true;
                    sa.RefreshDirtySky();
                }
            }
        }

		public void OnProtoDeserialize(ProtobufSerializer serializer)
		{
			if (!HasBeenPlaced)
            {
                GameObject model = GetModel();
                // If model is correct, apply translation.
                if (model != null)
                    model.transform.localPosition += GetTranslate();
                HasBeenPlaced = true;
			}
		}

		public void OnProtoSerialize(ProtobufSerializer serializer) { }

        internal abstract GameObject GetModel();
        internal abstract Vector3 GetTranslate();
	}

	internal class CustomBatteryPlaceTool : CbPlaceTool
	{
        internal override GameObject GetModel() => this.gameObject.FindChild("model");

        internal override Vector3 GetTranslate() => new Vector3(0.0f, 0.069f, 0.0f);
    }

	internal class CustomPowerCellPlaceTool : CbPlaceTool
	{
        internal override GameObject GetModel() => this.gameObject.FindChild("engine_power_cell_01") ?? this.gameObject.FindChild("engine_power_cell_ion");

        internal override Vector3 GetTranslate() => new Vector3(0.0f, 0.138f, 0.0f);
    }
}
