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
                    model.transform.localPosition -= GetTranslate();
                HasBeenPlaced = true;
            }
        }

        public void OnProtoDeserialize(ProtobufSerializer serializer)
        {
            if (!HasBeenPlaced)
            {
                GameObject model = GetModel();
                // If model is correct, apply translation.
                if (model != null)
                    model.transform.localPosition -= GetTranslate();
                HasBeenPlaced = true;
            }
        }

        public void OnProtoSerialize(ProtobufSerializer serializer) { }

        internal abstract GameObject GetModel();
        internal abstract Vector3 GetTranslate();
    }

    internal class CustomBatteryPlaceTool : CbPlaceTool
    {
        internal override GameObject GetModel() => this.gameObject.FindChild("battery_01") ?? this.gameObject.FindChild("battery_ion");

        internal override Vector3 GetTranslate() => new Vector3(0.0f, 0.069f, 0.0f);
    }

    internal class CustomPowerCellPlaceTool : CbPlaceTool
    {
        internal override GameObject GetModel() => this.gameObject.FindChild("engine_power_cell_01") ?? this.gameObject.FindChild("engine_power_cell_ion");

        internal override Vector3 GetTranslate() => new Vector3(0.0f, 0.0f, 0.0f);
    }
}
